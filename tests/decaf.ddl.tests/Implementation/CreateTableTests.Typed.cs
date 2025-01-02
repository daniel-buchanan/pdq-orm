using System;
using decaf.state;
using decaf.state.Ddl.Definitions;
using decaf.tests.common.Models;
using FluentAssertions;
using Xunit;

namespace decaf.ddl.Implementation;

public partial class CreateTableTests
{
    [Fact]
    public void GenericColumnsSucceeds()
    {
        // Arrange
        var impl = query.CreateTable();
        
        // Act
        impl.Named(TableName)
            .WithColumns<Note>();

        // Assert
        var o = query.Context as ICreateTableQueryContext;
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.PersonId), typeof(int)));
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.Value), typeof(string)));
    }
    
    [Fact]
    public void FromTypeSucceeds()
    {
        // Arrange
        var impl = query.CreateTable();
        
        // Act
        impl.FromType<Note>();

        // Assert
        var o = query.Context as ICreateTableQueryContext;
        o!.Name.Should().Be(nameof(Note));
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.PersonId), typeof(int)));
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.Value), typeof(string)));
    }
    
    [Fact]
    public void TypedColumnsSucceeds()
    {
        // Arrange
        var impl = query.CreateTable<Note>();
        
        // Act
        impl.WithColumns(c => c.PersonId, c => c.Value);

        // Assert
        var o = query.Context as ICreateTableQueryContext;
        o!.Name.Should().Be(nameof(Note));
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.PersonId), typeof(int)));
        o!.Columns.Should().ContainEquivalentOf(ColumnDefinition.Create(nameof(Note.Value), typeof(string)));
    }

    [Fact]
    public void TypedIndexSucceeds()
    {
        // Arrange
        var impl = query.CreateTable<Note>();

        // Act
        impl.WithIndex(c => c.PersonId);

        // Assert
        var c = query.Context as ICreateTableQueryContext;
        c!.Indexes.Should()
            .ContainEquivalentOf(IndexDefinition.Create(nameof(Note),
                ColumnDefinition.Create(nameof(Note.PersonId), typeof(int))));
    }
    
    [Fact]
    public void TypedIndexWithNameSucceeds()
    {
        // Arrange
        var impl = query.CreateTable<Note>();

        // Act
        impl.WithIndex("idx", c => c.PersonId);

        // Assert
        var c = query.Context as ICreateTableQueryContext;
        c!.Indexes.Should()
            .ContainEquivalentOf(IndexDefinition.Create("idx",nameof(Note),
                ColumnDefinition.Create(nameof(Note.PersonId), typeof(int))));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void TypedIndexWithNullOrEmptyNameThrows(string name)
    {
        // Arrange
        var impl = query.CreateTable<Note>();

        // Act
        Action method = () => impl.WithIndex(name, c => c.PersonId);

        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void TypedPrimaryKeyWithNullOrEmptyNameThrows(string name)
    {
        // Arrange
        var impl = query.CreateTable<Note>();

        // Act
        Action method = () => impl.WithPrimaryKey(name, c => c.PersonId);

        // Assert
        method.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void TypedPrimaryKeySucceeds()
    {
        // Arrange
        var impl = query.CreateTable<Note>();

        // Act
        impl.WithPrimaryKey(c => c.PersonId);

        // Assert
        var c = query.Context as ICreateTableQueryContext;
        c!.PrimaryKey.Should()
            .BeEquivalentTo(PrimaryKeyDefinition.Create("pk_" + nameof(Note),
                ColumnDefinition.Create(nameof(Note.PersonId), typeof(int))));
    }
}