using FluentAssertions;
using Xunit;

namespace decaf.services.tests.General;

public class CompositeKeyTests
{
    [Fact]
    public void TwoComponentKeyValueIsEqual()
    {
        // Arrange
        var key1 = new CompositeKeyValue<int, int>(1, 2);
        var key2 = new CompositeKeyValue<int, int>(1, 2);

        // Act
        var result = key1.Equals(key2);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void TwoComponentKeyValueHashcodeIsEqual()
    {
        // Arrange
        var key1 = new CompositeKeyValue<int, int>(1, 2);
        var key2 = new CompositeKeyValue<int, int>(1, 2);

        // Act
        var hash1 = key1.GetHashCode();
        var hash2 = key2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void TwoComponentKeyValueToStringIsValid()
    {
        // Arrange
        var expected = "(1,2)";
        var key1 = new CompositeKeyValue<int, int>(1, 2);
        
        // Act
        var created = key1.ToString();

        // Assert
        created.Should().Be(expected);
    }
    
    [Fact]
    public void ThreeComponentKeyValueIsEqual()
    {
        // Arrange
        var key1 = new CompositeKeyValue<int, int, int>(1, 2, 3);
        var key2 = new CompositeKeyValue<int, int, int>(1, 2, 3);
        var key3 = new CompositeKeyValue<int, int, int>(1, 2, 3);

        // Act
        var result = key1.Equals(key2) &&
                     key2.Equals(key3) &&
                     key1.Equals(key3);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ThreeComponentKeyValueHashcodeIsEqual()
    {
        // Arrange
        var key1 = new CompositeKeyValue<int, int, int>(1, 2, 3);
        var key2 = new CompositeKeyValue<int, int, int>(1, 2, 3);
        var key3 = new CompositeKeyValue<int, int, int>(1, 2, 3);

        // Act
        var hash1 = key1.GetHashCode();
        var hash2 = key2.GetHashCode();
        var hash3 = key3.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
        hash2.Should().Be(hash3);
        hash3.Should().Be(hash1);
    }

    [Fact]
    public void ThreeComponentKeyValueToStringIsValid()
    {
        // Arrange
        var expected = "(1,2,3)";
        var key1 = new CompositeKeyValue<int, int, int>(1, 2, 3);
        
        // Act
        var created = key1.ToString();

        // Assert
        created.Should().Be(expected);
    }
}