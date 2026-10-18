
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class ForeClosureRepository : IForeClosureRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly string _razorpayKeyId;
        private readonly string _razorpayKeySecret;

        public ForeClosureRepository(
            AppDbContext db,
            IMapper mapper,
            IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _razorpayKeyId = config["Razorpay:KeyId"] ?? "";
            _razorpayKeySecret = config["Razorpay:KeySecret"] ?? "";
        }

        // Customer side

        public async Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId)
        {
            var loans = await _db.LoanAccounts
                .Where(x => x.CustomerId == customerId &&
                            x.LoanStatus == "Active")
                .ToListAsync();

            return _mapper.Map<List<CustomerLoanDto>>(loans);
        }

        public async Task<ForeClosureSummaryDto?> GetSummary(int loanAccountId)
        {
            var loan = await _db.LoanAccounts.FindAsync(loanAccountId);

            if (loan == null)
                return null;

            var nextEmi = await GetNextPendingEmi(loanAccountId);

            return new ForeClosureSummaryDto
            {
                LoanAccountId = loan.LoanAccountId,
                LoanAccountNo = loan.LoanAccountNo,
                OutstandingPrincipal = nextEmi?.OpeningBalance ?? 0,
                NextEmiDate = nextEmi?.DueDate
            };
        }

        public async Task<(bool Success, string Message)> SubmitRequest(
            ForeClosureDto dto)
        {
            var loan = await _db.LoanAccounts.FindAsync(dto.LoanAccountId);

            if (loan == null || loan.CustomerId != dto.CustomerId)
                return (false, "Loan not found");

            if (loan.LoanStatus != "Active")
                return (false, "Loan already closed");

            if (dto.ForeClosureType != "Full" &&
                dto.ForeClosureType != "Partial")
                return (false, "Select foreclosure type (Full or Partial)");

            if (dto.ExpectedClosureDate.Date < DateTime.Today)
                return (false, "Expected closure date cannot be a past date");

            bool alreadyExists = await _db.ForeClosureRequests
                .AnyAsync(x => x.LoanAccountId == dto.LoanAccountId &&
                               x.Status != "Rejected");

            if (alreadyExists)
                return (false,
                    "Foreclosure request already exists for this loan account");

            var lastPaidEmi = await GetLastPaidEmi(dto.LoanAccountId);

            if (lastPaidEmi == null)
                return (false, "At least one EMI must be paid");

            var nextEmi = await GetNextPendingEmi(dto.LoanAccountId);

            if (nextEmi == null)
                return (false, "Loan is already completed");

            var request = new ForeClosureRequest
            {
                LoanAccountId = dto.LoanAccountId,
                ForeClosureType = dto.ForeClosureType,
                ExpectedClosureDate = dto.ExpectedClosureDate,
                Reason = string.IsNullOrWhiteSpace(dto.Reason)
                    ? "Nil"
                    : dto.Reason,
                RequestedDate = DateTime.Now,
                Status = "Pending"
            };

            if (dto.ForeClosureType == "Partial")
            {
                if (dto.PartialAmount <= 0)
                    return (false, "Enter a valid partial amount");

                if (dto.PartialAmount >= nextEmi.OpeningBalance)
                    return (false,
                        "Partial amount must be less than outstanding principal");

                request.PartialAmount = dto.PartialAmount;
            }
            else
            {
                decimal outstanding = nextEmi.OpeningBalance;

                DateTime lastPaidDate =
                    lastPaidEmi.PaidDate ?? lastPaidEmi.DueDate;

                int days = Math.Max(
                    0,
                    (dto.ExpectedClosureDate.Date - lastPaidDate.Date).Days);

                decimal interest =
                    outstanding * loan.InterestRate * days / 36500;

                decimal charges = outstanding * 0.02m;

                request.ForeClosureAmount =
                    Math.Round(outstanding + interest + charges, 2);
            }

            _db.ForeClosureRequests.Add(request);
            await _db.SaveChangesAsync();

            return (true, "Foreclosure request submitted");
        }

        public async Task<List<CustomerForeClosureDto>> GetMyRequests(
            int customerId)
        {
            return await _db.ForeClosureRequests
                .Where(x => x.LoanAccount != null &&
                            x.LoanAccount.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedDate)
                .ProjectTo<CustomerForeClosureDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<OfficerForeClosureDto>> GetAllRequests()
        {
            return await OfficerQuery().ToListAsync();
        }

        public async Task<OfficerForeClosureDto?> GetRequestById(int id)
        {
            return await OfficerQuery()
                .FirstOrDefaultAsync(x => x.RequestId == id);
        }

        private async Task<EmiSchedule?> GetNextPendingEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId &&
                            x.PaymentStatus == "Pending")
                .OrderBy(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

        private async Task<EmiSchedule?> GetLastPaidEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId &&
                            x.PaymentStatus == "Paid")
                .OrderByDescending(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

       
        private IQueryable<OfficerForeClosureDto> OfficerQuery()
        {
            return _db.ForeClosureRequests
                .OrderByDescending(x => x.RequestedDate)
                .ProjectTo<OfficerForeClosureDto>(
                    _mapper.ConfigurationProvider);
        }

        
        public async Task<(
            bool Success,
            string Message,
            ForeClosureOrderDto? Data)> CreatePaymentOrder(
                int requestId,
                int customerId)
        {
            var request = await GetOwnRequest(requestId, customerId);

            if (request == null)
                return (false, "Foreclosure request not found", null);

            if (request.IsPaid || request.Status == "Paid")
                return (false, "This request is already paid", null);

            if (request.Status != "Approved")
                return (false, "Only approved requests can be paid", null);

            if (string.IsNullOrWhiteSpace(_razorpayKeyId) ||
                string.IsNullOrWhiteSpace(_razorpayKeySecret))
            {
                return (false, "Razorpay keys are not configured", null);
            }

            decimal amount = GetPayableAmount(request);

            if (amount <= 0)
                return (false, "Invalid payment amount", null);

            long amountInPaise = (long)Math.Round(amount * 100);

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId,
                    _razorpayKeySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", "FC_" + requestId }
                };

                var order = client.Order.Create(options);
                string orderId = Convert.ToString(order["id"]) ?? "";

                request.RazorpayOrderId = orderId;
                await _db.SaveChangesAsync();

                return (true, "Order created", new ForeClosureOrderDto
                {
                    RequestId = requestId,
                    OrderId = orderId,
                    AmountInPaise = amountInPaise,
                    Currency = "INR",
                    KeyId = _razorpayKeyId
                });
            }
            catch (Exception ex)
            {
                return (
                    false,
                    "Could not create Razorpay order: " + ex.Message,
                    null);
            }
        }

        public async Task<(bool Success, string Message)> VerifyPayment(
            VerifyForeClosurePaymentDto dto,
            int customerId)
        {
            var request = await GetOwnRequest(dto.RequestId, customerId);

            if (request == null)
                return (false, "Foreclosure request not found");

            if (request.IsPaid || request.Status == "Paid")
                return (true, "Already paid");

            if (request.RazorpayOrderId != dto.RazorpayOrderId)
                return (false, "Order does not match this request");

            string payload =
                dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_razorpayKeySecret));

            string expected = Convert.ToHexString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)))
                .ToLowerInvariant();

            if (!string.Equals(
                    expected,
                    dto.RazorpaySignature,
                    StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Payment verification failed");
            }

            string paymentMode = "Razorpay";

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId,
                    _razorpayKeySecret);

                var razorpayPayment =
                    client.Payment.Fetch(dto.RazorpayPaymentId);

                paymentMode =
                    Convert.ToString(razorpayPayment["method"])?.ToUpper()
                    ?? "Razorpay";
            }
            catch
            {
            }

            decimal amount = GetPayableAmount(request);

            request.IsPaid = true;
            request.Status = "Paid";
            request.RazorpayPaymentId = dto.RazorpayPaymentId;
            request.PaidDate = DateTime.Now;

            _db.Set<LoanPayment>().Add(new LoanPayment
            {
                LoanAccountId = request.LoanAccountId,
                PaymentName = request.ForeClosureType + " Foreclosure",
                PaymentDate = DateTime.Now,
                PaidAmount = amount,
                PaymentMode = paymentMode,
                TransactionReference = dto.RazorpayPaymentId,
                PrincipalPaid = amount,
                InterestPaid = 0,
                PenaltyPaid = 0,
                PaymentStatus = "Success"
            });

            await _db.SaveChangesAsync();

            return (true, "Payment successful");
        }

        private async Task<ForeClosureRequest?> GetOwnRequest(
            int requestId,
            int customerId)
        {
            var request =
                await _db.ForeClosureRequests.FindAsync(requestId);

            if (request == null)
                return null;

            await _db.Entry(request)
                .Reference(x => x.LoanAccount)
                .LoadAsync();

            if (request.LoanAccount == null ||
                request.LoanAccount.CustomerId != customerId)
            {
                return null;
            }

            return request;
        }

        private static decimal GetPayableAmount(
            ForeClosureRequest request)
        {
            return request.ForeClosureType == "Partial"
                ? request.PartialAmount
                : request.ForeClosureAmount;
        }

        public async Task<(bool Success, string Message)> ChangeStatus(
            int id,
            string status)
        {
            if (status != "Approved" && status != "Rejected")
                return (false, "Status must be Approved or Rejected");

            var request =
                await _db.ForeClosureRequests.FindAsync(id);

            if (request == null)
                return (false, "Request not found");

            if (request.Status != "Pending")
            {
                return (
                    false,
                    $"Only pending requests can be changed. Current status: {request.Status}");
            }

            request.Status = status;

            if (status == "Approved")
                request.ApprovedDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return (true, $"Foreclosure request {status.ToLower()}");
        }
    }
}
