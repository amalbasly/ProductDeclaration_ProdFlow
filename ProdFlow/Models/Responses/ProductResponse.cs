namespace ProdFlow.Models.Responses
{
    public class ProductResponse
    {
        public string CodeProduit { get; set; }
        public string Libellé { get; set; }
        public string Libellé2 { get; set; }
        public string Ligne { get; set; }
        public string Famille { get; set; }
        public string SousFamille { get; set; }
        public string Type { get; set; }
        public string JustificationStatus { get; set; }
        public int JustificationID { get; set; }
        public string JustificationText { get; set; }
        public string UrgencyLevel { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string DecisionComments { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string DecidedBy { get; set; }
        public string CodeProduitClientC264 { get; set; }
        public decimal? Poids { get; set; }
        public string Createur { get; set; }
        public bool IsSerialized { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? VerificationDeadline { get; set; }
        public string Tolérances { get; set; }
        public int? Flashable { get; set; }
        public int? GalliaId { get; set; }
        public string GalliaName { get; set; }
    }
}