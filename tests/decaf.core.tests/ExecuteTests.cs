using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using decaf.common;
using decaf.common.Connections;
using decaf.Implementation.Execute;
using decaf.state;
using decaf.tests.common.Mocks;
using decaf.tests.common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace decaf.core_tests
{
    public class ExecuteTests
    {
        private readonly IExecute execute;
        private readonly ISelectQueryContext context;

        public ExecuteTests()
        {
            var services = new ServiceCollection();
            services.AddDecaf(o => o.UseMockDatabase());
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();
            var provider = services.BuildServiceProvider();
            var decaf = provider.GetService<IDecaf>();
            var transient = decaf.BuildUnit();
            var query = transient.GetQuery() as IQueryContainerInternal;
            var sqlFactory = provider.GetService<ISqlFactory>();
            context = query.CreateContext<ISelectQueryContext>();
            execute = new InheritedExecutor<ISelectQueryContext>(query, context);
        }

        [Fact]
        public void DynamicReturnsSelf()
        {
            // Act
            var dyn = execute.Dynamic();

            // Assert
            dyn.Should().Be(execute);
        }

        [Fact]
        public void AsEnumerableDoesntThrow()
        {
            // Act
            Action method = () => execute.AsEnumerable();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task AsEnumerableAsyncDoesntThrow()
        {
            // Act
            Func<Task<IEnumerable<dynamic>>> method = () => execute.AsEnumerableAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void AsEnumerableTypedDoesntThrow()
        {
            // Act
            Action method = () => execute.AsEnumerable<Person>();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task AsEnumerableTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task<IEnumerable<Person>>> method = () => execute.AsEnumerableAsync<Person>();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void ToListDoesntThrow()
        {
            // Act
            Action method = () => execute.ToList();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task ToListAsyncDoesntThrow()
        {
            // Act
            Func<Task<IList<dynamic>>> method = () => execute.ToListAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void ToListTypedDoesntThrow()
        {
            // Act
            Action method = () => execute.ToList<Person>();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task ToListTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task<IList<Person>>> method = () => execute.ToListAsync<Person>();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void ExecuteDoesntThrow()
        {
            // Act
            Action method = () => execute.Execute();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task ExecuteAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => execute.ExecuteAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void FirstDoesThrow()
        {
            // Act
            Action method = () => execute.First();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task FirstAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => execute.FirstAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void FirstTypedDoesThrow()
        {
            // Act
            Action method = () => execute.First<Person>();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task FirstTypedAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => execute.FirstAsync<Person>();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void FirstOrDefaultDoesntThrow()
        {
            // Act
            Action method = () => execute.FirstOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task FirstOrDefaultAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => execute.FirstOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void FirstOrDefaultTypedDoesntThrow()
        {
            // Act
            Action method = () => execute.FirstOrDefault<Person>();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task FirstOrDefaultTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => execute.FirstOrDefaultAsync<Person>();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void SingleDoesThrow()
        {
            // Act
            Action method = () => execute.Single();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task SingleAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => execute.SingleAsync();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void SingleTypedDoesThrow()
        {
            // Act
            Action method = () => execute.Single<Person>();

            // Assert
            method.Should().Throw<Exception>();
        }

        [Fact]
        public async Task SingleTypedAsyncDoesThrow()
        {
            // Act
            Func<Task> method = () => execute.SingleAsync<Person>();

            // Assert
            await method.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void SingleOrDefaultDoesntThrow()
        {
            // Act
            Action method = () => execute.SingleOrDefault();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task SingleOrDefaultAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => execute.SingleOrDefaultAsync();

            // Assert
            await method.Should().NotThrowAsync();
        }

        [Fact]
        public void SingleOrDefaultTypedDoesntThrow()
        {
            // Act
            Action method = () => execute.SingleOrDefault<Person>();

            // Assert
            method.Should().NotThrow();
        }

        [Fact]
        public async Task SingleOrDefaultTypedAsyncDoesntThrow()
        {
            // Act
            Func<Task> method = () => execute.SingleOrDefaultAsync<Person>();

            // Assert
            await method.Should().NotThrowAsync();
        }
    }

    class InheritedExecutor<T> : Execute<T>
        where T : IQueryContext
    {
        public InheritedExecutor(IQueryContainerInternal query, T context)
            : base(query, context)
        {
        }
    }
}

