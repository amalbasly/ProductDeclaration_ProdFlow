namespace ProdFlow.DTOs
{
    public class GalliaDto
    {
        public int GalliaId { get; set; }
        public string LabelName { get; set; }
        public DateTime? LabelDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? GalliaName { get; set; }
        public string? LabelImage { get; set; }
        public List<GalliaFieldDto> Fields { get; set; } = new List<GalliaFieldDto>();
    }
}