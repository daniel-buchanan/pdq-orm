using System;
using decaf.state.Ddl.Definitions;
using FluentAssertions;
using Xunit;

namespace decaf.ddl.State;

public class IndexDefinitionTests
{
    [Fact]
    public void ThrowsArgumentNullExceptionWhenNameIsEmpty()
    {
        // Act
        Action method = () => IndexDefinition.Create(string.Empty);
        
        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ThrowsArgumentNullExceptionWhenNameIsNull()
    {
        // Act
        Action method = () => IndexDefinition.Create(null);
        
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
        Action method = () => IndexDefinition.Create(whitespace);
        
        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
}