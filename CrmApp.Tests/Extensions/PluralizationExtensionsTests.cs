using CrmApp.Web.Extensions;

namespace CrmApp.Tests.Extensions;

public class PluralizationExtensionsTests
{
    [Theory]
    [InlineData(0, "smluv")]
    [InlineData(1, "smlouvu")]
    [InlineData(2, "smlouvy")]
    [InlineData(3, "smlouvy")]
    [InlineData(4, "smlouvy")]
    [InlineData(5, "smluv")]
    [InlineData(10, "smluv")]
    [InlineData(100, "smluv")]
    public void PluralizeContracts_ShouldReturnCorrectForm_ForGivenCount(int count, string expected)
    {
        // Act
        var result = count.PluralizeContracts();

        // Assert
        Assert.Equal(expected, result);
    }
}
