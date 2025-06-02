namespace ProdFlow.DTOs
{
    public class VerifyProductDto
    {
        public string ProductId { get; set; }
        public string Token { get; set; }
        public bool IsApproved { get; set; }
        public string DecisionComments { get; set; }
    }
}
