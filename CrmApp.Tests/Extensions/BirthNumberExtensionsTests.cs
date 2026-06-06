using CrmApp.Extensions;

namespace CrmApp.Tests.Extensions;

public class BirthNumberExtensionsTests
{
    [Theory]
    [InlineData("960101/1234", 1996, 1, 1)]
    [InlineData("965101/1234", 1996, 1, 1)]
    [InlineData("050228/1234", 2005, 2, 28)]
    [InlineData("200101/123", 1920, 1, 1)]
    [InlineData("460515/1234", 1946, 5, 15)]
    [InlineData("000101/0000", 2000, 1, 1)]
    [InlineData("052228/1234", 2005, 2, 28)]
    [InlineData("057228/1234", 2005, 2, 28)]
    public void GetDateOfBirth_ShouldParseCorrectly_WhenFormatIsValid(string rc, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Act
        var dob = rc.GetDateOfBirth();

        // Assert
        Assert.NotNull(dob);
        Assert.Equal(expectedYear, dob.Value.Year);
        Assert.Equal(expectedMonth, dob.Value.Month);
        Assert.Equal(expectedDay, dob.Value.Day);
    }

    [Theory]
    [InlineData("text")]
    [InlineData("9601011234")]
    [InlineData("961301/1234")]
    [InlineData("960132/1234")]
    [InlineData("990229/1234")]
    public void GetDateOfBirth_ShouldReturnNull_WhenFormatIsInvalid(string rc)
    {
        // Act
        var dob = rc.GetDateOfBirth();

        // Assert
        Assert.Null(dob);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 19)]
    [InlineData(-1, 20)]
    public void GetAge_ShouldCalculateCorrectly_AroundBirthday(int daysOffset, int expectedAge)
    {
        // Arrange
        var targetDate = DateTime.Today.AddDays(daysOffset).AddYears(-20);
        var rc = $"{targetDate:yyMMdd}/1234";

        // Act
        var age = rc.GetAge();

        // Assert
        Assert.Equal(expectedAge, age);
    }
}
