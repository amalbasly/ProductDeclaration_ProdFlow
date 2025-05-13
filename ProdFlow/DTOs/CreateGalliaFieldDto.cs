namespace ProdFlow.DTOs
{
    public class CreateGalliaFieldDto
    {
        public string? FieldName { get; set; }
        public string FieldValue { get; set; } = "";
        public int DisplayOrder { get; set; }
        public string VisualizationType { get; set; } = "qrcode";
    }
}