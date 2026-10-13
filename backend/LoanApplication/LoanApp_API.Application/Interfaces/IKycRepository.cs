using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IKycRepository
    {
        KycDocument UploadKyc(KycDocumentDTO model);
        int? GetCustomerIdByUserId(int userId);
        List<KycDocument> GetMyKycDocuments(int customerId);
    }
}
