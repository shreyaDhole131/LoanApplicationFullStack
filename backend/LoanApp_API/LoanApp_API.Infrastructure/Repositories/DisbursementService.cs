using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LoanApp_API.Domain.Entities;


namespace LoanApp_API.Infrastructure.Repositories
{
    public class DisbursementService : IDisbursement
    {
        AppDbContext db;
        IMapper mapper;

        public DisbursementService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task ApplyDisbursement(CreateDisbursementDTO dto)
        {
            var alreadyExists = await db.Disbursements.AnyAsync(d => d.DealId == dto.DealId);

            if (alreadyExists) throw new InvalidOperationException($"Deal {dto.DealId} already disbursed");

            var disb = mapper.Map<Disbursements>(dto);

            await db.Disbursements.AddAsync(disb);

            await db.SaveChangesAsync();
        }

        public async Task<DisbursePrefillDTO?> fetchDisbursePrefill(int dealId)
        {
            var letter = await db.SanctionLetters.FirstOrDefaultAsync(s => s.DealId == dealId);
            if (letter == null) return null;

            return new DisbursePrefillDTO
            {
                DealId = letter.DealId,
                DisburseAmount = letter.LoanAmount,
                DisbursementDate = DateTime.UtcNow
            };
        }


    }
}
