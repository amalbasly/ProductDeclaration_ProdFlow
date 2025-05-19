namespace ProdFlow.Models.Entities
{
    public class AssemblageProduit
    {
        public int AssemblageProduitId { get; set; }
        public int AssemblageId { get; set; }
        public string ProduitPtNum { get; set; }

        // Navigation properties
        public Assemblage Assemblage { get; set; }
        public Produit Produit { get; set; }
    }
}
