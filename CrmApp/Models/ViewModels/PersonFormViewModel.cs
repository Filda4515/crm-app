using System.ComponentModel.DataAnnotations;

using CrmApp.Extensions;

namespace CrmApp.Models.ViewModels;

public abstract class PersonFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Jméno je povinné.")]
    [MaxLength(50, ErrorMessage = "Jméno může mít maximálně 50 znaků.")]
    [Display(Name = "Jméno")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Příjmení je povinné.")]
    [MaxLength(50, ErrorMessage = "Příjmení může mít maximálně 50 znaků.")]
    [Display(Name = "Příjmení")]
    public required string LastName { get; set; }

    [MaxLength(100, ErrorMessage = "E-mail je příliš dlouhý.")]
    [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "Telefon je příliš dlouhý.")]
    [Phone(ErrorMessage = "Neplatný formát telefonu.")]
    [Display(Name = "Telefonní číslo")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Rodné číslo je povinné.")]
    [RegularExpression(@"^\d{6}\/\d{3,4}$", ErrorMessage = "Rodné číslo musí být ve formátu YYMMDD/XXXX.")]
    [Display(Name = "Rodné číslo")]
    public required string BirthNumber { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var dob = BirthNumber?.GetDateOfBirth();
        if (dob == null)
        {
            yield return new ValidationResult("Rodné číslo neobsahuje platné datum narození.", [nameof(BirthNumber)]);
            yield break;
        }

        var age = BirthNumber!.GetAge();
        if (age < 18)
        {
            yield return new ValidationResult("Osoba musí být starší 18 let.", [nameof(BirthNumber)]);
        }
    }
}

public class ClientFormViewModel : PersonFormViewModel
{

}

public class AdvisorFormViewModel : PersonFormViewModel
{

}
