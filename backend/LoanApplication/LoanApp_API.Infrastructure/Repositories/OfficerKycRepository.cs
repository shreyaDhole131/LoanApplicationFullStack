using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerKycRepository : IOfficerKycRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public OfficerKycRepository(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<OfficerKycCustomerVM> GetKycCustomers()
        {
            var customers = _db.Customers
                .Where(c => _db.KycDocuments
                    .Any(k => k.CustomerId == c.CustomerId))
                .ToList();

            return _mapper.Map<List<OfficerKycCustomerVM>>(customers);
        }

        public OfficerKycDetailsVM? GetCustomerKyc(int customerId)
        {
            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
                return null;

            var documents = _db.KycDocuments
                .Where(x => x.CustomerId == customerId)
                .ToList();

            var result = new OfficerKycDetailsVM
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.FirstName + " " + customer.LastName,
                Email = customer.Email,
                Documents = _mapper.Map<List<OfficerKycDocumentVM>>(documents)
            };

            return result;
        }

        public KycDocument? GetDocumentById(int documentId)
        {
            return _db.KycDocuments
                .FirstOrDefault(x => x.DocumentId == documentId);
        }

        public bool UpdateKycStatus(List<int> documentIds, string status)
        {
            var documents = _db.KycDocuments
                .Where(x => documentIds.Contains(x.DocumentId))
                .ToList();

            if (documents.Count == 0)
                return false;

            foreach (var document in documents)
            {
                document.VerificationStatus = status;
            }

            _db.SaveChanges();

            return true;
        }
    }
}