using System.Text;

using CrmApp.Web.Extensions;

namespace CrmApp.Tests.Extensions;

public class CsvExtensionsTests
{
    [Fact]
    public void GetCsvBytes_ShouldPrependUtf8BomAndKeepContent()
    {
        // Arrange
        var testString = "Příliš žluťoučký kůň úpěl ďábelské ódy.";
        var sb = new StringBuilder(testString);

        var expectedBom = Encoding.UTF8.GetPreamble();
        var expectedContentBytes = Encoding.UTF8.GetBytes(testString);

        // Act
        var result = sb.GetCsvBytes();

        // Assert
        Assert.Equal(expectedBom.Length + expectedContentBytes.Length, result.Length);

        var actualBom = result.Take(3).ToArray();
        Assert.Equal(expectedBom, actualBom);

        var actualContent = result.Skip(3).ToArray();
        Assert.Equal(expectedContentBytes, actualContent);
    }

    [Theory]
    [InlineData("Běžný text", "Běžný text")]
    [InlineData("=SUM(1+1)", "'=SUM(1+1)")]
    [InlineData("+420123456", "'+420123456")]
    [InlineData("-1000", "'-1000")]
    [InlineData("@Pavel", "'@Pavel")]
    [InlineData("Jan;Pavel", "\"Jan;Pavel\"")]
    [InlineData("Text s \"uvozovkou\"", "\"Text s \"\"uvozovkou\"\"\"")]
    [InlineData(null, "")]
    [InlineData("", "")]
    public void EscapeCsv_ShouldFormatAndProtectDataCorrectly(string? input, string expectedOutput)
    {
        // Act
        var result = input.EscapeCsv();

        // Assert
        Assert.Equal(expectedOutput, result);
    }
}
