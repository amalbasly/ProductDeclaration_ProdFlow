namespace ProdFlow.Models.Responses
{
    public class SynoptiqueSaveResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ProductCode { get; set; }
        public int DeletedEntries { get; set; }
        public int InsertedEntries { get; set; }
    }
}
