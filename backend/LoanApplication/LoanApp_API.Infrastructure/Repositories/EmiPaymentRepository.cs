
using System.Security.Cryptography;
using System.Text;
using LoanApp_API.Application.DTO.EmiPayment;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class EmiPaymentRepository : IEmiPaymentRepository
    {
        private readonly AppDbContext _db;
        private readonly string _razorpayKeyId;
        private readonly string _razorpayKeySecret;

        public EmiPaymentRepository(
            AppDbContext db, IConfiguration config)
        {
            _db = db;
            _razorpayKeyId = config["Razorpay:KeyId"] ?? "";
            _razorpayKeySecret = config["Razorpay:KeySecret"] ?? "";
        }

        // Get EMI payment details
        public async Task<EmiPaymentDto?> GetPaymentDetails(
            int emiScheduleId, int customerId)
        {
            var emi = await _db.EmiSchedules
                .Include(x => x.LoanAccount)
                .FirstOrDefaultAsync(x =>
                    x.EmiScheduleId == emiScheduleId &&
                    x.LoanAccount!.CustomerId == customerId);

            if (emi == null || emi.PaymentStatus != "Pending")
                return null;

            return new EmiPaymentDto
            {
                EmiScheduleId = emi.EmiScheduleId,
                InstallmentNo = emi.InstallmentNo,
                DueDate = emi.DueDate,
                Emi = emi.Emi,
                Penalty = 0,
                BounceCharges = 0,
                GST = 0,
                TotalAmount = emi.Emi
            };
        }

        // Create Razorpay order
        public async Task<(
            bool Success, string Message, EmiPaymentOrderDto? Data)>
            CreatePaymentOrder(int emiScheduleId, int customerId)
        {
            var emi = await _db.EmiSchedules
                .Include(x => x.LoanAccount)
                .FirstOrDefaultAsync(x =>
                    x.EmiScheduleId == emiScheduleId &&
                    x.LoanAccount!.CustomerId == customerId);

            if (emi == null)
                return (false, "EMI not found", null);

            if (emi.PaymentStatus != "Pending")
                return (false, "This EMI is not pending", null);

            // Only allow the earliest unpaid installment
            var previousPending = await _db.EmiSchedules
                .AnyAsync(x =>
                    x.LoanAccountId == emi.LoanAccountId &&
                    x.InstallmentNo < emi.InstallmentNo &&
                    x.PaymentStatus != "Paid");

            if (previousPending)
                return (false, "Pay the previous pending EMI first", null);

            if (string.IsNullOrWhiteSpace(_razorpayKeyId) ||
                string.IsNullOrWhiteSpace(_razorpayKeySecret))
                return (false, "Razorpay keys are not configured", null);

            decimal amount = emi.Emi;
            long amountInPaise = (long)Math.Round(amount * 100);

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId, _razorpayKeySecret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", "EMI_" + emi.EmiScheduleId }
                };

                var order = client.Order.Create(options);
                string orderId = Convert.ToString(order["id"]) ?? "";

                if (string.IsNullOrWhiteSpace(orderId))
                    return (false, "Could not create Razorpay order", null);

                // Store the order as a pending transaction
                _db.LoanPayment.Add(new LoanPayment
                {
                    LoanAccountId = emi.LoanAccountId,
                    EmiScheduleId = emi.EmiScheduleId,
                    PaymentName = "EMI Installment " + emi.InstallmentNo,
                    PaymentDate = DateTime.Now,
                    PaidAmount = amount,
                    PaymentMode = "Razorpay",
                    TransactionReference = orderId,
                    PrincipalPaid = emi.PrincipalAmount,
                    InterestPaid = emi.InterestAmount,
                    PenaltyPaid = 0,
                    PaymentStatus = "Created"
                });

                await _db.SaveChangesAsync();

                return (true, "Order created", new EmiPaymentOrderDto
                {
                    EmiScheduleId = emi.EmiScheduleId,
                    OrderId = orderId,
                    AmountInPaise = amountInPaise,
                    Currency = "INR",
                    KeyId = _razorpayKeyId
                });
            }
            catch
            {
                return (false, "Could not create Razorpay order", null);
            }
        }

        // Verify payment and update EMI status
        public async Task<(bool Success, string Message)> VerifyPayment(
            VerifyEmiPaymentDto dto, int customerId)
        {
            var emi = await _db.EmiSchedules
                .Include(x => x.LoanAccount)
                .FirstOrDefaultAsync(x =>
                    x.EmiScheduleId == dto.EmiScheduleId &&
                    x.LoanAccount!.CustomerId == customerId);

            if (emi == null)
                return (false, "EMI not found");

            if (emi.PaymentStatus == "Paid")
                return (false, "This EMI is already paid");

            var transaction = await _db.LoanPayment
                .FirstOrDefaultAsync(x =>
                    x.EmiScheduleId == dto.EmiScheduleId &&
                    x.TransactionReference == dto.RazorpayOrderId &&
                    x.PaymentStatus == "Created");

            if (transaction == null)
                return (false, "Payment order not found");

            if (string.IsNullOrWhiteSpace(_razorpayKeySecret))
                return (false, "Razorpay keys are not configured");

            if (string.IsNullOrWhiteSpace(dto.RazorpayOrderId) ||
                string.IsNullOrWhiteSpace(dto.RazorpayPaymentId) ||
                string.IsNullOrWhiteSpace(dto.RazorpaySignature))
                return (false, "Payment details are incomplete");

            if (emi.PaymentStatus != "Pending")
                return (false, "This EMI is not pending");

            // Verify Razorpay signature
            string payload =
                dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_razorpayKeySecret));

            byte[] expectedBytes =
                hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            byte[] receivedBytes;

            try
            {
                receivedBytes = Convert.FromHexString(dto.RazorpaySignature);
            }
            catch
            {
                return (false, "Invalid payment signature");
            }

            if (!CryptographicOperations.FixedTimeEquals(
                expectedBytes, receivedBytes))
                return (false, "Payment verification failed");

            try
            {
                var client = new Razorpay.Api.RazorpayClient(
                    _razorpayKeyId, _razorpayKeySecret);

                var payment = client.Payment.Fetch(dto.RazorpayPaymentId);

                string orderId =
                    Convert.ToString(payment["order_id"]) ?? "";

                string status =
                    Convert.ToString(payment["status"]) ?? "";

                long paidAmount = Convert.ToInt64(payment["amount"]);

                if (orderId != dto.RazorpayOrderId ||
                    status != "captured" ||
                    paidAmount != (long)Math.Round(emi.Emi * 100))
                {
                    return (false,
                        "Payment is not captured or payment details do not match");
                }

                string paymentMode =
                    Convert.ToString(payment["method"])?.ToUpper()
                    ?? "Razorpay";

                // Update transaction
                transaction.TransactionReference = dto.RazorpayPaymentId;
                transaction.PaymentDate = DateTime.Now;
                transaction.PaymentMode = paymentMode;
                transaction.PaymentStatus = "Success";

                // Update EMI schedule
                emi.PaymentStatus = "Paid";
                emi.PaidDate = DateTime.Now;

                await _db.SaveChangesAsync();

                return (true, "EMI payment successful");
            }
            catch
            {
                return (false, "Could not verify payment with Razorpay");
            }
        }
    }
}
