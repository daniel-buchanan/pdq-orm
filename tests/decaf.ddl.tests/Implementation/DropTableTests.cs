using System;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Connections;
using decaf.tests.common.Mocks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.ddl.Implementation;

public class DropTableTests
{
    const string TableName = "bob";
    readonly IQueryContainer query;
    
    public DropTableTests()
    {
        var services = new ServiceCollection();
        services.AddDecaf(d => d.UseMockDatabase())
            .WithConnection<IConnectionDetails, MockConnectionDetails>();
        var provider = services.BuildServiceProvider();
        var decaf = provider.GetRequiredService<IDecaf>();
        var unit = decaf.BuildUnit();
        query = unit.GetQuery();
    }

    [Fact]
    public async Task DropTableSucceeds()
    {
        // Arrange
        await query.CreateTable()
            .Named(TableName)
            .WithColumns(c => c.Named("id").AsType<int>())
            .ExecuteAsync();

        // Act
        Func<Task> method = async () => await query.DropTable().Named(TableName).ExecuteAsync();

        // Assert
        await method.Should().NotThrowAsync();
    }
}