using System.ComponentModel.DataAnnotations;

namespace ProdFlow.DTOs
{
    public class UpdateGalliaDto : CreateGalliaDto
    {
        [Required]
        public int GalliaId { get; set; }
    }
}
