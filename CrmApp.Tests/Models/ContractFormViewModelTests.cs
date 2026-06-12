using System.ComponentModel.DataAnnotations;

using CrmApp.Domain.Models;
using CrmApp.Web.Models.ViewModels;

namespace CrmApp.Tests.Models;

public class ContractFormViewModelTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    [Theory]
    [InlineData("2026-06-01", "2026-05-31", false)]
    [InlineData("2026-06-01", "2026-06-01", true)]
    [InlineData("2026-06-01", "2026-06-02", true)]
    public void EffectiveDate_Validation(string signed, string effective, bool expectedIsValid)
    {
        // Arrange
        var viewModel = new ContractFormViewModel
        {
            RegistrationNumber = "12345",
            Institution = "ČSOB",
            ClientId = 1,
            ManagerId = 1,
            ParticipantIds = [1],
            SignedDate = DateTime.Parse(signed),
            EffectiveDate = DateTime.Parse(effective)
        };

        // Act
        var results = Validate(viewModel);

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
    public void EndDate_Validation_ShouldFail_WhenEndDateIsBeforeEffectiveDate()
    {
        // Arrange
        var viewModel = new ContractFormViewModel
        {
            RegistrationNumber = "12345",
            Institution = "ČSOB",
            ClientId = 1,
            ManagerId = 1,
            ParticipantIds = [1],
            SignedDate = new DateTime(2026, 6, 1),
            EffectiveDate = new DateTime(2026, 6, 10),
            EndDate = new DateTime(2026, 6, 5)
        };

        // Act
        var results = Validate(viewModel);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ContractFormViewModel.EndDate)));
    }

    [Fact]
    public void Participants_Validation_ShouldFail_WhenListIsEmpty()
    {
        // Arrange
        var viewModel = new ContractFormViewModel
        {
            RegistrationNumber = "12345",
            Institution = "Axa",
            ClientId = 1,
            ManagerId = 1,
            ParticipantIds = [],
            SignedDate = new DateTime(2026, 1, 1),
            EffectiveDate = new DateTime(2026, 1, 2),
        };

        // Act
        var results = Validate(viewModel);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ContractFormViewModel.ParticipantIds)));
    }

    [Fact]
    public void Manager_Validation_ShouldFail_WhenNotInParticipants()
    {
        // Arrange
        var viewModel = new ContractFormViewModel
        {
            RegistrationNumber = "12345",
            Institution = "Axa",
            ClientId = 1,
            ManagerId = 1,
            ParticipantIds = [99],
            SignedDate = new DateTime(2026, 1, 1),
            EffectiveDate = new DateTime(2026, 1, 2)
        };

        // Act
        var results = Validate(viewModel);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ContractFormViewModel.ParticipantIds)));
    }
}
