using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task AddSanctionLetter(CreateSanctionDTO dto)
        {
            var letter = mapper.Map<SanctionLetters>(dto);
            await db.SanctionLetters.AddAsync(letter);
            await db.SaveChangesAsync();

        }

        public async Task<List<PendingSanctionDTO>> fetchCustomers()
        {
            return await db.LoanDeals.Where(d => d.CurrentStatus == "Approved" && !db.SanctionLetters.Any(s => s.DealId == d.DealId))
                .Select(d =>
                
                    new PendingSanctionDTO { 
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
            var query = db.SanctionLetters.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => (s.deals.customers.FirstName + " " + s.deals.customers.LastName).Contains(name));
            }

            if (!string.IsNullOrEmpty(FromDate) && DateTime.TryParse(FromDate, out var from))
            {
                query = query.Where(s => s.CreatedAt >= from);

            }

            if(!string.IsNullOrWhiteSpace(ToDate) &&  DateTime.TryParse(ToDate, out var to))
            {
                var toEnd = to.Date.AddDays(1);

                query = query.Where(s => s.CreatedAt < toEnd);
            }

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                query = query.Where(s => s.deals.LoanType == Filter);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort?.ToLower() == "ascending" ? query.OrderBy(s => s.CreatedAt) : query.OrderByDescending(s => s.CreatedAt);
            }

            return await query.Select(s => new SanctionGridDTO
            {
                CustomerID = s.deals.customers.CustomerId,
                CustomerName = s.deals.customers.FirstName + " " + s.deals.customers.LastName,
                LoanAmount = s.LoanAmount,
                LoanType = s.deals.LoanType,
                InterestRate = s.InterestRate,
                TenureMonths = s.TenureMonths,
                EmiAmount = s.EmiAmount,
                AppliedDate = s.CreatedAt,
                IsDisbursed = s.deals.disbursement != null,       
                DisbursementStatus = s.deals.disbursement == null
                            ? "Not Disbursed"
                            : "Disbursed"
            }).ToListAsync();
        }
    }
}
