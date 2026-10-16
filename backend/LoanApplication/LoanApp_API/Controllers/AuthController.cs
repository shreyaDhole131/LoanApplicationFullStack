
using Google.Apis.Auth;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
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
        private readonly ITotpService _totpService;

        public AuthController(
            IAuthRepository repo,
            IEmailSender emailSender,
            IConfiguration configuration,
            ITotpService totpService)
        {
            _repo = repo;
            _emailSender = emailSender;
            _configuration = configuration;
            _totpService = totpService;
        }

        // 1. CUSTOMER REGISTRATION
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] CustomerRegistrationDTO model)
        {
            try
            {
                var customer = _repo.Register(model);

                if (string.IsNullOrWhiteSpace(customer.EmailOtp))
                {
                    return StatusCode(500, new
                    {
                        message = "Registration OTP was not generated"
                    });
                }

                await _emailSender.SendOtpAsync(
                    customer.Email,
                    customer.EmailOtp);

                return Ok(new
                {
                    message = "Registration successful. OTP sent to your email.",
                    email = customer.Email
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                if (ex.Message == "Email already registered")
                {
                    return BadRequest(new
                    {
                        message = "Email already registered"
                    });
                }

                return StatusCode(500, new
                {
                    message = "Registration failed",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // 2. VERIFY REGISTRATION EMAIL OTP
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public IActionResult VerifyOtp([FromBody] VerifyOtpDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Otp))
            {
                return BadRequest(new
                {
                    message = "Email and OTP are required"
                });
            }

            var customer = _repo.GetCustomerByEmail(model.Email);

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

            if (customer.OtpExpiry == null ||
                DateTime.UtcNow > customer.OtpExpiry)
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

            if (!_repo.VerifyEmailOtp(model.Email, model.Otp))
            {
                return BadRequest(new
                {
                    message = "OTP verification failed"
                });
            }

            return Ok(new
            {
                message = "Email verified successfully. You can now log in."
            });
        }

        // 3. PASSWORD LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new
                {
                    message = "Email and password are required"
                });
            }

            var customer = _repo.GetCustomerByEmail(model.Email);

            if (customer != null && customer.IsEmailVerified != 1)
            {
                return Unauthorized(new
                {
                    message = "Please verify your email before login"
                });
            }

            var user = _repo.Login(model.Email, model.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            if (user.IsTwoFactorEnabled &&
                !string.IsNullOrWhiteSpace(user.AuthenticatorSecret))
            {
                return Ok(new
                {
                    message = "Enter your authenticator OTP",
                    nextStep = "verify-login-otp",
                    challengeToken = CreateStageToken(user, "login")
                });
            }

            return Ok(new
            {
                message = "Set up your authenticator app to continue",
                nextStep = "setup-2fa",
                challengeToken = CreateStageToken(user, "setup")
            });
        }

        // 4. GENERATE AUTHENTICATOR QR CODE
        [Authorize]
        [HttpPost("setup-2fa")]
        public IActionResult SetupTwoFactor()
        {
            if (!TryGetStageUser("setup", out var user))
            {
                return Unauthorized(new
                {
                    message = "A valid 2FA setup token is required"
                });
            }

            if (user.IsTwoFactorEnabled)
            {
                return BadRequest(new
                {
                    message = "Two-factor authentication is already enabled"
                });
            }

            var secret = _totpService.GenerateSecret();

            var saved = _repo.SavePendingAuthenticatorSecret(
                user.UserId, secret);

            if (!saved)
            {
                return StatusCode(500, new
                {
                    message = "Could not save authenticator setup"
                });
            }

            var uri = _totpService.GenerateOtpUri(
                user.Email, secret);

            var imageBytes = GenerateQrImage(uri);

            return Ok(new
            {
                message = "Scan this QR code with your authenticator app",

                secret = secret,

                qrCodeDataUrl =
                    "data:image/png;base64," +
                    Convert.ToBase64String(imageBytes),

                qrCodeHtml =
                    "<img src=\"data:image/png;base64," +
                    Convert.ToBase64String(imageBytes) +
                    "\" width=\"250\" height=\"250\" />",

                qrImageEndpoint = "/api/auth/setup-2fa/qr",

                nextStep = "verify-2fa-setup"
            });
        }

        // 5. RETURN QR CODE AS AN IMAGE
        // Authorize this endpoint with the same setup challenge token.
        [Authorize]
        [HttpGet("setup-2fa/qr")]
        [Produces("image/png")]
        public IActionResult GetTwoFactorQr()
        {
            if (!TryGetStageUser("setup", out var user))
            {
                return Unauthorized(new
                {
                    message = "A valid 2FA setup token is required"
                });
            }

            if (user.IsTwoFactorEnabled)
            {
                return BadRequest(new
                {
                    message = "Two-factor authentication is already enabled"
                });
            }

            if (string.IsNullOrWhiteSpace(
                user.PendingAuthenticatorSecret))
            {
                return BadRequest(new
                {
                    message = "Generate the authenticator QR code first"
                });
            }

            var uri = _totpService.GenerateOtpUri(
                user.Email,
                user.PendingAuthenticatorSecret);

            var imageBytes = GenerateQrImage(uri);

            return File(imageBytes, "image/png");
        }

        // Shared QR image generation method
        private static byte[] GenerateQrImage(string uri)
        {
            using var generator = new QRCodeGenerator();

            using var data = generator.CreateQrCode(
                uri,
                QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(data);

            return qrCode.GetGraphic(10);
        }

        // 6. VERIFY INITIAL AUTHENTICATOR OTP AND ENABLE 2FA
        [Authorize]
        [HttpPost("verify-2fa-setup")]
        public IActionResult VerifyTwoFactorSetup(
            [FromBody] VerifyAuthenticatorOtpDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return BadRequest(new
                {
                    message = "Authenticator OTP is required"
                });
            }

            if (!TryGetStageUser("setup", out var user))
            {
                return Unauthorized(new
                {
                    message = "A valid setup token is required"
                });
            }

            if (string.IsNullOrWhiteSpace(
                user.PendingAuthenticatorSecret))
            {
                return BadRequest(new
                {
                    message = "Generate the authenticator QR code first"
                });
            }

            if (!_totpService.VerifyOtp(
                user.PendingAuthenticatorSecret, model.Otp))
            {
                return BadRequest(new
                {
                    message = "Invalid authenticator OTP"
                });
            }

            var enabled = _repo.EnableTwoFactor(
                user.UserId,
                user.PendingAuthenticatorSecret);

            if (!enabled)
            {
                return StatusCode(500, new
                {
                    message = "Could not enable two-factor authentication"
                });
            }

            var updatedUser = _repo.GetUserById(user.UserId);

            if (updatedUser == null)
            {
                return StatusCode(500, new
                {
                    message = "User could not be loaded"
                });
            }

            return CreateJwtResponse(updatedUser);
        }

        // 7. VERIFY LOGIN AUTHENTICATOR OTP
        [Authorize]
        [HttpPost("verify-login-otp")]
        public IActionResult VerifyLoginOtp(
            [FromBody] VerifyAuthenticatorOtpDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return BadRequest(new
                {
                    message = "Authenticator OTP is required"
                });
            }

            if (!TryGetStageUser("login", out var user))
            {
                return Unauthorized(new
                {
                    message = "A valid login challenge token is required"
                });
            }

            if (!user.IsTwoFactorEnabled ||
                string.IsNullOrWhiteSpace(user.AuthenticatorSecret))
            {
                return Unauthorized(new
                {
                    message = "Authenticator verification is not available"
                });
            }

            if (!_totpService.VerifyOtp(
                user.AuthenticatorSecret, model.Otp))
            {
                return Unauthorized(new
                {
                    message = "Invalid authenticator OTP"
                });
            }

            return CreateJwtResponse(user);
        }

        // 8. GOOGLE LOGIN
        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin(
            [FromBody] GoogleLoginDTO model)
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
                var clientId =
                    _configuration["Authentication:Google:ClientId"];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    return StatusCode(500, new
                    {
                        message = "Google ClientId is not configured"
                    });
                }

                payload = await GoogleJsonWebSignature.ValidateAsync(
                    model.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { clientId }
                    });
            }
            catch
            {
                return Unauthorized(new
                {
                    message = "Invalid Google token"
                });
            }

            if (!payload.EmailVerified ||
                string.IsNullOrWhiteSpace(payload.Email))
            {
                return Unauthorized(new
                {
                    message = "A verified Google email is required"
                });
            }

            var email = payload.Email;
            var user = _repo.GetUserByEmail(email);

            if (user != null)
            {
                if (user.RoleId != 1)
                {
                    return BadRequest(new
                    {
                        message = "This Google account is not registered as a customer"
                    });
                }

                _repo.MarkEmailVerified(email);
            }
            else
            {
                var customer = new Customers
                {
                    FirstName = payload.GivenName ?? "Google",
                    LastName = payload.FamilyName ?? "User",
                    Age = 0,
                    Email = email,
                    Password = Guid.NewGuid().ToString(),
                    MobileNo = string.Empty,
                    Pan = string.Empty,
                    AadhaarNo = string.Empty,
                    EmploymentType = string.Empty,
                    MonthlyIncome = string.Empty,
                    IsEmailVerified = 1,
                    EmailOtp = null,
                    OtpExpiry = null
                };

                _repo.CreateCustomer(customer);
                user = _repo.GetUserByEmail(email);
            }

            if (user == null)
            {
                return StatusCode(500, new
                {
                    message = "Google user creation failed"
                });
            }

            if (user.IsTwoFactorEnabled &&
                !string.IsNullOrWhiteSpace(user.AuthenticatorSecret))
            {
                return Ok(new
                {
                    message = "Enter your authenticator OTP",
                    nextStep = "verify-login-otp",
                    challengeToken = CreateStageToken(user, "login")
                });
            }

            return Ok(new
            {
                message = "Set up your authenticator app to continue",
                nextStep = "setup-2fa",
                challengeToken = CreateStageToken(user, "setup")
            });
        }

        // 9. SHORT-LIVED TOKEN FOR AN AUTHENTICATION STAGE
        private string CreateStageToken(User user, string stage)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.RoleId.ToString()),

                new Claim("auth_stage", stage)
            };

            return CreateToken(
                claims,
                DateTime.UtcNow.AddMinutes(5));
        }

        private bool TryGetStageUser(
            string requiredStage,
            out User user)
        {
            user = null!;

            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var stage = User.FindFirst("auth_stage")?.Value;

            if (stage != requiredStage)
            {
                return false;
            }

            var userIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
            {
                return false;
            }

            var foundUser = _repo.GetUserById(userId);

            if (foundUser == null)
            {
                return false;
            }

            user = foundUser;
            return true;
        }

        // 10. FINAL JWT RESPONSE
        private IActionResult CreateJwtResponse(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.RoleId.ToString())
            };

            var token = CreateToken(
                claims,
                DateTime.UtcNow.AddHours(2));

            return Ok(new
            {
                message = "Login successful",
                token = token,
                userId = user.UserId,
                roleId = user.RoleId,
                customerId = user.CustomerId
            });
        }

        // 11. CREATE JWT
        private string CreateToken(
            IEnumerable<Claim> claims,
            DateTime expiry)
        {
            var keyValue = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyValue))
            {
                throw new InvalidOperationException(
                    "JWT signing key is not configured");
            }

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JWT issuer or audience is not configured");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
