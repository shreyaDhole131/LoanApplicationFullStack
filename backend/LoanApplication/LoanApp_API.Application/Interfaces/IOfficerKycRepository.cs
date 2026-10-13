using LoanApp_API.Application.DTO;

namespace LoanApp_API.Application.Interfaces
{
    public interface IOfficerKycRepository
    {
        List<OfficerKycCustomerVM> GetKycCustomers();

        OfficerKycDetailsVM? GetCustomerKyc(int customerId);

        bool UpdateKycStatus(List<int> documentIds, string status);
    }
}