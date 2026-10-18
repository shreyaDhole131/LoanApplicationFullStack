using AutoMapper;
using Hangfire;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Application.Mapper;
using LoanApp_API.Infrastructure.Data;
using LoanApp_API.Infrastructure.Email;
using LoanApp_API.Infrastructure.Repositories;
using LoanApp_API.Infrastructure.Services;
using LoanApp_API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// 1. REPOSITORY AND SERVICE REGISTRATIONS
// ============================================================

builder.Services.AddScoped<IEmiPaymentRepository, EmiPaymentRepository>();
builder.Services.AddScoped<IForeClosureRepository, ForeClosureRepository>();
builder.Services.AddScoped<IPaymentHistoryRepository, PaymentHistoryRepository>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
builder.Services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
builder.Services.AddScoped<IKycRepository, KycRepository>();
builder.Services.AddScoped<ICustomerDashboardRepository, CustomerDashboardRepository>();

builder.Services.AddScoped<IOfficerDashboardRepository, OfficerDashboardRepository>();
builder.Services.AddScoped<IOfficerCustomerProfileRepository, OfficerCustomerProfileRepository>();
builder.Services.AddScoped<IOfficerKycRepository, OfficerKycRepository>();
builder.Services.AddScoped<IOfficerSupportTicketRepository, OfficerSupportTicketRepository>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanAccountService, LoanAccountService>();
builder.Services.AddScoped<IEmiScheduleService, EmiScheduleService>();

builder.Services.AddScoped<ILoanDeals, LoanDealsService>();
builder.Services.AddScoped<ISanctionLetter, SanctionLetterService>();
builder.Services.AddScoped<IDisbursement, DisbursementService>();
builder.Services.AddScoped<IPendingDeals, PendingDealsService>();


// ============================================================
// 2. CIBIL, SCORECARD, ELIGIBILITY AND TOTP SERVICES
// ============================================================

builder.Services.AddScoped<ICibilService, CibilService>();
builder.Services.AddScoped<IScoreCardService, ScoreCardService>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();
builder.Services.AddScoped<ITotpService, TotpService>();


// ============================================================
// 3. CONTROLLERS AND MEMORY CACHE
// ============================================================

builder.Services.AddControllers();

builder.Services.AddMemoryCache();


// ============================================================
// 4. RATE LIMITING
// ============================================================

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;

        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueLimit = 0;
    });
});


// ============================================================
// 5. DATABASE CONNECTION
// ============================================================

var connectionString =
    builder.Configuration.GetConnectionString("dbconn")
    ?? throw new InvalidOperationException(
        "Database connection string 'dbconn' is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// ============================================================
// 6. JWT CONFIGURATION
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"];

var jwtIssuer = builder.Configuration["Jwt:Issuer"];

var jwtAudience = builder.Configuration["Jwt:Audience"];


if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT Key, Issuer, or Audience is missing from configuration.");
}


var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtKey));


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ====================================================
        // JWT TOKEN VALIDATION
        // ====================================================
        // ADD THIS LINE HERE:
        options.MapInboundClaims = false;
        options.TokenHandlers.Clear();
        options.TokenHandlers.Add(new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // -------------------------------
                // Validate Issuer
                // -------------------------------

                ValidateIssuer = true,

                ValidIssuer = jwtIssuer,


                // -------------------------------
                // Validate Audience
                // -------------------------------

                ValidateAudience = true,

                ValidAudience = jwtAudience,


                // -------------------------------
                // Validate Expiration
                // -------------------------------

                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromMinutes(1),


                // -------------------------------
                // Validate Signing Key
                // -------------------------------

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = signingKey,


                // -------------------------------
                // Claim Mapping
                // -------------------------------

                NameClaimType = ClaimTypes.NameIdentifier,

                RoleClaimType = ClaimTypes.Role
            };


        // ====================================================
        // JWT EVENTS
        // ====================================================

        options.Events = new JwtBearerEvents
        {
            // ------------------------------------------------
            // TOKEN RECEIVED
            // ------------------------------------------------

            OnMessageReceived = context =>
            {
                // 👇 ADD THIS BLOCK TO ALLOW CORS PREFLIGHT 👇
                var request = context.HttpContext.Request;
                if (HttpMethods.IsOptions(request.Method))
                {
                    // Skip JWT validation for OPTIONS preflight requests
                    return Task.CompletedTask;
                }
                // 👆 END OF BLOCK 👆

                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("JWT TOKEN RECEIVED");
                Console.WriteLine($"Backend UTC NOW: {DateTime.UtcNow:O}");
                Console.WriteLine("==========================================");

                return Task.CompletedTask;
            },


            // ------------------------------------------------
            // AUTHENTICATION FAILED
            // ------------------------------------------------

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("JWT AUTHENTICATION FAILED");

                Console.WriteLine(
                    $"Backend UTC NOW: {DateTime.UtcNow:O}");

                Console.WriteLine(
                    $"Exception Type: {context.Exception.GetType().Name}");

                Console.WriteLine(
                    $"Exception Message: {context.Exception.Message}");

                Console.WriteLine(
                    $"Full Exception: {context.Exception}");

                Console.WriteLine("==========================================");
                Console.WriteLine();

                return Task.CompletedTask;
            },


            // ------------------------------------------------
            // TOKEN VALIDATED
            // ------------------------------------------------

            OnTokenValidated = context =>
            {
                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("JWT TOKEN VALIDATED SUCCESSFULLY");

                Console.WriteLine(
                    $"Backend UTC NOW: {DateTime.UtcNow:O}");

                if (context.Principal != null)
                {
                    foreach (var claim in context.Principal.Claims)
                    {
                        Console.WriteLine(
                            $"CLAIM: {claim.Type} = {claim.Value}");
                    }
                }

                Console.WriteLine("==========================================");
                Console.WriteLine();

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();


// ============================================================
// 7. SWAGGER CONFIGURATION
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description = "Enter your JWT token."
        });


    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = []
        });
});


// ============================================================
// 8. HANGFIRE BACKGROUND JOBS
// ============================================================

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();


// ============================================================
// 9. AUTOMAPPER
// ============================================================

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey =
        builder.Configuration["AutoMapper:LicenseKey"];

}, typeof(MappingData).Assembly);


// ============================================================
// 10. CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "MyApp",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});





// ============================================================
// 11. BUILD APPLICATION
// ============================================================

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var authRepo = services.GetRequiredService<IAuthRepository>();
        // Cast to AuthRepository to access the SeedOfficer method
        if (authRepo is AuthRepository concreteRepo)
        {
            concreteRepo.SeedOfficer();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding officer: {ex.Message}");
    }
}
// 👆 END OF BLOCK 👆




// ============================================================
// 12. HTTPS AND SWAGGER
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}




// ============================================================
// 13. MIDDLEWARE PIPELINE
// ============================================================

// Keep this commented for now.
// app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("MyApp");

app.UseRateLimiter();

app.UseStaticFiles();


// IMPORTANT:
// Authentication must come BEFORE Authorization.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


// ============================================================
// 14. HANGFIRE DASHBOARD
// ============================================================

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<INotificationService>(
    "generate-emi-notifications",
    service => service.GenerateEmiNotifications(),
    //Cron.Daily
    "*/1 * * * *");


// ============================================================
// 15. RUN APPLICATION
// ============================================================

app.Run();