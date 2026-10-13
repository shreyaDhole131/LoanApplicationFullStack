using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class CustomerDashboardVM
    {
        public int TotalActiveLoanAccounts { get; set; }

        public string SelectedLoanAccountNo { get; set; }

        public List<CustomerLoanDropdownVM> LoanAccounts { get; set; }

        public CustomerLoanDetailsVM SelectedLoanDetails { get; set; }
    }

    public class CustomerLoanDropdownVM
    {
        public string LoanAccountNo { get; set; }
    }

    public class CustomerLoanDetailsVM
    {
        public string LoanAccountNo { get; set; }

        public decimal OutstandingPrincipal { get; set; }

        public DateTime? NextEmiDate { get; set; }
    }
}
