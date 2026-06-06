using System.ComponentModel.DataAnnotations;

using CrmApp.Models.ViewModels;

namespace CrmApp.Tests.Models;

public class PersonFormViewModelTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static ClientFormViewModel CreateValidViewModel() => new()
    {
        Id = 1,
        FirstName = "Jan",
        LastName = "Novák",
        BirthNumber = "960101/1234"
    };

    [Theory]
    [InlineData("960101/1234")]
    [InlineData("000101/0000")]
    public void Validate_ShouldPass_WhenPersonIsAdult(string birthNumber)
    {
        // Arrange
        var vm = CreateValidViewModel();
        vm.BirthNumber = birthNumber;

        // Act
        var results = Validate(vm);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPersonIsUnder18()
    {
        // Arrange
        var vm = CreateValidViewModel();

        var targetDate = DateTime.Today.AddYears(-17);
        vm.BirthNumber = $"{targetDate:yyMMdd}/1234";

        // Act
        var results = Validate(vm);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(PersonFormViewModel.BirthNumber)));
        Assert.Contains(results, r => r.ErrorMessage!.Contains("18 let"));
    }

    [Theory]
    [InlineData("961301/1234")]
    [InlineData("960132/1234")]
    public void Validate_ShouldFail_WhenBirthNumberHasInvalidDate(string birthNumber)
    {
        // Arrange
        var vm = CreateValidViewModel();
        vm.BirthNumber = birthNumber;

        // Act
        var results = Validate(vm);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(PersonFormViewModel.BirthNumber)));
        Assert.Contains(results, r => r.ErrorMessage!.Contains("datum narození"));
    }
}
