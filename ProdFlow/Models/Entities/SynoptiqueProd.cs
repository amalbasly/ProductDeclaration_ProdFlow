using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Entities
{
    [Table("SynoptiqueProd")]
    public class SynoptiqueProd
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Column("pt_num", TypeName = "varchar(18)")]
        public string PtNum { get; set; }

        [Required]
        [Column("NomMvt", TypeName = "nvarchar(50)")]
        public string NomMvt { get; set; }

        [Required]
        [Column("Ordre")]
        public int Ordre { get; set; } // Ranking order (1, 2, 3...)

        [Required]
        [Column("DateCrea")]
        public DateTime DateCrea { get; set; } = DateTime.Now;

        [Required]
        [Column("Matricule", TypeName = "nvarchar(50)")]
        public string Matricule { get; set; } // User who assigned it

        // Navigation properties (optional, for EF Core relationships)
        public Mode? Mode { get; set; }
        public Produit? Produit { get; set; }
    }
}
