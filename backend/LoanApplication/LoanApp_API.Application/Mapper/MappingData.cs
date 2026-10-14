
using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.DTO.PaymentHistory;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Mapper
{
    public class MappingData : Profile
    {
        public MappingData()
        {
            // Existing mappings
            CreateMap<LoanDeals, LoanDealDTO>().ReverseMap();

            CreateMap<LoanAccount, LoanAccountDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.Customer.FirstName + " " +
                        src.Customer.LastName));

            CreateMap<EmiSchedule, EmiScheduleDto>();

            CreateMap<LoanDeals, SanctionPrefillDTO>();

            CreateMap<CreateSanctionDTO, SanctionLetters>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<DisbursePrefillDTO, Disbursements>();

            CreateMap<CreateDisbursementDTO, Disbursements>();

            // Foreclosure: customer loan dropdown
            CreateMap<LoanAccount, CustomerLoanDto>();

            // Foreclosure: customer requests
            CreateMap<ForeClosureRequest, CustomerForeClosureDto>()
                .ForMember(dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.LoanAccountNo))
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src =>
                        src.ForeClosureType == "Full"
                            ? src.ForeClosureAmount
                            : src.PartialAmount))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src =>
                        src.IsPaid ? "Paid" : src.Status));

            // Foreclosure: officer dashboard and details
            CreateMap<ForeClosureRequest, OfficerForeClosureDto>()
                .ForMember(dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.LoanAccountNo))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.Customer!.FirstName + " " +
                        src.LoanAccount.Customer.LastName))
                .ForMember(dest => dest.LoanAmount,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.LoanAmount))
                .ForMember(dest => dest.InterestRate,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.InterestRate))
                .ForMember(dest => dest.DisbursedOn,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.DisbursementDate))
                .ForMember(dest => dest.OutstandingPrincipal,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.OutstandingPrincipal))
                .ForMember(dest => dest.RequestDate,
                    opt => opt.MapFrom(src => src.RequestedDate))
                .ForMember(dest => dest.CustomerNote,
                    opt => opt.MapFrom(src => src.Reason));

            // Payment history
            CreateMap<LoanPayment, PaymentHistoryDto>()
                .ForMember(dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.LoanAccountNo));
        }
    }
}
