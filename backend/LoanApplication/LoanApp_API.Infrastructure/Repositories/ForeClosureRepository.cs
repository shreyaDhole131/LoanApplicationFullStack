
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

        public ForeClosureRepository(AppDbContext db, IMapper mapper, IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _razorpayKeyId = config["Razorpay:KeyId"] ?? "";
            _razorpayKeySecret = config["Razorpay:KeySecret"] ?? "";
        }

        // Customer side

        // Active loans of this customer
        public async Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId)
        {
            var loans = await _db.LoanAccounts
                .Where(x => x.CustomerId == customerId &&
                            x.LoanStatus == "Active")
                .ToListAsync();

            return _mapper.Map<List<CustomerLoanDto>>(loans);
        }

        // Foreclosure summary
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

        // Submit foreclosure request
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
                    ? "Nil" : dto.Reason,
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
                // Outstanding + interest + 2% charges
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

        // Customer foreclosure requests
        public async Task<List<CustomerForeClosureDto>> GetMyRequests(
            int customerId)
        {
            return await _db.ForeClosureRequests
                .Where(x => x.LoanAccount!.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedDate)
                .ProjectTo<CustomerForeClosureDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // Officer dashboard
        public async Task<List<OfficerForeClosureDto>> GetAllRequests()
        {
            return await OfficerQuery()
                .OrderByDescending(x => x.RequestDate)
                .ToListAsync();
        }

        // Officer request details
        public async Task<OfficerForeClosureDto?> GetRequestById(int id)
        {
            return await OfficerQuery()
                .FirstOrDefaultAsync(x => x.RequestId == id);
        }

        // Next pending EMI
        private async Task<EmiSchedule?> GetNextPendingEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId &&
                            x.PaymentStatus == "Pending")
                .OrderBy(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

        // Last paid EMI
        private async Task<EmiSchedule?> GetLastPaidEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId &&
                            x.PaymentStatus == "Paid")
                .OrderByDescending(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

        // Common query for officer list and details
        private IQueryable<OfficerForeClosureDto> OfficerQuery()
        {
            return _db.ForeClosureRequests
                .ProjectTo<OfficerForeClosureDto>(
                    _mapper.ConfigurationProvider);
        }


        // Customer payment (Razorpay)

        // Create Razorpay order for an approved request
        public async Task<(bool Success, string Message, ForeClosureOrderDto? Data)> CreatePaymentOrder(
            int requestId, int customerId)
        {
            var request = await GetOwnRequest(requestId, customerId);

            if (request == null)
                return (false, "Foreclosure request not found", null);

            if (request.IsPaid || request.Status == "Paid")
                return (false, "This request is already paid", null);

            if (request.Status != "Approved")
                return (false, "Only approved requests can be paid", null);

            if (_razorpayKeyId == "" || _razorpayKeySecret == "")
                return (false, "Razorpay keys are not configured", null);

            decimal amount = GetPayableAmount(request);

            if (amount <= 0)
                return (false, "Invalid payment amount", null);

            // Amount always comes from the database, never from the frontend
            long amountInPaise = (long)Math.Round(amount * 100);

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId, _razorpayKeySecret);

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
                    KeyId = _razorpayKeyId   // key id is public, secret is never sent
                });
            }
            catch (Exception ex)
            {
                return (false, "Could not create Razorpay order: " + ex.Message, null);
            }
        }

        // Verify payment, mark request as Paid and add entry in LoanPayment
        public async Task<(bool Success, string Message)> VerifyPayment(
            VerifyForeClosurePaymentDto dto, int customerId)
        {
            var request = await GetOwnRequest(dto.RequestId, customerId);

            if (request == null)
                return (false, "Foreclosure request not found");

            if (request.IsPaid || request.Status == "Paid")
                return (true, "Already paid");

            if (request.RazorpayOrderId != dto.RazorpayOrderId)
                return (false, "Order does not match this request");

            // Signature = HMAC_SHA256(orderId + "|" + paymentId, keySecret)
            string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_razorpayKeySecret));

            string expected = Convert.ToHexString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLower();

            if (expected != dto.RazorpaySignature)
                return (false, "Payment verification failed");

            // Payment mode (UPI / CARD / NETBANKING) from Razorpay
            string paymentMode = "Razorpay";

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId, _razorpayKeySecret);

                var razorpayPayment = client.Payment.Fetch(dto.RazorpayPaymentId);
                paymentMode = Convert.ToString(razorpayPayment["method"])?.ToUpper()
                              ?? "Razorpay";
            }
            catch
            {
                // keep default payment mode
            }

            decimal amount = GetPayableAmount(request);

            // 1. Update foreclosure request
            request.IsPaid = true;
            request.Status = "Paid";
            request.RazorpayPaymentId = dto.RazorpayPaymentId;
            request.PaidDate = DateTime.Now;

            // 2. Add entry in LoanPayment table
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

            // Both changes are saved together
            await _db.SaveChangesAsync();

            return (true, "Payment successful");
        }

        // Request that belongs to this customer
        private async Task<ForeClosureRequest?> GetOwnRequest(int requestId, int customerId)
        {
            var request = await _db.ForeClosureRequests.FindAsync(requestId);

            if (request == null)
                return null;

            await _db.Entry(request).Reference(x => x.LoanAccount).LoadAsync();

            if (request.LoanAccount == null ||
                request.LoanAccount.CustomerId != customerId)
                return null;

            return request;
        }

        // Full = ForeClosureAmount, Partial = PartialAmount
        private static decimal GetPayableAmount(ForeClosureRequest request)
        {
            return request.ForeClosureType == "Partial"
                ? request.PartialAmount
                : request.ForeClosureAmount;
        }

        // Approve or reject
        public async Task<(bool Success, string Message)> ChangeStatus(
            int id, string status)
        {
            if (status != "Approved" && status != "Rejected")
                return (false, "Status must be Approved or Rejected");

            var request = await _db.ForeClosureRequests.FindAsync(id);

            if (request == null)
                return (false, "Request not found");

            if (request.Status != "Pending")
                return (false,
                    $"Only pending requests can be changed. Current status: {request.Status}");

            request.Status = status;

            if (status == "Approved")
                request.ApprovedDate = DateTime.Now;

            await _db.SaveChangesAsync();

            return (true, $"Foreclosure request {status.ToLower()}");
        }
    }
}
