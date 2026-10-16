using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Interfaces
{
    public interface IOfficerKycRepository
    {
        List<OfficerKycCustomerVM> GetKycCustomers();

        OfficerKycDetailsVM? GetCustomerKyc(int customerId);

        KycDocument? GetDocumentById(int documentId);

        bool UpdateKycStatus(List<int> documentIds, string status);
    }
}