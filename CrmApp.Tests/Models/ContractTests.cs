using System.ComponentModel.DataAnnotations;

using CrmApp.Models;

namespace CrmApp.Tests.Models;

public class ContractTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static Advisor CreateValidAdvisor(int id) => new()
    {
        Id = id,
        FirstName = "Jan",
        LastName = "Novák",
        BirthNumber = "960101/1234",
        Age = 30
    };

    [Theory]
    [InlineData("2026-06-01", "2026-05-31", false)]
    [InlineData("2026-06-01", "2026-06-01", true)]
    [InlineData("2026-06-01", "2026-06-02", true)]
    public void EffectiveDate_Validation(string signed, string effective, bool expectedIsValid)
    {
        // Arrange
        var manager = CreateValidAdvisor(1);
        var contract = new Contract
        {
            RegistrationNumber = "12345",
            Institution = "ČSOB",
            ClientId = 1,
            ManagerId = manager.Id,
            SignedDate = DateTime.Parse(signed),
            EffectiveDate = DateTime.Parse(effective)
        };
        contract.Participants.Add(manager);

        // Act
        var results = Validate(contract);

        // Assert
        if (expectedIsValid)
        {
            Assert.Empty(results);
        }
        else
        {
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Contract.EffectiveDate)));
        }
    }

    [Fact]
    public void Participants_Validation_ShouldFail_WhenListIsEmpty()
    {
        // Arrange
        var contract = new Contract
        {
            RegistrationNumber = "12345",
            Institution = "Axa",
            ClientId = 1,
            ManagerId = 1,
            SignedDate = new DateTime(2026, 1, 1),
            EffectiveDate = new DateTime(2026, 1, 2)
        };
        // Act
        var results = Validate(contract);

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void Manager_Validation_ShouldFail_WhenNotInParticipants()
    {
        // Arrange
        var contract = new Contract
        {
            RegistrationNumber = "12345",
            Institution = "Axa",
            ClientId = 1,
            ManagerId = 1,
            SignedDate = new DateTime(2026, 1, 1),
            EffectiveDate = new DateTime(2026, 1, 2)
        };

        contract.Participants.Add(CreateValidAdvisor(99));

        // Act
        var results = Validate(contract);

        // Assert
        Assert.NotEmpty(results);
    }
}
