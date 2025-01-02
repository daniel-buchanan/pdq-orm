using System;
using decaf.state.Ddl.Definitions;
using FluentAssertions;
using Xunit;

namespace decaf.ddl.State;

public class PrimaryKeyDefinitionTests
{
    [Fact]
    public void ThrowsArgumentNullExceptionWhenNameIsEmpty()
    {
        // Act
        Action method = () => PrimaryKeyDefinition.Create(string.Empty);
        
        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ThrowsArgumentNullExceptionWhenNameIsNull()
    {
        // Act
        Action method = () => PrimaryKeyDefinition.Create(null);
        
        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData([""])]
    [InlineData(["  "])]
    [InlineData(["\t"])]
    [InlineData(["\r\n"])]
    public void ThrowsArgumentNullExceptionWhenNameIsWhitespace(string whitespace)
    {
        // Act
        Action method = () => PrimaryKeyDefinition.Create(whitespace);
        
        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
}