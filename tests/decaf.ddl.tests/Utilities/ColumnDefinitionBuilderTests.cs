using System.Collections.Generic;
using decaf.common.Utilities.Reflection.Dynamic;
using decaf.state.Ddl.Definitions;
using decaf.tests.common.Models;
using FluentAssertions;
using Xunit;

namespace decaf.ddl.Utilities
{
    public class ColumnDefinitionBuilderTests
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void DynamicColumnInfoParsedCorrectly(DynamicColumnInfo info, ColumnDefinition expected)
        {
            // Act
            var def = ColumnDefinitionBuilder.Build(info);

            // Assert
            def.Name.Should().Be(info.Name != info.NewName ? info.NewName : expected.Name);
            def.Type.Should().Be(expected.Type);
        }

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { DynamicColumnInfo.Empty(), ColumnDefinition.Create(null) };
                yield return new object[] { DynamicColumnInfo.Create("name"), ColumnDefinition.Create("name") };
                yield return new object[] { DynamicColumnInfo.Create("name", "bob"), ColumnDefinition.Create("bob") };
            }
        }

        [Fact]
        public void ExpressionParsedCorrectly()
        {
            // Arrange
            
            // Act
            var def = ColumnDefinitionBuilder.Build<Person>(p => p.FirstName);

            // Assert
            def.Name.Should().Be(nameof(Person.FirstName));
            def.Type.Should().Be(typeof(string));
        }
    }
}