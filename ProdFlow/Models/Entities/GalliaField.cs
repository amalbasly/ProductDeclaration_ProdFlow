using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("GalliaField")]
    public class GalliaField
    {
        [Key]
        public int GalliaFieldId { get; set; }

        public int GalliaId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FieldValue { get; set; } = "";

        public int DisplayOrder { get; set; }

        [Required]
        [MaxLength(20)]
        public string VisualizationType { get; set; } = "qrcode";

        [MaxLength(100)]
        public string? FieldName { get; set; }
    }
}