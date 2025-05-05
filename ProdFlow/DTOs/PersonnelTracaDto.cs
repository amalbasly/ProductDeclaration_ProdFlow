namespace ProdFlow.DTOs
{
    public class PersonnelTracaDto
    {
        public long pl_matric { get; set; }

        public long pl_badge { get; set; }
        public string? pl_nom { get; set; }
        public string? pl_prenom { get; set; }
        public string? pl_fonc { get; set; }
        public int? idGrp { get; set; }
        public string? img { get; set; }
        public GroupeDto? Groupe { get; set; } // Include Groupe data
    }
}
