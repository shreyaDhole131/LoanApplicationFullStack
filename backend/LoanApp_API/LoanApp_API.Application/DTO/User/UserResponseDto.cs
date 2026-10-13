namespace LoanApp_API.Application.DTO.User
{
    public class UserResponseDto
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public int? CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;
    }
}