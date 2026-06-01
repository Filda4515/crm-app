using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models;

public abstract class Person : IValidatableObject
{
    [Key]
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
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "Telefon je příliš dlouhý.")]
    [Phone(ErrorMessage = "Neplatný formát telefonu.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Rodné číslo je povinné.")]
    [RegularExpression(@"^\d{6}\/\d{3,4}$", ErrorMessage = "Rodné číslo musí být ve formátu YYMMDD/XXXX.")]
    [Display(Name = "Rodné číslo")]
    public required string BirthNumber { get; set; }

    [Required(ErrorMessage = "Věk je povinný.")]
    [Range(18, 150, ErrorMessage = "Věk musí být minimálně 18 let.")]
    public int Age { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        int slashIndex = BirthNumber.IndexOf('/');
        int yy = int.Parse(BirthNumber.AsSpan(0, 2));
        int mm = int.Parse(BirthNumber.AsSpan(2, 2));
        int dd = int.Parse(BirthNumber.AsSpan(4, 2));

        string suffix = BirthNumber[(slashIndex + 1)..];
        int century = suffix.Length == 3 ? 1900 : yy >= 54 ? 1900 : 2000;
        int fullYear = century + yy;

        if (mm > 50) mm -= 50;

        if (!DateTime.TryParseExact($"{fullYear:D4}{mm:D2}{dd:D2}", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dob))
        {
            yield return new ValidationResult("Rodné číslo neobsahuje platné datum narození.", [nameof(BirthNumber)]);
            yield break;
        }

        int expectedAge = DateTime.Today.Year - dob.Year;
        if (DateTime.Today < dob.AddYears(expectedAge))
            expectedAge--;

        if (expectedAge != Age)
            yield return new ValidationResult($"Zadaný věk neodpovídá rodnému číslu. Očekávaný věk je {expectedAge}.", [nameof(Age)]);
    }
}
