using System.Collections.Generic;

namespace ProdFlow.DTOs
{
    public class CreateGalliaDto
    {
        public string LabelName { get; set; }
        public string? GalliaName { get; set; }
        public DateTime? LabelDate { get; set; }
        public List<CreateGalliaFieldDto> Fields { get; set; } = new List<CreateGalliaFieldDto>();
    }
}