using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Mapper
{
    public class MappingData : Profile
    {
        public MappingData()
        {
            CreateMap<LoanDeals, LoanDealDTO>().ReverseMap();
            CreateMap<LoanDeals, SanctionPrefillDTO>();
            CreateMap<CreateSanctionDTO, SanctionLetters>().ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));
            CreateMap<DisbursePrefillDTO, Disbursements>();

            CreateMap<CreateDisbursementDTO, Disbursements>();

        }
    }
}
