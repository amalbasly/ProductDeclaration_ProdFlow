using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("Gallia")]
    public class Gallia
    {
        [Key]
        public int GalliaId { get; set; }
        public string LabelName { get; set; } = "Gallia"; // Default to "Gallia"
        public DateTime? LabelDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? GalliaName { get; set; }

        // Navigation property
        public ICollection<GalliaImage> Images { get; set; }
    }
}