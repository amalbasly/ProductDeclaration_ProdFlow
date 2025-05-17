using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    // FlanPartie.cs
    [Table("FlanPartie")]
    public class FlanPartie
    {
        [Key]
        [StringLength(20)]
        public string CodePartie { get; set; }

        [Required]
        [StringLength(18)]
        public string pt_numOriginal { get; set; }

        [Required]
        public int NumeroPartie { get; set; }

        [StringLength(50)]
        public string Label { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Foreign key to FlanDecoupe
        public int FlanDecoupeId { get; set; }

        // Navigation properties
        [ForeignKey("pt_numOriginal")]
        public Produit Produit { get; set; }

        [ForeignKey("FlanDecoupeId")]
        public FlanDecoupe FlanDecoupe { get; set; }
    }
}