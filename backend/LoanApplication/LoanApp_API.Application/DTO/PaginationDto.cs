namespace LoanApp_API.Application.DTO
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int? LastId { get; set; }
    }
}