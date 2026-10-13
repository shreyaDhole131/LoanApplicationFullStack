namespace LoanApp_API.Application.DTO.User
{
    public class CreateUserDto
    {
        public int RoleId { get; set; }

        public int? CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}