using AutoMapper;
using Hangfire;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Application.Mapper;
using LoanApp_API.Infrastructure.Data;
using LoanApp_API.Infrastructure.Email;
using LoanApp_API.Infrastructure.Repositories;
using LoanApp_API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Repository and service registrations
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

// CIBIL, ScoreCard and Eligibility services
builder.Services.AddScoped<ICibilService, CibilService>();
builder.Services.AddScoped<IScoreCardService, ScoreCardService>();
builder.Services.AddScoped<IEligibilityService, EligibilityService>();

// Controllers and memory cache
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("dbconn")
    ));

// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {token}"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Hangfire background jobs
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("dbconn")
    ));

builder.Services.AddHangfireServer();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
}, typeof(MappingData).Assembly);


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});
// Build application
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Uncomment if you want to enable the exception middleware
// app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("MyApp");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

app.Run();