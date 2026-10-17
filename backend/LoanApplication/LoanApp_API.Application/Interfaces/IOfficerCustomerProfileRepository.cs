using LoanApp_API.Application.DTO;

namespace LoanApp_API.Application.Interfaces
{
    public interface IOfficerCustomerProfileRepository
    {
        List<OfficerCustomerListVM> GetAllCustomers();

        OfficerCustomerProfileVM? GetCustomerProfile(int customerId);
    }
}