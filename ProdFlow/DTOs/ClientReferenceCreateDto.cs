using System.ComponentModel.DataAnnotations;

namespace ProdFlow.DTOs
{
    public class ClientReferenceCreateDto
    {
        [Required]
        [StringLength(255)]
        public string ClientReference { get; set; }

        [Required]
        [StringLength(255)]
        public string ClientIndex { get; set; }

        [Required]
        [StringLength(255)]
        public string Client { get; set; }

        [Required]
        [StringLength(255)]
        public string Referentiel { get; set; }
    }
}
