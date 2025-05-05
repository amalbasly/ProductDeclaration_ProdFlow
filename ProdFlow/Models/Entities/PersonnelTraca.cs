using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Entities
{
    [Table("PersonnelTraca")]
    public class PersonnelTraca
    {
        [Key]
        public long pl_matric { get; set; }

        public long pl_badge { get; set; }

        public string? pl_nom { get; set; }
        public string? pl_prenom { get; set; } // Fixed typo
        public string? pl_fonc { get; set; }
        public int? IDGrp { get; set; }
        public string? img { get; set; }

        [ForeignKey("IDGrp")] // Add ForeignKey attribute
        public Groupe? Groupe { get; set; }
    }
}