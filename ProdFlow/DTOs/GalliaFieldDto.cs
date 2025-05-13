namespace ProdFlow.DTOs
{
    public class GalliaFieldDto
    {
        public int GalliaFieldId { get; set; }
        public int GalliaId { get; set; }
        public string? FieldName { get; set; }
        public string FieldValue { get; set; } = "";
        public int DisplayOrder { get; set; }
        public string VisualizationType { get; set; } = "qrcode";
    }
}