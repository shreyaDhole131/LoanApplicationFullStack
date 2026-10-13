using System.Collections.Generic;

namespace LoanApp_API.Application.DTO
{
    public class OfficerKycCustomerVM
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }

    public class OfficerKycDetailsVM
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<OfficerKycDocumentVM> Documents { get; set; } = new();
    }

    public class OfficerKycDocumentVM
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string VerificationStatus { get; set; } = string.Empty;
    }

    public class OfficerKycStatusDTO
    {
        public List<int> DocumentIds { get; set; } = new();
    }
}