using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Jméno je povinné.")]
        [Display(Name = "Jméno")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [Display(Name = "Příjmení")]
        public required string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Neplatný formát telefonu.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Rodné číslo je povinné.")]
        [Display(Name = "Rodné číslo")]
        public required string BirthNumber { get; set; }

        [Required(ErrorMessage = "Věk je povinný.")]
        [Range(18, 150, ErrorMessage = "Věk musí být minimálně 18 let.")]
        public int Age { get; set; }
    }
}
