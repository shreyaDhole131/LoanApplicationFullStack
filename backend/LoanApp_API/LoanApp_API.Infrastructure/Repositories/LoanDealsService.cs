using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Application.Mapper;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class LoanDealsService : ILoanDeals
    {
        AppDbContext db;
        IMapper mapper;

        public LoanDealsService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;

        }

        public async Task applyLoan(LoanDealDTO dto)
        {
            var deals = mapper.Map<LoanDeals>(dto);
            await db.LoanDeals.AddAsync(deals);
            await db.SaveChangesAsync();

        }
    }
}
