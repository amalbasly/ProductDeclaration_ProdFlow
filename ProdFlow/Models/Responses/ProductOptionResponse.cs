namespace ProdFlow.DTOs
{
    public class ProductOptionResponse
    {
        public List<string> Lignes { get; set; }
        public List<string> Famille { get; set; }
        public List<string> SousFamilles { get; set; }
        public List<string> Types { get; set; }
        public List<string> Statuts { get; set; }
    }
}