using Microsoft.EntityFrameworkCore;

namespace ProdFlow.Models.Responses
{
    [Keyless]
    public class StoredProcedureResult
    {
        public long pl_matric { get; set; }
        public long pl_badge { get; set; }
        public string? pl_nom { get; set; }
        public string? pl_prenom { get; set; }
        public string? pl_fonc { get; set; }
        public int? IDGrp { get; set; }
        public string? img { get; set; }
        public int? Groupe_IDGrp { get; set; } // Additional columns for Groupe
        public string? Groupe_descriptionGrp { get; set; }
    }
}
