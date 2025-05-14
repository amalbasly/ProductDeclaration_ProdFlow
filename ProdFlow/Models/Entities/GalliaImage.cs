using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    public class GalliaImage
    {
        [Key]
        public int GalliaImageId { get; set; }

        [Required]
        public int GalliaId { get; set; }

        [Required]
        public string LabelImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("GalliaId")]
        public Gallia Gallia { get; set; }
    }
}
