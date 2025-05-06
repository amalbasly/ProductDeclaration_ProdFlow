using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("ProductGallia")]

    public class ProductGallia
    {
        public int ProductGalliaId { get; set; }
        public int? GalliaId { get; set; }          // Foreign key to Gallia
        public string Pt_Num { get; set; }          // Foreign key to Product
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public string SupplierReference { get; set; }
        public string LabelNumber { get; set; }
        public string Description { get; set; }
        public string SupplierName { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime CreatedAt { get; set; }      // Auto-set to GETDATE()

        // Navigation properties
        public Gallia Gallia { get; set; }
        public Produit Produit { get; set; }
    }
}