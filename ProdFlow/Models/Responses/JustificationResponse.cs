namespace ProdFlow.Models.Responses
{
    public class JustificationResponse
    {
        public int JustificationID { get; set; }
        public string ProductCode { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Message { get; set; }
    }
}
