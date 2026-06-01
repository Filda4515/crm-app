using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Evidenční číslo je povinné.")]
        [Display(Name = "Evidenční číslo")]
        public required string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Instituce je povinná.")]
        public required string Institution { get; set; }

        [Required(ErrorMessage = "Smlouva musí mít přiřazeného klienta.")]
        public required int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required(ErrorMessage = "Správce smlouvy je povinný.")]
        public required int ManagerId { get; set; }
        public Advisor? Manager { get; set; }

        [Required]
        [Display(Name = "Datum uzavření")]
        public DateTime SignedDate { get; set; }

        [Required]
        [Display(Name = "Datum platnosti")]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Datum ukončení")]
        public DateTime? EndDate { get; set; }

        public ICollection<Advisor> Participants { get; set; } = [];
    }
}
