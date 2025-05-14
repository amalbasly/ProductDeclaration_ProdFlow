using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("Gallia")]
    public class Gallia
    {
        [Key]
        public int GalliaId { get; set; }
        public DateTime? LabelDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<GalliaImage> Images { get; set; }
    }
}