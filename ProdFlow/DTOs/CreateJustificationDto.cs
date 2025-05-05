using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.DTOs
{
    public class CreateJustificationDto
    {
        [Required]
        [StringLength(18)]
        public string ProductCode { get; set; }

        [Required]
        public string JustificationText { get; set; }

        [Required]
        [RegularExpression("Low|Medium|High")]
        public string UrgencyLevel { get; set; }

        [Required]
        public string SubmittedBy { get; set; }
    }
}

