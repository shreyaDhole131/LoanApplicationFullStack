using System;
using System.Collections.Generic;
using System.Text;


using LoanApp_API.Application.DTO.EmiPayment;

namespace LoanApp_API.Application.Interfaces
{
    public interface IEmiPaymentRepository
    {
        Task<EmiPaymentDto?> GetPaymentDetails(
            int emiScheduleId, int customerId);

        Task<(bool Success, string Message, EmiPaymentOrderDto? Data)>
            CreatePaymentOrder(int emiScheduleId, int customerId);

        Task<(bool Success, string Message)> VerifyPayment(
            VerifyEmiPaymentDto dto, int customerId);
    }
}
