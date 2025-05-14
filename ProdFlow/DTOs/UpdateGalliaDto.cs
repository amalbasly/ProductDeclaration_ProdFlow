using System.Collections.Generic;

namespace ProdFlow.DTOs
{
    public class UpdateGalliaDto
    {
        public int GalliaId { get; set; }
        public string LabelName { get; set; } // <-- Added
        public DateTime? LabelDate { get; set; }
        public List<CreateGalliaFieldDto> Fields { get; set; } = new List<CreateGalliaFieldDto>();
    }
}