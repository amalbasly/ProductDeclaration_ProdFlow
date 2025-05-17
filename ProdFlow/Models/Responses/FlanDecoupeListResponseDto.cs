using System.Collections.Generic;

namespace ProdFlow.Models.Responses
{
    public class FlanDecoupeListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<FlanDecoupeResponseDto> FlanDecoupes { get; set; } = new List<FlanDecoupeResponseDto>();
    }
}