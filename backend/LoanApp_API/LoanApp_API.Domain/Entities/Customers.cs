using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class Customers
    {
        [Key]
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string MobileNo { get; set; }

        public string Pan { get; set; }

        public string AadhaarNo { get; set; }

        public string MonthlyIncome { get; set; }

        public int IsEmailVerified { get; set; }

        public List<LoanDeals> loanDeals { get; set; }

        public List<LoanAccount> LoanAccounts { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
