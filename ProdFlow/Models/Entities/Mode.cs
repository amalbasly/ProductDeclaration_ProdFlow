using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Entities
{
    [Table("Mode")]
    public class Mode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("Specif")]
        [MaxLength(50)]
        public string? Specif { get; set; }

        [Column("NomMode")]
        [MaxLength(50)]
        public string? NomMode { get; set; }
    }
}
