using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProdFlow.Models.Entities
{
    
    [Table("Groupe")]
    public class Groupe
    {
        [Key]
        public int IDGrp { get; set; }

        [StringLength(250)]
        public string? descriptionGrp { get; set; }

        public ICollection<PersonnelTraca>? Employees { get; set; }
    }
}
