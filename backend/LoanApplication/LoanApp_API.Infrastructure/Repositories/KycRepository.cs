using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class KycRepository : IKycRepository
    {
        private readonly AppDbContext _db;

        public KycRepository(AppDbContext db)
        {
            _db = db;
        }

        public int? GetCustomerIdByUserId(int userId)
        {
            var user = _db.Users
                  .FirstOrDefault(x => x.UserId == userId);

            return user?.CustomerId;
        }

        public List<KycDocument> GetMyKycDocuments(int customerId)
        {
            return _db.KycDocuments
        .Where(x => x.CustomerId == customerId)
        .ToList();
        }

        public KycDocument UploadKyc(KycDocumentDTO model)
        {
            var kyc = new KycDocument
            {
                CustomerId = model.CustomerId,
                DocumentType = model.DocumentType,
                FilePath = model.FilePath,
                VerificationStatus = "Pending"
            };

            _db.KycDocuments.Add(kyc);
            _db.SaveChanges();

            return kyc;
        }
    }
}