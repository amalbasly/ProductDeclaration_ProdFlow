using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("VerificationToken")]
    public class VerificationToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("pt_num")]
        [StringLength(18)]
        public string PtNum { get; set; } // Matches column name in database

        [Required]
        [Column("Token")]
        [StringLength(100)]
        public string Token { get; set; }

        [Required]
        [Column("TraceabilityManagerId")]
        [StringLength(50)]
        public string TraceabilityManagerId { get; set; }

        [Required]
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("PtNum")]
        public Produit Produit { get; set; }
    }
}