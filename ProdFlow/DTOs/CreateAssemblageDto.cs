namespace ProdFlow.DTOs
{
    public class CreateAssemblageDto
    {
        public string NomAssemblage { get; set; }
        public string MainProduitPtNum { get; set; }
        public string GalliaName { get; set; }
        public List<string> SecondaryProduitPtNums { get; set; }
    }
}
