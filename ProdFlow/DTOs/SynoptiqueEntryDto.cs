using System.ComponentModel.DataAnnotations;

namespace ProdFlow.DTOs
{
    public class SynoptiqueEntryDto
    {
        [Required]
        public int ModeID { get; set; } // Refers to Mode.ID

        [Required]
        public string PtNum { get; set; } // Product ID

        [Required]
        public string NomMvt { get; set; } // Mode name (optional, can be fetched from Mode)

        [Required]
        [Range(1, 100)]
        public int Ordre { get; set; } // Step number (1, 2, 3...)

        public string? Matricule { get; set; } // Can be auto-filled by backend
    }
}
