using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProdFlow.Models.Entities
{
    [Table("produit")]
    public class Produit
    {
        [Key]
        [Column("pt_num")]
        [StringLength(18)]
        public string PtNum { get; set; }

        [Column("pt_idplan")]
        [StringLength(6)]
        public string PtIdplan { get; set; }

        [Column("pt_lib")]
        [StringLength(96)]
        [Required]
        public string PtLib { get; set; }

        [Column("pt_lib2")]
        [StringLength(96)]
        public string PtLib2 { get; set; }

        [Column("lp_num")]
        [StringLength(2)]
        [Required]
        public string LpNum { get; set; }

        [Column("fp_cod")]
        [StringLength(10)]
        [Required]
        public string FpCod { get; set; }

        [Column("sp_cod")]
        [StringLength(25)]
        [Required]
        public string SpCod { get; set; }

        [Column("pt_poids")]
        public double? PtPoids { get; set; }

        [Column("pt_codval")]
        public int? PtCodval { get; set; }

        [Column("pt_coeffinc")]
        public double? PtCoeffinc { get; set; }

        [Column("pt_coutsup")]
        public double? PtCoutsup { get; set; }

        [Column("pt_coutva")]
        public double? PtCoutva { get; set; }

        [Column("pt_createur")]
        [StringLength(12)]
        public string PtCreateur { get; set; }

        [Column("pt_crebus")]
        public int? PtCrebus { get; set; }

        [Column("pt_dcreat")]
        public DateTime? PtDcreat { get; set; }

        [Column("pt_devis")]
        [StringLength(2)]
        public string PtDevis { get; set; }

        [Column("pt_dladmin")]
        public int? PtDladmin { get; set; }

        [Column("pt_dlalloc")]
        public int? PtDlalloc { get; set; }

        [Column("pt_dlctrl")]
        public int? PtDlctrl { get; set; }

        [Column("pt_dlfournis")]
        public int? PtDlfournis { get; set; }

        [Column("pt_dlobt")]
        public int? PtDlobt { get; set; }

        [Column("pt_dlprerupt")]
        public int? PtDlprerupt { get; set; }

        [Column("pt_dlsecu")]
        public int? PtDlsecu { get; set; }

        [Column("pt_empmag")]
        [StringLength(18)]
        public string PtEmpmag { get; set; }

        [Column("pt_empprinc")]
        [StringLength(8)]
        public string PtEmpprinc { get; set; }

        [Column("pt_etatart")]
        [StringLength(2)]
        public string PtEtatart { get; set; }

        [Column("pt_fantome")]
        public byte? PtFantome { get; set; }

        [Column("pt_ipde")]
        [StringLength(12)]
        public string PtIpde { get; set; }

        [Column("pt_ipdf")]
        [StringLength(12)]
        public string PtIpdf { get; set; }

        [Column("pt_kconv")]
        public double? PtKconv { get; set; }

        [Column("pt_kperte")]
        public int? PtKperte { get; set; }

        [Column("pt_magasin")]
        [StringLength(2)]
        public string PtMagasin { get; set; }

        [Column("pt_mappro")]
        [StringLength(2)]
        public string PtMappro { get; set; }

        [Column("pt_nature")]
        [StringLength(2)]
        public string PtNature { get; set; }

        [Column("pt_pdp")]
        public bool? PtPdp { get; set; }

        [Column("pt_pmp")]
        public double? PtPmp { get; set; }

        [Column("pt_pua")]
        public double? PtPua { get; set; }

        [Column("pt_puv")]
        public double? PtPuv { get; set; }

        [Column("pt_qbes6mois")]
        public double? PtQbes6mois { get; set; }

        [Column("pt_qcde")]
        public double? PtQcde { get; set; }

        [Column("pt_qcons")]
        public double? PtQcons { get; set; }

        [Column("pt_qconsval")]
        public double? PtQconsval { get; set; }

        [Column("pt_qmultcde")]
        public double? PtQmultcde { get; set; }

        [Column("pt_qpointcde")]
        public double? PtQpointcde { get; set; }

        [Column("pt_reffabr")]
        [StringLength(255)]
        public string PtReffabr { get; set; }

        [Column("pt_reglapp")]
        [StringLength(2)]
        public string PtReglapp { get; set; }

        [Column("pt_stkinv")]
        public double? PtStkinv { get; set; }

        [Column("pt_stksecu")]
        public double? PtStksecu { get; set; }

        [Column("pt_stock")]
        public double? PtStock { get; set; }

        [Column("pt_tps")]
        public double? PtTps { get; set; }

        [Column("pt_ua")]
        public int? PtUa { get; set; }

        [Column("pt_um")]
        [StringLength(5)]
        public string PtUm { get; set; }

        [Column("sl_num")]
        [StringLength(4)]
        public string SlNum { get; set; }

        [Column("ce_num")]
        public int? CeNum { get; set; }

        [Column("cr_num")]
        public int? CrNum { get; set; }

        [Column("fs_nentr")]
        [StringLength(6)]
        public string FsNentr { get; set; }

        [Column("fs_netabl")]
        [StringLength(3)]
        public string FsNetabl { get; set; }

        [Column("pt_catabc")]
        [StringLength(100)]
        public string PtCatabc { get; set; }

        [Column("pt_codsag")]
        [StringLength(14)]
        public string PtCodsag { get; set; }

        [Column("tp_cod")]
        [StringLength(10)]
        public string TpCod { get; set; }

        [Column("tr_id")]
        [StringLength(3)]
        public string TrId { get; set; }

        [Column("pt_numSWAP")]
        [StringLength(18)]
        public string PtNumSWAP { get; set; }

        [Column("pt_obs")]
        [StringLength(255)]
        public string PtObs { get; set; }

        [Column("pt_groupeExpertise")]
        [StringLength(50)]
        public string PtGroupeExpertise { get; set; }

        [Column("pt_agrement")]
        [StringLength(30)]
        public string PtAgrement { get; set; }

        [Column("pt_codeFrs")]
        [StringLength(15)]
        public string PtCodeFrs { get; set; }

        [Column("ct_num")]
        [StringLength(15)]
        public string CtNum { get; set; }

        [Column("ne_cod")]
        [StringLength(20)]
        public string NeCod { get; set; }

        [Column("sp_Id")]
        [StringLength(6)]
        public string SpId { get; set; }

        [Column("or_cod")]
        [StringLength(2)]
        public string OrCod { get; set; }

        [Column("mp_cod")]
        [StringLength(10)]
        public string MpCod { get; set; }

        [Column("pt_specifN01")]
        public decimal? PtSpecifN01 { get; set; }

        [Column("pt_specifN02")]
        public decimal? PtSpecifN02 { get; set; }

        [Column("pt_specifN03")]
        public decimal? PtSpecifN03 { get; set; }

        [Column("pt_specifN04")]
        public decimal? PtSpecifN04 { get; set; }

        [Column("pt_specifN05")]
        public decimal? PtSpecifN05 { get; set; }

        [Column("pt_specifN06")]
        public decimal? PtSpecifN06 { get; set; }

        [Column("pt_specifN07")]
        public decimal? PtSpecifN07 { get; set; }

        [Column("pt_specifN08")]
        public decimal? PtSpecifN08 { get; set; }

        [Column("pt_specifN09")]
        public decimal? PtSpecifN09 { get; set; }

        [Column("pt_specifN10")]
        public decimal? PtSpecifN10 { get; set; }

        [Column("pt_specifT01")]
        [StringLength(50)]
        public string PtSpecifT01 { get; set; }

        [Column("pt_specifT02")]
        [StringLength(50)]
        public string PtSpecifT02 { get; set; }

        [Column("pt_specifT03")]
        [StringLength(50)]
        public string PtSpecifT03 { get; set; }

        [Column("pt_specifT04")]
        [StringLength(50)]
        public string PtSpecifT04 { get; set; }

        [Column("pt_specifT05")]
        [StringLength(50)]
        public string PtSpecifT05 { get; set; }

        [Column("pt_specifT06")]
        [StringLength(255)]
        public string PtSpecifT06 { get; set; }

        [Column("pt_specifT07")]
        [StringLength(255)]
        public string PtSpecifT07 { get; set; }

        [Column("pt_specifT08")]
        [StringLength(255)]
        public string PtSpecifT08 { get; set; }

        [Column("pt_specifT09")]
        [StringLength(255)]
        public string PtSpecifT09 { get; set; }

        [Column("pt_specifT10")]
        [StringLength(255)]
        public string PtSpecifT10 { get; set; }

        [Column("pt_importNomenc_Statut")]
        [StringLength(5)]
        public string PtImportNomencStatut { get; set; }

        [Column("pt_importNomenc_Date")]
        public DateTime? PtImportNomencDate { get; set; }

        [Column("pt_lieuFab")]
        [StringLength(2)]
        public string PtLieuFab { get; set; }

        [Column("pt_flasher")]
        public byte? PtFlasher { get; set; }

        [Column("pt_specifN11")]
        public decimal? PtSpecifN11 { get; set; }

        [Column("pt_specifN12")]
        public decimal? PtSpecifN12 { get; set; }

        [Column("pt_specifT11")]
        [StringLength(255)]
        public string PtSpecifT11 { get; set; }

        [Column("pt_specifT12")]
        [StringLength(50)]
        public string PtSpecifT12 { get; set; }

        [Column("pt_specifN13")]
        public decimal? PtSpecifN13 { get; set; }

        [Column("pt_specifT13")]
        [StringLength(50)]
        public string PtSpecifT13 { get; set; }

        [Column("pt_specifN14")]
        public decimal? PtSpecifN14 { get; set; }

        [Column("pt_specifT14")]
        [StringLength(50)]
        public string PtSpecifT14 { get; set; }

        [Column("pt_specifN15")]
        public decimal? PtSpecifN15 { get; set; }

        [Column("pt_specifT15")]
        [StringLength(50)]
        public string PtSpecifT15 { get; set; }

        [Column("dn_code")]
        [StringLength(4)]
        public string DnCode { get; set; }

        [Column("mn_cod")]
        [StringLength(4)]
        public string MnCod { get; set; }

        [Column("tt_trans")]
        [StringLength(4)]
        public string TtTrans { get; set; }

        [Column("VersionFab")]
        [StringLength(4)]
        public string VersionFab { get; set; }

        [Column("Pt_SpecifLazer")]
        [StringLength(10)]
        public string PtSpecifLazer { get; set; }

        [Column("pt_specifN16")]
        public decimal? PtSpecifN16 { get; set; }

        [Column("pt_specifT16")]
        public decimal? PtSpecifT16 { get; set; }

        [Column("pt_specifT17")]
        public decimal? PtSpecifT17 { get; set; }

        [Column("GalliaId")]
        public int? GalliaId { get; set; }

        [Column("pt_verification_deadline")]
        public DateTime? PtVerificationDeadline { get; set; } // Added for verification deadline

        [ForeignKey("GalliaId")]
        public Gallia Gallia { get; set; }

        public List<Justification> Justifications { get; set; }
        public List<SynoptiqueProd> SynoptiqueProds { get; set; }
        public ClientReferenceData ClientReferenceData { get; set; }
    }
}