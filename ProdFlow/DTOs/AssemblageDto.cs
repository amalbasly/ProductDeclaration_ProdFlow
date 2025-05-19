namespace ProdFlow.DTOs
{
    public class AssemblageDto
    {
            public int AssemblageId { get; set; }
            public string NomAssemblage { get; set; }
            public string MainProduitPtNum { get; set; }
            public string MainProduitLib { get; set; }
            public int GalliaId { get; set; }
            public string GalliaName { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<SecondaryProduitDto> SecondaryProduits { get; set; }
        }
    }
