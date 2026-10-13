using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class CibilReport
    {
        [Key]
        public int CibilReportId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public string PanNo { get; set; }
        public int CibilScore { get; set; }
        public DateTime CheckDate { get; set; }
        public Customers? Customer { get; set; }
    }
}
