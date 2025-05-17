namespace ProdFlow.Models.Requests
{
    public class FlanDecoupeRequestDto
    {
        public string PtNumOriginal { get; set; }
        public int NombreDeParts { get; set; }
        public string LabelUtilise { get; set; }
        public string Utilisateur { get; set; }
    }
}