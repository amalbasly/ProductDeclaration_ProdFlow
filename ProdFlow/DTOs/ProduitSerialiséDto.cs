using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.DTOs
{
    public class ProduitSerialiséDto
    {
        [Required(ErrorMessage = "Le code ligne est requis")]
        [StringLength(2, ErrorMessage = "Le code ligne ne peut dépasser 2 caractères")]
        [Display(Name = "Ligne")]
        public string LpNum { get; set; }

        [Required(ErrorMessage = "La famille est requise")]
        [StringLength(10, ErrorMessage = "La famille ne peut dépasser 10 caractères")]
        [Display(Name = "Famille")]
        public string FpCod { get; set; }

        [Required(ErrorMessage = "La sous-famille est requise")]
        [StringLength(25, ErrorMessage = "La sous-famille ne peut dépasser 25 caractères")]
        [Display(Name = "Sous-Famille")]
        public string SpCod { get; set; }

        [Required(ErrorMessage = "Le code produit est requis")]
        [StringLength(18, ErrorMessage = "Le code produit ne peut dépasser 18 caractères")]
        [Display(Name = "Code Produit")]
        public string PtNum { get; set; }

        [Required(ErrorMessage = "Le libellé est requis")]
        [StringLength(96, ErrorMessage = "Le libellé ne peut dépasser 96 caractères")]
        [Display(Name = "Libellé")]
        public string PtLib { get; set; }

        [StringLength(96, ErrorMessage = "Le libellé 2 ne peut dépasser 96 caractères")]
        [Display(Name = "Libellé Complémentaire")]
        public string PtLib2 { get; set; }

        [StringLength(10, ErrorMessage = "Le type ne peut dépasser 10 caractères")]
        [Display(Name = "Type")]
        public string TpCod { get; set; }

        [StringLength(6, ErrorMessage = "Le statut ne peut dépasser 6 caractères")]
        [Display(Name = "Statut")]
        public string SpId { get; set; }

        [StringLength(50, ErrorMessage = "Le code client ne peut dépasser 50 caractères")]
        [Display(Name = "Code Client (C264)")]
        public string PtSpecifT14 { get; set; }

        [Display(Name = "Poids (kg)")]
        [Range(0, double.MaxValue, ErrorMessage = "Le poids doit être positif")]
        public double? PtPoids { get; set; }

        [StringLength(12, ErrorMessage = "Le créateur ne peut dépasser 12 caractères")]
        [Display(Name = "Créé par")]
        public string PtCreateur { get; set; }

        [Display(Name = "Date Création")]
        [DataType(DataType.DateTime)]
        public DateTime? PtDcreat { get; set; }

        [Display(Name = "Tolerance")]
        [Column("pt_specifT15")]
        [StringLength(50)]
        public string PtSpecifT15 { get; set; }

        public byte? PtFlasher { get; set; }

        [Display(Name = "Création")]
        public string CreationComplete =>
            PtCreateur != null && PtDcreat.HasValue
                ? $"{PtCreateur} - {PtDcreat.Value:dd/MM/yyyy HH:mm}"
                : "Non spécifié";

        [Display(Name = "Flashable")]
        public bool EstFlashable => PtFlasher == 1;

        [Display(Name = "Produit Serialisé")]
        public bool IsSerialized { get; set; } = false;

        public int? GalliaId { get; set; }

        public string GalliaName { get; set; }

        public DateTime? PtVerificationDeadline { get; set; }

        // Justification-related fields
        public string JustificationStatus { get; set; }

        public int JustificationID { get; set; }

        public string JustificationText { get; set; }

        public string UrgencyLevel { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public string DecisionComments { get; set; }

        public DateTime? DecisionDate { get; set; }

        public string DecidedBy { get; set; }
        public bool IsApproved { get; set; }
    }
}
