namespace ProdFlow.Models.Requests
{
    public class SynoptiqueUpdateRequest
    {
        public int ModeID { get; set; }
        public string PtNum { get; set; }
        public string NomMvt { get; set; }
        public int Ordre { get; set; }
        public string Matricule { get; set; }
    }
}
