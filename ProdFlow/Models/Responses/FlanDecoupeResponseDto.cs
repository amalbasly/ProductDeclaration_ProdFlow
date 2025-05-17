using System;
using System.Collections.Generic;
using ProdFlow.DTOs;

namespace ProdFlow.Models.Responses
{
    public class FlanDecoupeResponseDto
    {
        public bool Success { get; set; }
        public int IdDecoupe { get; set; }
        public string PtNumOriginal { get; set; }
        public int NombreDeParts { get; set; }
        public string LabelUtilise { get; set; }
        public DateTime? DateDecoupe { get; set; }
        public string Utilisateur { get; set; }
        public int PartsCount { get; set; }
        public List<FlanPartieDto> Parts { get; set; } = new List<FlanPartieDto>();
    }
}