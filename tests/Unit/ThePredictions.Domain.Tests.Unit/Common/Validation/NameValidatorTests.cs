using AwesomeAssertions;
using ThePredictions.Domain.Common.Validation;
using Xunit;

namespace ThePredictions.Domain.Tests.Unit.Common.Validation;

public class NameValidatorTests
{
    #region IsValid — Valid Names

    [Theory]
    [InlineData("John")]
    [InlineData("John Smith")]
    [InlineData("O'Brien")]
    [InlineData("Mary-Jane")]
    [InlineData("Dr. Smith")]
    [InlineData("José García")]
    [InlineData("naïve")]
    [InlineData("王明")]
    [InlineData("محمد")]
    public void IsValid_ShouldReturnTrue_WhenNameContainsAllowedCharacters(string name)
    {
        // Act
        var result = NameValidator.IsValid(name);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenNameIsNull()
    {
        // Act
        var result = NameValidator.IsValid(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenNameIsEmpty()
    {
        // Act
        var result = NameValidator.IsValid("");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenNameIsWhitespace()
    {
        // Act
        var result = NameValidator.IsValid(" ");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region IsValid — Invalid Names

    [Theory]
    [InlineData("John123")]
    [InlineData("<script>")]
    [InlineData("Tom & Jerry")]
    [InlineData("John 😀")]
    [InlineData("user@name")]
    [InlineData("Name#1")]
    [InlineData("(John)")]
    [InlineData("[John]")]
    [InlineData("John/Smith")]
    [InlineData("John\\Smith")]
    [InlineData("John!")]
    [InlineData("12345")]
    public void IsValid_ShouldReturnFalse_WhenNameContainsBlockedCharacters(string name)
    {
        // Act
        var result = NameValidator.IsValid(name);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Sanitize — Preservation

    [Fact]
    public void Sanitize_ShouldReturnSameName_WhenNameIsValid()
    {
        // Act
        var result = NameValidator.Sanitize("John Smith");

        // Assert
        result.Should().Be("John Smith");
    }

    [Fact]
    public void Sanitize_ShouldPreserveApostrophesAndHyphens()
    {
        // Act
        var result = NameValidator.Sanitize("O'Brien-Smith");

        // Assert
        result.Should().Be("O'Brien-Smith");
    }

    [Fact]
    public void Sanitize_ShouldPreservePeriods()
    {
        // Act
        var result = NameValidator.Sanitize("Dr. Smith");

        // Assert
        result.Should().Be("Dr. Smith");
    }

    [Fact]
    public void Sanitize_ShouldPreserveUnicodeLetters()
    {
        // Act
        var result = NameValidator.Sanitize("José");

        // Assert
        result.Should().Be("José");
    }

    [Fact]
    public void Sanitize_ShouldPreserveChineseCharacters()
    {
        // Act
        var result = NameValidator.Sanitize("王明");

        // Assert
        result.Should().Be("王明");
    }

    #endregion

    #region Sanitize — Removal

    [Fact]
    public void Sanitize_ShouldRemoveNumbers_WhenPresent()
    {
        // Act
        var result = NameValidator.Sanitize("John123");

        // Assert
        result.Should().Be("John");
    }

    [Fact]
    public void Sanitize_ShouldRemoveHtmlCharacters_WhenPresent()
    {
        // Act
        var result = NameValidator.Sanitize("<b>John</b>");

        // Assert
        result.Should().Be("bJohnb");
    }

    [Fact]
    public void Sanitize_ShouldRemoveSpecialSymbols_WhenPresent()
    {
        // Act
        var result = NameValidator.Sanitize("user@name#!");

        // Assert
        result.Should().Be("username");
    }

    [Fact]
    public void Sanitize_ShouldTrimResult_WhenTrailingSpacesRemain()
    {
        // "John 123" → "John " after stripping digits → "John" after trim
        var result = NameValidator.Sanitize("John 123");

        // Assert
        result.Should().Be("John");
    }

    #endregion

    #region Sanitize — Null/Empty/Whitespace

    [Fact]
    public void Sanitize_ShouldReturnEmpty_WhenNameIsNull()
    {
        // Act
        var result = NameValidator.Sanitize(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_ShouldReturnEmpty_WhenNameIsWhitespace()
    {
        // Act
        var result = NameValidator.Sanitize(" ");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_ShouldReturnEmpty_WhenNameIsEmpty()
    {
        // Act
        var result = NameValidator.Sanitize("");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_ShouldReturnEmpty_WhenAllCharactersAreUnsafe()
    {
        // Act
        var result = NameValidator.Sanitize("123!@#");

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
