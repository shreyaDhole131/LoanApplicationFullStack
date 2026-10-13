using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
