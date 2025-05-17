using System;

namespace ProdFlow.DTOs
{
    public class FlanPartieDto
    {
        public string CodePartie { get; set; }
        public string PtNumOriginal { get; set; }
        public int NumeroPartie { get; set; }
        public string Label { get; set; }
        public DateTime? DateCreation { get; set; }
    }
}