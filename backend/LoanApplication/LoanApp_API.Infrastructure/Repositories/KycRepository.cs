using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class KycRepository : IKycRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public KycRepository(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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
            var kyc = _mapper.Map<KycDocument>(model);

            kyc.VerificationStatus = "Pending";

            _db.KycDocuments.Add(kyc);

            _db.SaveChanges();

            return kyc;
        }

        public KycDocument? GetKycById(
            int documentId,
            int customerId)
        {
            return _db.KycDocuments
                .FirstOrDefault(x =>
                    x.DocumentId == documentId &&
                    x.CustomerId == customerId);
        }

        public bool DeleteKyc(
            int documentId,
            int customerId)
        {
            var kyc = _db.KycDocuments
                .FirstOrDefault(x =>
                    x.DocumentId == documentId &&
                    x.CustomerId == customerId);

            if (kyc == null)
                return false;

            _db.KycDocuments.Remove(kyc);

            _db.SaveChanges();

            return true;
        }
    }
}