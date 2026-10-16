using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Interfaces
{
    public interface IKycRepository
    {
        KycDocument UploadKyc(KycDocumentDTO model);

        int? GetCustomerIdByUserId(int userId);

        List<KycDocument> GetMyKycDocuments(int customerId);

        KycDocument? GetKycById(int documentId, int customerId);

        bool DeleteKyc(int documentId, int customerId);
    }
}