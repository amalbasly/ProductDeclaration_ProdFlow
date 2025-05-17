using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProdFlow.Models.Entities
{
    // FlanDecoupe.cs
    [Table("FlanDecoupe")]
    public class FlanDecoupe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDecoupe { get; set; }

        [Required]
        [StringLength(18)]
        [ForeignKey("Produit")]
        public string pt_numOriginal { get; set; }

        [Required]
        public int NombreDeParts { get; set; }

        [StringLength(50)]
        public string LabelUtilise { get; set; }

        public DateTime DateDecoupe { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Utilisateur { get; set; }

        // Navigation properties
        public Produit Produit { get; set; }
        public ICollection<FlanPartie> Parts { get; set; } = new List<FlanPartie>();
    }
}