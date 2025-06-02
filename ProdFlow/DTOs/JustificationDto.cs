// DTOs/JustificationDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.DTOs
{
    public class JustificationDto
    {
        public int JustificationID { get; set; }

        [StringLength(18)]
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string JustificationText { get; set; }

        [StringLength(20)]
        public string UrgencyLevel { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(50)]
        public string SubmittedBy { get; set; }

        public DateTime SubmissionDate { get; set; }  // Non-nullable (required)

        public string DecisionComments { get; set; }

        public DateTime? DecisionDate { get; set; }  // Nullable (optional)

        [StringLength(50)]
        public string DecidedBy { get; set; }
    }
}