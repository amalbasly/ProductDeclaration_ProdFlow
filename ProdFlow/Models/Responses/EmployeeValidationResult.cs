namespace ProdFlow.Models.Responses
{
    public class EmployeeValidationResult
    {
        public int AccessStatus { get; set; }
        public string? EmployeeFunction { get; set; }
        public string? pl_nom { get; set; }
        public string? pl_prenom { get; set; }

        public string? EmployeeEmail { get; set; }
    }
}

