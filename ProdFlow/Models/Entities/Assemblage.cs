namespace ProdFlow.Models.Entities
{
    public class Assemblage
    {
        public int AssemblageId { get; set; }
        public string NomAssemblage { get; set; }
        public string MainProduitPtNum { get; set; }
        public int GalliaId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Produit MainProduit { get; set; }
        public Gallia Gallia { get; set; }
        public List<AssemblageProduit> SecondaryProduits { get; set; }
    }
}
