using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models;

public class Contract : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Evidenční číslo je povinné.")]
    [MaxLength(50, ErrorMessage = "Evidenční číslo může mít maximálně 50 znaků.")]
    [Display(Name = "Evidenční číslo")]
    public required string RegistrationNumber { get; set; }

    [Required(ErrorMessage = "Instituce je povinná.")]
    [MaxLength(100, ErrorMessage = "Název instituce může mít maximálně 100 znaků.")]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EffectiveDate.Date < SignedDate.Date)
        {
            yield return new ValidationResult("Datum platnosti nesmí předcházet datu uzavření.", [nameof(EffectiveDate)]);
        }

        if (Participants.Count == 0)
        {
            yield return new ValidationResult("Smlouva musí mít minimálně jednoho účastníka.", [nameof(Participants)]);
            yield break;
        }
        else if (!Participants.Any(p => p.Id == ManagerId))
        {
            yield return new ValidationResult("Správce smlouvy musí být zároveň jedním z jejích účastníků.", [nameof(Participants)]);
        }
    }
}
