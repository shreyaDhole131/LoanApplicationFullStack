using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerKycRepository : IOfficerKycRepository
    {
        private readonly AppDbContext _db;

        public OfficerKycRepository(AppDbContext db)
        {
            _db = db;
        }

        // 1. Get all customers who have KYC documents
        public List<OfficerKycCustomerVM> GetKycCustomers()
        {
            return _db.KycDocuments
                .Join(
                    _db.Customers,
                    document => document.CustomerId,
                    customer => customer.CustomerId,
                    (document, customer) => new OfficerKycCustomerVM
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.FirstName + " " + customer.LastName,
                        Email = customer.Email,
                        Mobile = customer.MobileNo
                    }
                )
                .GroupBy(x => x.CustomerId)
                .Select(x => x.First())
                .ToList();
        }

        // 2. Get KYC details of one particular customer
        public OfficerKycDetailsVM? GetCustomerKyc(int customerId)
        {
            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
                return null;

            var documents = _db.KycDocuments
                .Where(x => x.CustomerId == customerId)
                .Select(x => new OfficerKycDocumentVM
                {
                    DocumentId = x.DocumentId,
                    DocumentType = x.DocumentType,
                    FilePath = x.FilePath,
                    VerificationStatus = x.VerificationStatus
                })
                .ToList();

            var result = new OfficerKycDetailsVM
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.FirstName + " " + customer.LastName,
                Email = customer.Email,
                Documents = documents
            };

            return result;
        }

        // 3. Approve or Reject selected KYC documents
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