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

        public ForeClosureRepository(AppDbContext db)
        {
            _db = db;
        }

        // =====================  CUSTOMER SIDE  =====================

        // Dropdown: only Active loans of this customer
        public async Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId)
        {
            return await _db.LoanAccounts
                .Where(x => x.CustomerId == customerId && x.LoanStatus == "Active")
                .Select(x => new CustomerLoanDto
                {
                    LoanAccountId = x.LoanAccountId,
                    LoanAccountNo = x.LoanAccountNo
                })
                .ToListAsync();
        }

        // 3 cards on top
        public async Task<ForeClosureSummaryDto?> GetSummary(int loanAccountId)
        {
            var loan = await _db.LoanAccounts.FindAsync(loanAccountId);
            if (loan == null) return null;

            var nextEmi = await GetNextPendingEmi(loanAccountId);

            return new ForeClosureSummaryDto
            {
                LoanAccountId = loan.LoanAccountId,
                LoanAccountNo = loan.LoanAccountNo,
                OutstandingPrincipal = nextEmi?.OpeningBalance ?? 0,
                NextEmiDate = nextEmi?.DueDate
            };
        }

        // Submit Request button
        public async Task<(bool Success, string Message)> SubmitRequest(ForeClosureDto dto)
        {
            var loan = await _db.LoanAccounts.FindAsync(dto.LoanAccountId);
            if (loan == null || loan.CustomerId != dto.CustomerId)
                return (false, "Loan not found");

            if (loan.LoanStatus != "Active")
                return (false, "Loan already closed");

            if (dto.ForeClosureType != "Full" && dto.ForeClosureType != "Partial")
                return (false, "Select foreclosure type (Full or Partial)");

            if (dto.ExpectedClosureDate.Date < DateTime.Today)
                return (false, "Expected closure date cannot be a past date");

            // only one request per loan (a Rejected one does not count)
            bool alreadyExists = await _db.ForeClosureRequests
                .AnyAsync(x => x.LoanAccountId == dto.LoanAccountId && x.Status != "Rejected");
            if (alreadyExists)
                return (false, "Foreclosure request already exists for this loan account");

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
                Reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Nil" : dto.Reason,
                RequestedDate = DateTime.Now,
                Status = "Pending"
            };

            if (dto.ForeClosureType == "Partial")
            {
                if (dto.PartialAmount <= 0)
                    return (false, "Enter a valid partial amount");

                if (dto.PartialAmount >= nextEmi.OpeningBalance)
                    return (false, "Partial amount must be less than outstanding principal");

                request.PartialAmount = dto.PartialAmount;
            }
            else
            {
                // Full = outstanding + interest since last paid EMI + 2% charge
                decimal outstanding = nextEmi.OpeningBalance;
                DateTime lastPaidDate = lastPaidEmi.PaidDate ?? lastPaidEmi.DueDate;
                int days = Math.Max(0, (dto.ExpectedClosureDate.Date - lastPaidDate.Date).Days);

                decimal interest = outstanding * loan.InterestRate * days / 36500;
                decimal charges = outstanding * 0.02m;

                request.ForeClosureAmount = Math.Round(outstanding + interest + charges, 2);
            }

            _db.ForeClosureRequests.Add(request);
            await _db.SaveChangesAsync();

            return (true, "Foreclosure request submitted");
        }

        // "My Foreclosure Requests" table
        public async Task<List<CustomerForeClosureDto>> GetMyRequests(int customerId)
        {
            return await _db.ForeClosureRequests
                .Where(x => x.LoanAccount!.CustomerId == customerId)
                .OrderByDescending(x => x.RequestedDate)
                .Select(x => new CustomerForeClosureDto
                {
                    LoanAccountNo = x.LoanAccount!.LoanAccountNo,
                    Amount = x.ForeClosureType == "Full" ? x.ForeClosureAmount : x.PartialAmount,
                    ForeClosureType = x.ForeClosureType,
                    Reason = x.Reason,
                    Status = x.IsPaid ? "Paid" : x.Status
                })
                .ToListAsync();
        }

        // =====================  OFFICER SIDE  =====================

        // Dashboard table
        public async Task<List<OfficerForeClosureDto>> GetAllRequests()
        {
            return await OfficerQuery()
                .OrderByDescending(x => x.RequestDate)
                .ToListAsync();
        }

        // View page
        public async Task<OfficerForeClosureDto?> GetRequestById(int id)
        {
            return await OfficerQuery()
                .FirstOrDefaultAsync(x => x.RequestId == id);
        }

        public Task<(bool Success, string Message)> Approve(int id) => ChangeStatus(id, "Approved");

        public Task<(bool Success, string Message)> Reject(int id) => ChangeStatus(id, "Rejected");

        // =====================  SMALL HELPERS (used more than once)  =====================

        private async Task<EmiSchedule?> GetNextPendingEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId && x.PaymentStatus == "Pending")
                .OrderBy(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

        private async Task<EmiSchedule?> GetLastPaidEmi(int loanAccountId)
        {
            return await _db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId && x.PaymentStatus == "Paid")
                .OrderByDescending(x => x.InstallmentNo)
                .FirstOrDefaultAsync();
        }

        // One query for the officer list AND the officer details page
        private IQueryable<OfficerForeClosureDto> OfficerQuery()
        {
            return _db.ForeClosureRequests.Select(x => new OfficerForeClosureDto
            {
                RequestId = x.RequestId,
                LoanAccountNo = x.LoanAccount!.LoanAccountNo,
                CustomerName = x.LoanAccount.Customer!.FirstName + " " + x.LoanAccount.Customer.LastName,
                LoanAmount = x.LoanAccount.LoanAmount,
                InterestRate = x.LoanAccount.InterestRate,
                DisbursedOn = x.LoanAccount.DisbursementDate,
                OutstandingPrincipal = x.LoanAccount.OutstandingPrincipal,
                ForeClosureType = x.ForeClosureType,
                RequestDate = x.RequestedDate,
                ExpectedClosureDate = x.ExpectedClosureDate,
                ForeClosureAmount = x.ForeClosureAmount,
                Status = x.Status,
                CustomerNote = x.Reason
            });
        }

        // Approve and Reject do the same work, only the new status is different
        private async Task<(bool Success, string Message)> ChangeStatus(int id, string newStatus)
        {
            var request = await _db.ForeClosureRequests.FindAsync(id);
            if (request == null)
                return (false, "Request not found");

            if (request.Status != "Pending")
                return (false, $"Only pending requests can be changed. Current status: {request.Status}");

            request.Status = newStatus;
            if (newStatus == "Approved")
                request.ApprovedDate = DateTime.Now;

            await _db.SaveChangesAsync();
            return (true, $"Foreclosure request {newStatus.ToLower()}");
        }
    }
}
