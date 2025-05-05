using ProdFlow.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Requests
{
    public class SynoptiqueSaveRequest
    {
        [Required]
        [StringLength(18)]
        public string PtNum { get; set; }

        [StringLength(20)]
        public string Matricule { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one entry is required")]
        public List<SynoptiqueEntryDto> Entries { get; set; }
    }
}
