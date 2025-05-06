using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("Gallia")] // ← Fix attribute casing
    public class Gallia
    {
        [Key]
        public int GalliaId { get; set; }

        [MaxLength(50)]
        public string? PLIB1 { get; set; }

        [MaxLength(50)]
        public string? QLIB3 { get; set; }

        [MaxLength(100)]
        public string? LIB1 { get; set; }

        [MaxLength(100)]
        public string? LIB2 { get; set; }

        [MaxLength(100)]
        public string? LIB3 { get; set; }

        [MaxLength(100)]
        public string? LIB4 { get; set; }

        [MaxLength(100)]
        public string? LIB5 { get; set; }

        [MaxLength(100)]
        public string? LIB6 { get; set; }

        [MaxLength(100)]
        public string? LIB7 { get; set; }

        [MaxLength(100)]
        public string? SupplierName { get; set; }

        public DateTime? LabelDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}
