using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class DealReviews
    {

        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("DealId")]
        public int DealId { get; set; }

        public LoanDeals deals { get; set; }

        [ForeignKey("OfficerId")]
        public int OfficerId { get; set; }

        public User users { get; set; }

        public string Status { get; set; }
    }
}
