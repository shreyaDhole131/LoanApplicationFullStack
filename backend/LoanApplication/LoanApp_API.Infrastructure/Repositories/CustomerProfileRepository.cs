using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CustomerProfileRepository : ICustomerProfileRepository
    {
        private readonly AppDbContext _db;

        public CustomerProfileRepository(AppDbContext db)
        {
            _db = db;
        }
        public CustomerProfileDTO? GetCustomerProfile(int userId)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null || user.CustomerId == null)
                return null;

            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == user.CustomerId);

            if (customer == null)
                return null;

            return new CustomerProfileDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Age = customer.Age,
                Email = customer.Email,
                MobileNo = customer.MobileNo,
                Pan = customer.Pan,
                AadhaarNo = customer.AadhaarNo,
                EmploymentType = customer.EmploymentType,
                MonthlyIncome = customer.MonthlyIncome
            };
        
    }

        public CustomerProfileDTO? UpdateCustomerProfile(
      int userId,
      UpdateCustomerProfileDTO model)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null || user.CustomerId == null)
                return null;

            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == user.CustomerId);

            if (customer == null)
                return null;

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.MobileNo = model.MobileNo;
            customer.EmploymentType = model.EmploymentType;
            customer.MonthlyIncome = model.MonthlyIncome;
            customer.Age = model.Age;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Mobile = model.MobileNo;

            _db.SaveChanges();

            return new CustomerProfileDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Age = customer.Age,
                Email = customer.Email,
                MobileNo = customer.MobileNo,
                Pan = customer.Pan,
                AadhaarNo = customer.AadhaarNo,
                EmploymentType = customer.EmploymentType,
                MonthlyIncome = customer.MonthlyIncome
            };
        }
    }
}