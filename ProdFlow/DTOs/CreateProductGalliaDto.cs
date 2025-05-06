namespace ProdFlow.DTOs
{
    public class CreateProductGalliaDto
    {
        public int GalliaId { get; set; }
        public string Pt_Num { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string SupplierReference { get; set; }
        public string LabelNumber { get; set; }
        public string Description { get; set; }
        public string SupplierName { get; set; }
        public DateTime? ProductionDate { get; set; }
    }
}
