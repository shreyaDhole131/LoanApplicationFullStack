namespace LoanApp_API.Application.DTO.Eligibility
{
    public class EligibilityRejectDto
    {
        public string RejectionReason { get; set; } = string.Empty;

        public string? OtherReason { get; set; }
    }
}