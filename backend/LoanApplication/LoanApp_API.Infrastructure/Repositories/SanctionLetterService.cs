using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class SanctionLetterService : ISanctionLetter
    {
        AppDbContext db;
        IMapper mapper;

        public SanctionLetterService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<SanctionLetters> AddSanctionLetter(CreateSanctionDTO dto)
        {
            var letter = mapper.Map<SanctionLetters>(dto);
            await db.SanctionLetters.AddAsync(letter);
            await db.SaveChangesAsync();
            return letter;
        }

        public async Task<List<PendingSanctionDTO>> fetchCustomers()
        {
            return await db.LoanDeals.Where(d => d.CurrentStatus == "Approved" && !db.SanctionLetters.Any(s => s.DealId == d.DealId))
                .Select(d =>

                    new PendingSanctionDTO
                    {
                        DealId = d.DealId,
                        Name = d.customers.FirstName + " " + d.customers.LastName
                    }).ToListAsync();

        }

        public async Task<SanctionPrefillDTO?> fetchSanctionDetails(int dealId)
        {
            var deal = await db.LoanDeals.FirstOrDefaultAsync(d => d.DealId == dealId);

            if (deal == null) return null;

            return mapper.Map<SanctionPrefillDTO>(deal);

        }

        //public async Task<List<SanctionGridDTO>> fetchSanctionGrids()
        //{
        //    return await db.SanctionLetters.Include(s => s.deals).ThenInclude(s => s.customers).Select(s => new SanctionGridDTO
        //    {
        //        CustomerID = s.deals.customers.CustomerId,
        //        CustomerName = s.deals.customers.FirstName + " " + s.deals.customers.LastName,
        //        LoanAmount = s.LoanAmount,
        //        LoanType = s.deals.LoanType,
        //        InterestRate = s.InterestRate,
        //        TenureMonths = s.TenureMonths,
        //        EmiAmount = s.EmiAmount,
        //        AppliedDate = s.CreatedAt,
        //        IsDisbursed = s.deals.disbursement != null,      
        //        DisbursementStatus = s.deals.disbursement == null
        //                    ? "Not Disbursed"
        //                    : "Disbursed"
        //    }).ToListAsync();
        //}

        public async Task<List<SanctionGridDTO>> fetchSanctionsGrid(string? name, string? FromDate, string? ToDate, string? Filter, string? sort)
        {
            var query = db.LoanDeals.Where(d => d.CurrentStatus == "Approved").AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => (d.customers.FirstName + " " + d.customers.LastName).Contains(name));
            }

            if (!string.IsNullOrEmpty(FromDate) && DateTime.TryParse(FromDate, out var from))
            {
                query = query.Where(d => d.AppliedDate >= from);

            }

            if (!string.IsNullOrWhiteSpace(ToDate) && DateTime.TryParse(ToDate, out var to))
            {
                var toEnd = to.Date.AddDays(1);

                query = query.Where(d => d.AppliedDate < toEnd);
            }

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                query = query.Where(d => d.LoanType == Filter);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort?.ToLower() == "ascending" ? query.OrderBy(d => d.AppliedDate) : query.OrderByDescending(d => d.AppliedDate);
            }

            return await query.Select(d => new SanctionGridDTO
            {
                SanctionLetterId = d.sanctionLetters == null
                        ? (int?)null
                        : d.sanctionLetters.SanctionId,
                DealId = d.DealId,
                CustomerID = d.customers.CustomerId,
                CustomerName = d.customers.FirstName + " " + d.customers.LastName,
                LoanAmount = d.LoanAmount,
                LoanType = d.LoanType,
                InterestRate = d.InterestRate,
                TenureMonths = d.TenureMonths,
                EmiAmount = d.EmiAmount,
                AppliedDate = d.AppliedDate,

                IsDisbursed = d.disbursement != null,
                DisbursementStatus = db.Disbursements.Any(x => x.DealId == d.DealId)
                        ? "Disbursed"
                        : "Not Disbursed"
            }).ToListAsync();
        }

        public byte[] GenerateSanctionLetterPdf(SanctionLetterDTO l)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        col.Item().Text("LOAN SANCTION LETTER")
                            .FontSize(20).Bold().AlignCenter();

                        col.Item().PaddingTop(20)
                            .Text($"Date: {DateTime.Now:dd/MM/yyyy}");

                        col.Item().PaddingTop(10)
                            .Text($"Dear {l.CustomerName},");

                        col.Item().PaddingTop(10).Text(
                            "Thank you for choosing ABC Bank. Based on your application " +
                            "and information provided therein, we are pleased to extend " +
                            "an offer to you for a loan as per the preliminary terms and " +
                            "conditions mentioned below.");

                        col.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(200);
                                c.RelativeColumn();
                            });

                            void Row(string k, string v)
                            {
                                table.Cell().Border(1).Padding(5).Text(k).Bold();
                                table.Cell().Border(1).Padding(5).Text(v);
                            }

                            Row("Application No", $"APP{l.SanctionLetterId}");
                            Row("Sanction Date", DateTime.Now.ToString("dd/MM/yyyy"));
                            Row("Applicant Name", l.CustomerName);
                            Row("Mobile No", l.MobileNo);
                            Row("Loan Type", l.LoanType);
                            Row("Loan Amount Sanctioned", $"Rs. {l.LoanAmount:F2}");
                            Row("Reference Interest Rate", $"{l.InterestRate}% per annum");
                            Row("Loan Tenure", $"{l.TenureMonths} Months");
                            Row("Total Processing Charges", "Up to 0.5% of Loan Amount");
                            Row("Origination Fee", "2500");
                            Row("Sanction Letter Validity", "180 Days");
                            Row("Amount of EMI", $"Rs. {l.EmiAmount:F2}");
                        });

                        col.Item().PaddingTop(20)
                            .Text("Additional Conditions To Comply Prior To Loan Disbursal:").Bold();

                        col.Item().PaddingTop(5)
                            .Text("1. Repayment from ABC Bank");
                        col.Item().Text("2. Legal vetting and search to be conducted");
                        col.Item().Text("3. NOC and offered collateral");
                        col.Item().Text("4. Confirmation form, official ID and copy of ID");

                        col.Item().PaddingTop(30).Text("Authorized Signatory").Bold();
                        col.Item().Text("ABC Bank");
                    });
                });
            }).GeneratePdf();
        }

        public async Task<SanctionLetterDTO?> GetSanctionLetter(int letterId)
        {
            return await db.SanctionLetters
        .Where(l => l.SanctionId == letterId)
        .Select(l => new SanctionLetterDTO
        {
            SanctionLetterId = l.SanctionId,
            DealId = l.DealId,
            LoanAmount = l.LoanAmount,
            LoanType = l.deals.LoanType,
            TenureMonths = l.TenureMonths,
            InterestRate = l.InterestRate,
            EmiAmount = l.EmiAmount,
            CreatedAt = l.CreatedAt,
            CustomerName = l.deals.customers.FirstName + " " + l.deals.customers.LastName,
            MobileNo = l.deals.customers.MobileNo
        })
        .FirstOrDefaultAsync();
        }
    }
}
