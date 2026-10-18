
using AutoMapper;
using LoanApp_API.Application.DTO;

using LoanApp_API.Application.DTO.User;

using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.DTO.PaymentHistory;

using LoanApp_API.Domain.Entities;
using System;

namespace LoanApp_API.Application.Mapper
{
    public class MappingData : Profile
    {
        public MappingData()
        {

            CreateMap<CustomerRegistrationDTO, Customers>();
            CreateMap<LoanDeals, LoanDealDTO>().ReverseMap();

            // Loan Deals
            CreateMap<LoanDeals, LoanDealDTO>()
                .ReverseMap();


            // Loan Account
            CreateMap<LoanAccount, LoanAccountDto>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.Customer.FirstName + " " +
                        src.Customer.LastName));

            // EMI Schedule
            CreateMap<EmiSchedule, EmiScheduleDto>();

            // Sanction Letter
            CreateMap<LoanDeals, SanctionPrefillDTO>();

            CreateMap<CreateSanctionDTO, SanctionLetters>()
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Disbursement
            CreateMap<DisbursePrefillDTO, Disbursements>();

            CreateMap<CreateDisbursementDTO, Disbursements>();

            // Customer Registration
            CreateMap<CustomerRegistrationDTO, Customers>();

            // KYC
            CreateMap<KycDocumentDTO, KycDocument>();

            CreateMap<KycDocument, KycDocumentDTO>();

            // Customer Support Ticket
            CreateMap<SupportTicketDTO, SupportTicket>();

            // Score Card
            CreateMap<ScoreCard, ScoreCardProfileDto>();

            // Officer Customer List
            CreateMap<Customers, OfficerCustomerListVM>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName));

            // Officer Customer Profile
            CreateMap<Customers, OfficerCustomerProfileVM>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName))
                .ForMember(
                    dest => dest.Mobile,
                    opt => opt.MapFrom(src => src.MobileNo))
                .ForMember(
                    dest => dest.Aadhaar,
                    opt => opt.MapFrom(src => src.AadhaarNo))
                .ForMember(
                    dest => dest.Salary,
                    opt => opt.MapFrom(src => src.MonthlyIncome))
                .ForMember(
                    dest => dest.Employment,
                    opt => opt.MapFrom(src => src.EmploymentType))
                .ForMember(
                    dest => dest.CibilScore,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RiskCategory,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.EligibleLoanAmount,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.Loans,
                    opt => opt.Ignore());

            // Officer Loan Details
            CreateMap<LoanAccount, OfficerLoanDetailsVM>();

            // Officer EMI History
            CreateMap<EmiSchedule, OfficerEmiHistoryVM>();

            // Officer KYC Customer
            CreateMap<Customers, OfficerKycCustomerVM>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.FirstName + " " + src.LastName))
                .ForMember(
                    dest => dest.Mobile,
                    opt => opt.MapFrom(src => src.MobileNo));

            // Officer KYC Document
            CreateMap<KycDocument, OfficerKycDocumentVM>();

            // Officer Support Ticket
            CreateMap<SupportTicket, OfficerSupportTicketVM>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.Customer != null
                            ? src.Customer.FirstName + " " +
                              src.Customer.LastName
                            : string.Empty))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src =>
                        src.Customer != null
                            ? src.Customer.Email
                            : string.Empty))
                .ForMember(
                    dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount != null
                            ? src.LoanAccount.LoanAccountNo
                            : string.Empty));

            // Customer Application
            CreateMap<LoanDeals, CustomerApplicationVM>();

            // Customer Loan Dropdown
            CreateMap<LoanAccount, CustomerLoanDropdownVM>();

            // Customer Loan Details
            CreateMap<LoanAccount, CustomerLoanDetailsVM>()
                .ForMember(
                    dest => dest.LoanType,
                    opt => opt.MapFrom(src =>
                        src.LoanDeal != null
                            ? src.LoanDeal.LoanType
                            : string.Empty))
                .ForMember(
                    dest => dest.NextEmiDate,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.TotalEmis,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.PaidEmis,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.RemainingEmis,
                    opt => opt.Ignore());

            // Customer EMI Payment
            CreateMap<EmiSchedule, CustomerEmiPaymentVM>();

            // Customer KYC
            CreateMap<KycDocument, CustomerKycVM>();

            // Officer Dashboard - Application Funnel
            CreateMap<LoanDeals, OfficerApplicationFunnelVM>()
                .ForMember(
                    dest => dest.Stage,
                    opt => opt.MapFrom(src =>
                        src.CurrentStatus ?? "Unknown"))
                .ForMember(
                    dest => dest.Count,
                    opt => opt.MapFrom(src => 1));

            // Officer Dashboard - Loan Portfolio
            CreateMap<OfficerLoanPortfolioData, OfficerLoanPortfolioVM>();


            // Foreclosure - Customer Loan Dropdown
            CreateMap<LoanAccount, CustomerLoanDto>();

            // Foreclosure - Customer Requests
            CreateMap<ForeClosureRequest, CustomerForeClosureDto>()
                .ForMember(
                    dest => dest.RequestId,
                    opt => opt.MapFrom(src => src.RequestId))
                .ForMember(
                    dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount != null
                            ? src.LoanAccount.LoanAccountNo
                            : string.Empty))
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(src =>
                        src.ForeClosureType == "Full"
                            ? src.ForeClosureAmount
                            : src.PartialAmount))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src =>
                        src.IsPaid ? "Paid" : src.Status));

            // Foreclosure - Officer Dashboard and Details
            CreateMap<ForeClosureRequest, OfficerForeClosureDto>()
                .ForMember(
                    dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount != null
                            ? src.LoanAccount.LoanAccountNo
                            : string.Empty));



            // Payment History
            CreateMap<LoanPayment, PaymentHistoryDto>()
                .ForMember(
                    dest => dest.LoanAccountNo,
                    opt => opt.MapFrom(src =>
                        src.LoanAccount!.LoanAccountNo));
        }
    }
}
