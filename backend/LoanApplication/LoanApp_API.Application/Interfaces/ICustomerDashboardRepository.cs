using LoanApp_API.Application.DTO;

namespace LoanApp_API.Application.Interfaces
{
    public interface ICustomerDashboardRepository
    {
        CustomerDashboardVM? GetCustomerDashboard(int userId);

    }
}