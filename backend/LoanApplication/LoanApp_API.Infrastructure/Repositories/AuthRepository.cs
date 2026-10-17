using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public AuthRepository(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public User? GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(x => x.Email == email);
        }

        public Customers? GetCustomerByEmail(string email)
        {
            return _db.Customers.FirstOrDefault(x => x.Email == email);
        }

        public Customers Register(CustomerRegistrationDTO model)
        {
            var existingUser = GetUserByEmail(model.Email);
            var existingCustomer = GetCustomerByEmail(model.Email);

            if (existingUser != null || existingCustomer != null)
            {
                if (existingCustomer != null &&
                    existingCustomer.IsEmailVerified == 0)
                {
                    var newOtp = Random.Shared
                        .Next(100000, 1000000)
                        .ToString();

                    existingCustomer.EmailOtp = newOtp;
                    existingCustomer.OtpExpiry =
                        DateTime.UtcNow.AddMinutes(5);

                    _db.Customers.Update(existingCustomer);
                    _db.SaveChanges();

                    return existingCustomer;
                }

                throw new Exception("Email already registered");
            }

            var customer = _mapper.Map<Customers>(model);

            customer.IsEmailVerified = 0;

            customer.EmailOtp = Random.Shared
                .Next(100000, 1000000)
                .ToString();

            customer.OtpExpiry =
                DateTime.UtcNow.AddMinutes(5);

            return CreateCustomer(customer);
        }

        public User CreateUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public Customers CreateCustomer(Customers customer)
        {
            customer.Password =
                BCrypt.Net.BCrypt.HashPassword(customer.Password);

            _db.Customers.Add(customer);
            _db.SaveChanges();

            var user = new User
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Mobile = customer.MobileNo,
                Password = customer.Password,
                RoleId = 1,
                CustomerId = customer.CustomerId
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return customer;
        }

        public User? Login(string email, string password)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    password,
                    user.Password);

            if (!isPasswordValid)
                return null;

            return user;
        }

        public bool VerifyEmailOtp(string email, string otp)
        {
            var customer = _db.Customers
                .FirstOrDefault(x => x.Email == email);

            if (customer == null)
                return false;

            if (customer.IsEmailVerified == 1)
                return false;

            if (string.IsNullOrEmpty(customer.EmailOtp))
                return false;

            if (customer.OtpExpiry == null)
                return false;

            if (DateTime.UtcNow > customer.OtpExpiry)
                return false;

            if (customer.EmailOtp != otp)
                return false;

            customer.IsEmailVerified = 1;
            customer.EmailOtp = null;
            customer.OtpExpiry = null;

            _db.Customers.Update(customer);
            _db.SaveChanges();

            return true;
        }

        public bool MarkEmailVerified(string email)
        {
            var customer = _db.Customers
                .FirstOrDefault(x => x.Email == email);

            if (customer == null)
                return false;

            customer.IsEmailVerified = 1;
            customer.EmailOtp = null;
            customer.OtpExpiry = null;

            _db.Customers.Update(customer);
            _db.SaveChanges();

            return true;
        }

        public User CreateOfficer(User user)
        {
            user.Password =
                BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.RoleId = 2;
            user.CustomerId = null;

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }
    }
}