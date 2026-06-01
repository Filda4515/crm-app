using System.ComponentModel.DataAnnotations;

using CrmApp.Models;

namespace CrmApp.Tests.Models;

public class PersonTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static Client CreateValidClient() => new()
    {
        Id = 1,
        FirstName = "Jan",
        LastName = "Novák",
        BirthNumber = "960101/1234",
        Age = 30
    };

    [Theory]
    [InlineData("530101/123", 73)]
    [InlineData("960101/1234", 30)]
    [InlineData("965101/1234", 30)]
    [InlineData("961231/1234", 29)]
    [InlineData("050101/1234", 21)]
    public void Validate_ShouldPass_WhenAgeMatchesBirthNumber(string birthNumber, int age)
    {
        // Arrange
        var client = CreateValidClient();
        client.BirthNumber = birthNumber;
        client.Age = age;

        // Act
        var results = Validate(client);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData("960101/1234", 25)]
    [InlineData("960101/1234", 35)]
    public void Validate_ShouldFail_WhenAgeMismatchesBirthNumber(string birthNumber, int age)
    {
        // Arrange
        var client = CreateValidClient();
        client.BirthNumber = birthNumber;
        client.Age = age;

        // Act
        var results = Validate(client);

        // Assert
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Person.Age)));
    }

    [Theory]
    [InlineData("961301/1234")]
    [InlineData("960132/1234")]
    public void Validate_ShouldFail_WhenBirthNumberHasInvalidDate(string birthNumber)
    {
        // Arrange
        var client = CreateValidClient();
        client.BirthNumber = birthNumber;

        // Act
        var results = Validate(client);

        // Assert
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Person.BirthNumber)));
    }
}
