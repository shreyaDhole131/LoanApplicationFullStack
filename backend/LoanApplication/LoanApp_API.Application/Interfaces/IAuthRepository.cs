using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Interfaces
{
    public interface IAuthRepository
    {
        User? GetUserByEmail(string email);

        Customers? GetCustomerByEmail(string email);

        User CreateUser(User user);

        User CreateOfficer(User user);

        Customers CreateCustomer(Customers customer);

        Customers Register(CustomerRegistrationDTO model);

        User? Login(string email, string password);

        bool VerifyEmailOtp(string email, string otp);

        bool MarkEmailVerified(string email);
    }
}