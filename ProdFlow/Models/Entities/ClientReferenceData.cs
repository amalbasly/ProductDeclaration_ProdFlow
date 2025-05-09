using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("client_reference_data")]
    public class ClientReferenceData
    {
        [Key]
        [Column("pt_num")]
        [StringLength(18)]
        public string PtNum { get; set; }

        [Required]
        [Column("client_reference")]
        [StringLength(255)]
        public string ClientReference { get; set; }

        [Required]
        [Column("client_index")]
        [StringLength(255)]
        public string ClientIndex { get; set; }

        [Required]
        [Column("client")]
        [StringLength(255)]
        public string Client { get; set; }

        [Required]
        [Column("referentiel")]
        [StringLength(255)]
        public string Referentiel { get; set; }

        // Navigation property
        [ForeignKey("PtNum")]
        public virtual Produit Produit { get; set; }
    }
}