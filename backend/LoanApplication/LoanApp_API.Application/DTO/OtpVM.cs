using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class OtpVM
    {
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
