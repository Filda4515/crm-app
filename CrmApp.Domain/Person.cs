using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CrmApp.Domain.Extensions;

namespace CrmApp.Domain.Models;

public abstract class Person
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Jméno")]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Příjmení")]
    public required string LastName { get; set; }

    [MaxLength(100)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [MaxLength(20)]
    [Display(Name = "Telefonní číslo")]
    public string? Phone { get; set; }

    [Required]
    [MaxLength(11)]
    [Display(Name = "Rodné číslo")]
    public required string BirthNumber { get; set; }

    [Display(Name = "Věk")]
    [NotMapped]
    public int Age => BirthNumber?.GetAge() ?? 0;
}
