using System.ComponentModel.DataAnnotations;
using System;


namespace ProdFlow.DTOs
{
    public class CreateGalliaDto
    {
        [StringLength(50)]
        public string? PLIB1 { get; set; }

        [StringLength(50)]
        public string? QLIB3 { get; set; }

        [StringLength(100)]
        public string? LIB1 { get; set; }

        [StringLength(100)]
        public string? LIB2 { get; set; }

        [StringLength(100)]
        public string? LIB3 { get; set; }

        [StringLength(100)]
        public string? LIB4 { get; set; }

        [StringLength(100)]
        public string? LIB5 { get; set; }

        [StringLength(100)]
        public string? LIB6 { get; set; }

        [StringLength(100)]
        public string? LIB7 { get; set; }

        [StringLength(100)]
        public string? SupplierName { get; set; }

        public DateTime? LabelDate { get; set; }
    }
}
