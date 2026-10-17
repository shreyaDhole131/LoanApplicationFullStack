using Microsoft.AspNetCore.Http;

namespace LoanApp_API.Application.DTO
{
    public class KycDocumentDTO
    {
        public int CustomerId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string? FilePath { get; set; }

        public IFormFile? File { get; set; }
    }
}