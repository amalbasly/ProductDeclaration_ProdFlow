namespace ProdFlow.DTOs
{
    public class ProductResult
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public string ProductCode { get; set; }
        public bool? IsSerialized { get; set; }
        public List<ProduitSerialiséDto> Products { get; set; }
    }
}