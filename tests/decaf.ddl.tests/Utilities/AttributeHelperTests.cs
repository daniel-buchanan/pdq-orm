using decaf.common.Utilities.Reflection;
using decaf.ddl.Attributes;
using FluentAssertions;
using Xunit;

namespace decaf.ddl.Utilities;

public class AttributeHelperTests
{
    private readonly AttributeHelper attributeHelper;

    public AttributeHelperTests()
    {
        var reflectionHelper = new ReflectionHelper();
        var expressionHelper = new ExpressionHelper(reflectionHelper);
        attributeHelper = new AttributeHelper(reflectionHelper, expressionHelper);
    }
    
    [Fact]
    public void SingleKey_PrimaryKeyParsedSuccessfully()
    {
        // Act
        var pk = attributeHelper.GetPrimaryKey<SingleKeyModel>();

        // Assert
        pk.Columns.Should().Satisfy(cd => cd.Name == nameof(SingleKeyModel.Id));
    }

    [Fact]
    public void SingleKey_PrimaryKeyWithDefaultName()
    {
        // Arrange
        var keyName = $"pk_{typeof(SingleKeyModel).Name}";
        
        // Act
        var pk = attributeHelper.GetPrimaryKey<SingleKeyModel>();

        // Assert
        pk.Name.Should().Be(keyName);
    }
    
    [Fact]
    public void SingleKey_PrimaryKeyWithProvidedName()
    {
        // Arrange
        var keyName = $"test";
        
        // Act
        var pk = attributeHelper.GetPrimaryKey<SingleKeyModel>(keyName);

        // Assert
        pk.Name.Should().Be(keyName);
    }
    
    [Fact]
    public void MultipleKey_PrimaryKeyParsedSuccessfully()
    {
        // Act
        var pk = attributeHelper.GetPrimaryKey<MultipleKeyModel>();

        // Assert
        pk.Columns.Should().Satisfy(
            cd => cd.Name == nameof(MultipleKeyModel.ComponentOne),
            cd => cd.Name == nameof(MultipleKeyModel.ComponentTwo));
    }

    [Fact]
    public void MultipleKey_PrimaryKeyWithDefaultName()
    {
        // Arrange
        var keyName = $"pk_{typeof(MultipleKeyModel).Name}";
        
        // Act
        var pk = attributeHelper.GetPrimaryKey<MultipleKeyModel>();

        // Assert
        pk.Name.Should().Be(keyName);
    }
    
    [Fact]
    public void MultipleKey_PrimaryKeyWithProvidedName()
    {
        // Arrange
        var keyName = $"test";
        
        // Act
        var pk = attributeHelper.GetPrimaryKey<MultipleKeyModel>(keyName);

        // Assert
        pk.Name.Should().Be(keyName);
    }

    private class SingleKeyModel
    {
        [PrimaryKeyComponent]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class MultipleKeyModel
    {
        [PrimaryKeyComponent]
        public int ComponentOne { get; set; }
        [PrimaryKeyComponent]
        public int ComponentTwo { get; set; }
        public string Name { get; set; }
    }
}