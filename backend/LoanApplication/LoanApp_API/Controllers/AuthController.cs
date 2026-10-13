using Google.Apis.Auth;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repo,IEmailSender emailSender,IConfiguration configuration)
        {
            _repo = repo;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CustomerRegistrationDTO model)
        {
            var existingUser =_repo.GetUserByEmail(model.Email);

            var existingCustomer =_repo.GetCustomerByEmail(model.Email);

            if (existingUser != null || existingCustomer != null)
            {
                if (existingCustomer != null &&
                    existingCustomer.IsEmailVerified == 0)
                {
                    var newOtp =
                        Random.Shared.Next(100000, 1000000)
                            .ToString();

                    existingCustomer.EmailOtp = newOtp;
                    existingCustomer.OtpExpiry =
                        DateTime.UtcNow.AddMinutes(5);

                    await _emailSender.SendOtpAsync(
                        existingCustomer.Email,
                        newOtp
                    );

                    return Ok(new
                    {
                        message =
                            "Email already registered but not verified. New OTP sent.",
                        email = existingCustomer.Email
                    });
                }

                return BadRequest(new
                {
                    message = "Email already registered"
                });
            }

            var otp =
                Random.Shared.Next(100000, 1000000)
                    .ToString();

            var customer = new Customers
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                Password = model.Password,
                MobileNo = model.MobileNo,
                Pan = model.Pan,
                AadhaarNo = model.AadhaarNo,
                EmploymentType = model.EmploymentType,
                MonthlyIncome = model.MonthlyIncome,

                IsEmailVerified = 0,

                EmailOtp = otp,

                OtpExpiry =
                    DateTime.UtcNow.AddMinutes(5)
            };

            _repo.CreateCustomer(customer);

            await _emailSender.SendOtpAsync(
                customer.Email,
                otp
            );

            return Ok(new
            {
                message =
                    "Registration successful. OTP sent to your email.",
                email = customer.Email
            });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp(
            VerifyOtpDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required"
                });
            }

            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return BadRequest(new
                {
                    message = "OTP is required"
                });
            }

            var customer =
                _repo.GetCustomerByEmail(model.Email);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            if (customer.IsEmailVerified == 1)
            {
                return BadRequest(new
                {
                    message = "Email already verified"
                });
            }

            if (customer.OtpExpiry == null ||DateTime.UtcNow > customer.OtpExpiry)
            {
                return BadRequest(new
                {
                    message = "OTP expired"
                });
            }

            if (customer.EmailOtp != model.Otp)
            {
                return BadRequest(new
                {
                    message = "Invalid OTP"
                });
            }

            bool verified =
                _repo.VerifyEmailOtp(model.Email,model.Otp);

            if (!verified)
            {
                return BadRequest(new
                {
                    message = "OTP verification failed"
                });
            }

            return Ok(new
            {
                message = "Email verified successfully"
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO model)
        {
            var customer =
                _repo.GetCustomerByEmail(model.Email);

            if (customer != null &&
                customer.IsEmailVerified == 0)
            {
                return Unauthorized(new
                {
                    message =
                        "Please verify your email before login"
                });
            }

            var user =
                _repo.Login(
                    model.Email,
                    model.Password
                );

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            return CreateJwtResponse(user);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin(
            GoogleLoginDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.IdToken))
            {
                return BadRequest(new
                {
                    message = "Google ID token is required"
                });
            }

            GoogleJsonWebSignature.Payload payload;

            try
            {
                var googleClientId =
                    _configuration[
                        "Authentication:Google:ClientId"
                    ];

                if (string.IsNullOrWhiteSpace(googleClientId))
                {
                    return StatusCode(500, new
                    {
                        message =
                            "Google ClientId is not configured"
                    });
                }

                payload =
                    await GoogleJsonWebSignature.ValidateAsync(
                        model.IdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience =
                                new[]
                                {
                                    googleClientId
                                }
                        }
                    );
            }
            catch
            {
                return Unauthorized(new
                {
                    message = "Invalid Google token"
                });
            }

            if (!payload.EmailVerified)
            {
                return Unauthorized(new
                {
                    message =
                        "Google email is not verified"
                });
            }

            var email = payload.Email;

            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    message =
                        "Google account email not available"
                });
            }

            var existingUser =
                _repo.GetUserByEmail(email);

            if (existingUser != null)
            {
                if (existingUser.RoleId != 1)
                {
                    return BadRequest(new
                    {
                        message =
                            "This Google account is not registered as a customer"
                    });
                }

                _repo.MarkEmailVerified(email);

                return CreateJwtResponse(existingUser);
            }

            var firstName =
                payload.GivenName ?? "Google";

            var lastName =
                payload.FamilyName ?? "User";

            var customer = new Customers
            {
                FirstName = firstName,
                LastName = lastName,
                Age = 0,
                Email = email,

                Password =
                    Guid.NewGuid().ToString(),

                MobileNo = string.Empty,

                Pan = string.Empty,

                AadhaarNo = string.Empty,

                EmploymentType =
                    string.Empty,

                MonthlyIncome =
                    string.Empty,

                IsEmailVerified = 1,

                EmailOtp = null,

                OtpExpiry = null
            };

            _repo.CreateCustomer(customer);

            var user =
                _repo.GetUserByEmail(email);

            if (user == null)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message =
                            "Google user creation failed"
                    }
                );
            }

            return CreateJwtResponse(user);
        }

        [HttpPost("create-officer")]
        public IActionResult CreateOfficer()
        {
            var officer =
                new User
                {
                    FirstName = "Test",
                    LastName = "Officer",
                    Email = "officer@test.com",
                    Mobile = "9999999999",
                    Password = "Officer@123",
                    RoleId = 2,
                    CustomerId = null
                };

            _repo.CreateOfficer(officer);

            return Ok(new
            {
                message = "Officer created successfully",
                email = officer.Email
            });
        }

        private IActionResult CreateJwtResponse(
            User user)
        {
            var claims =
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.UserId.ToString()
                    ),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        user.RoleId.ToString()
                    )
                };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]!
                    )
                );

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

            var token =
                new JwtSecurityToken(
                    issuer:
                        _configuration["Jwt:Issuer"],

                    audience:
                        _configuration["Jwt:Audience"],

                    claims: claims,

                    expires:
                        DateTime.UtcNow.AddHours(2),

                    signingCredentials:
                        credentials
                );

            var jwtToken =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            return Ok(new
            {
                message = "Login successful",

                token = jwtToken,

                userId = user.UserId,

                roleId = user.RoleId,

                customerId = user.CustomerId
            });
        }
    }
}