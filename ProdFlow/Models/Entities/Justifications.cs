using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Entities
{
   
    [Table("Justifications")]
    public class Justification
    {
        public int JustificationID { get; set; }

        [Required]
        [StringLength(18)]
        public string ProductCode { get; set; }

        [Required]
        public string JustificationText { get; set; }

        [Required]
        [StringLength(20)]
        public string UrgencyLevel { get; set; } // "Low", "Medium", "High"

        [Required]
        [StringLength(50)]
        public string SubmittedBy { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending"; // Remove [Required]

        public string? DecisionComments { get; set; } // Make nullable

        public DateTime? DecisionDate { get; set; } // Make nullable

        [StringLength(50)]
        public string? DecidedBy { get; set; } // Make nullable

        // Remove this if not needed for submission
        public Produit? Produit { get; set; } // Make nullable
    }
}