using AutoMapper;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        public User? GetUserByEmail(string email)
        {
            email = NormalizeEmail(email);

            return _db.Users.FirstOrDefault(
                x => x.Email.ToLower() == email);
        }

        public User? GetUserById(int userId)
        {
            return _db.Users.FirstOrDefault(
                x => x.UserId == userId);
        }

        public Customers? GetCustomerByEmail(string email)
        {
            email = NormalizeEmail(email);

            return _db.Customers.FirstOrDefault(
                x => x.Email.ToLower() == email);
        }

        public Customers Register(CustomerRegistrationDTO model)
        {
            var email = NormalizeEmail(model.Email);

            var existingCustomer = GetCustomerByEmail(email);

            if (existingCustomer != null)
            {
                if (existingCustomer.IsEmailVerified == 1)
                {
                    throw new InvalidOperationException(
                        "Email already registered");
                }

                // Generate a new OTP for an unverified customer.
                existingCustomer.EmailOtp =
                    Random.Shared.Next(100000, 1000000).ToString();

                existingCustomer.OtpExpiry =
                    DateTime.UtcNow.AddMinutes(5);

                _db.SaveChanges();

                return existingCustomer;
            }

            if (GetUserByEmail(email) != null)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }

            var customer = _mapper.Map<Customers>(model);

            customer.FirstName = model.FirstName.Trim();
            customer.LastName = model.LastName.Trim();
            customer.Email = email;
            customer.Password =
                BCrypt.Net.BCrypt.HashPassword(model.Password);

            customer.IsEmailVerified = 0;

            customer.EmailOtp =
                Random.Shared.Next(100000, 1000000).ToString();

            customer.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

            using var transaction =
                _db.Database.BeginTransaction();

            try
            {
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
                    CustomerId = customer.CustomerId,
                    IsTwoFactorEnabled = false
                };

                _db.Users.Add(user);
                _db.SaveChanges();

                transaction.Commit();

                return customer;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Customers CreateCustomer(Customers customer)
        {
            customer.Email = NormalizeEmail(customer.Email);

            if (GetCustomerByEmail(customer.Email) != null ||
                GetUserByEmail(customer.Email) != null)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }

            customer.Password =
                BCrypt.Net.BCrypt.HashPassword(customer.Password);

            using var transaction =
                _db.Database.BeginTransaction();

            try
            {
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
                    CustomerId = customer.CustomerId,
                    IsTwoFactorEnabled = false
                };

                _db.Users.Add(user);
                _db.SaveChanges();

                transaction.Commit();

                return customer;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public User CreateUser(User user)
        {
            user.Email = NormalizeEmail(user.Email);

            if (GetUserByEmail(user.Email) != null ||
                GetCustomerByEmail(user.Email) != null)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }

            user.Password =
                BCrypt.Net.BCrypt.HashPassword(user.Password);

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public User CreateOfficer(User user)
        {
            user.Email = NormalizeEmail(user.Email);

            if (GetUserByEmail(user.Email) != null ||
                GetCustomerByEmail(user.Email) != null)
            {
                throw new InvalidOperationException(
                    "Email already registered");
            }

            user.Password =
                BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.CustomerId = null;
            user.RoleId = 2;
            user.IsTwoFactorEnabled = false;

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public User? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            email = NormalizeEmail(email);

            var user = GetUserByEmail(email);

            if (user != null)
            {
                return VerifyPassword(user.Password, password)
                    ? user
                    : null;
            }

            // Recover an older customer record if its User record is missing.
            var customer = GetCustomerByEmail(email);

            if (customer == null ||
                customer.IsEmailVerified != 1 ||
                !VerifyPassword(customer.Password, password))
            {
                return null;
            }

            if (!_db.Roles.Any(x => x.RoleId == 1))
            {
                throw new InvalidOperationException(
                    "Customer role with RoleId 1 was not found");
            }

            user = new User
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Mobile = customer.MobileNo,
                Password = customer.Password,
                RoleId = 1,
                CustomerId = customer.CustomerId,
                IsTwoFactorEnabled = false
            };

            _db.Users.Add(user);

            try
            {
                _db.SaveChanges();
                return user;
            }
            catch (DbUpdateException)
            {
                _db.Entry(user).State = EntityState.Detached;

                user = GetUserByEmail(email);

                return user != null &&
                       VerifyPassword(user.Password, password)
                    ? user
                    : null;
            }
        }

        private static bool VerifyPassword(
            string? passwordHash,
            string password)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(
                    password, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyEmailOtp(string email, string otp)
        {
            var customer = GetCustomerByEmail(email);

            if (customer == null ||
                customer.IsEmailVerified == 1 ||
                string.IsNullOrWhiteSpace(customer.EmailOtp) ||
                customer.EmailOtp != otp ||
                customer.OtpExpiry == null ||
                customer.OtpExpiry < DateTime.UtcNow)
            {
                return false;
            }

            customer.IsEmailVerified = 1;
            customer.EmailOtp = null;
            customer.OtpExpiry = null;

            _db.SaveChanges();

            return true;
        }

        public bool MarkEmailVerified(string email)
        {
            var customer = GetCustomerByEmail(email);

            if (customer == null)
            {
                return false;
            }

            customer.IsEmailVerified = 1;
            customer.EmailOtp = null;
            customer.OtpExpiry = null;

            _db.SaveChanges();

            return true;
        }

        public bool SavePendingAuthenticatorSecret(
            int userId,
            string secret)
        {
            var user = GetUserById(userId);

            if (user == null ||
                user.IsTwoFactorEnabled ||
                string.IsNullOrWhiteSpace(secret))
            {
                return false;
            }

            user.PendingAuthenticatorSecret = secret;

            _db.SaveChanges();

            return true;
        }

        public bool EnableTwoFactor(int userId, string secret)
        {
            var user = GetUserById(userId);

            if (user == null ||
                user.IsTwoFactorEnabled ||
                string.IsNullOrWhiteSpace(
                    user.PendingAuthenticatorSecret) ||
                user.PendingAuthenticatorSecret != secret)
            {
                return false;
            }

            user.AuthenticatorSecret = secret;
            user.PendingAuthenticatorSecret = null;
            user.IsTwoFactorEnabled = true;

            _db.SaveChanges();

            return true;
        }

        // =========================================================
        // ADDED SEED METHOD FOR HARDCODED OFFICER
        // =========================================================
        public void SeedOfficer()
        {
            // 1. Define the hardcoded officer details
            var officerEmail = "officer@loanapp.com";
            var officerPassword = "Officer@123";
            var officerFirstName = "Default";
            var officerLastName = "Officer";

            // 2. Check if the officer already exists
            var existingOfficer = GetUserByEmail(officerEmail);

            if (existingOfficer == null)
            {
                // 3. Create the new officer
                var officer = new User
                {
                    FirstName = officerFirstName,
                    LastName = officerLastName,
                    Email = officerEmail,
                    Mobile = "0000000000",
                    Password = BCrypt.Net.BCrypt.HashPassword(officerPassword),
                    RoleId = 2,
                    CustomerId = null,
                    IsTwoFactorEnabled = false
                };

                _db.Users.Add(officer);
                _db.SaveChanges();

                Console.WriteLine($"✅ Hardcoded Officer Created: {officerEmail} / {officerPassword}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Officer already exists: {officerEmail}");
            }
        }
    }
}