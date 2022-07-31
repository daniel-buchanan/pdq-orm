﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using pdq.common;
using pdq.core_tests.Mocks;
using pdq.core_tests.Models;
using pdq.state;
using pdq.state.Conditionals;
using pdq.state.Conditionals.ValueFunctions;
using pdq.state.QueryTargets;
using Xunit;

namespace pdq.core_tests
{
    public class DeleteTests
    {
        private IQueryInternal query;

        public DeleteTests()
        {
            var services = new ServiceCollection();
            services.AddPdq(o =>
            {
                o.EnableTransientTracking();
                o.OverrideDefaultLogLevel(LogLevel.Debug);
                o.UseMockDatabase();
            });
            services.AddScoped<IConnectionDetails, MockConnectionDetails>();

            var provider = services.BuildServiceProvider();
            var uow = provider.GetService<IUnitOfWork>();
            var transient = uow.Begin();
            this.query = transient.Query() as IQueryInternal;
        }

        [Fact]
        public void SimpleDeleteSucceeds()
        {
            // Act
            this.query
                .Delete()
                .From<Person>(p => p)
                .Where(p => p.LastName.Contains("smith"));

            // Assert
            var context = this.query.Context as IDeleteQueryContext;
            context.QueryTargets.Should().HaveCount(1);
            var where = context.WhereClause as IColumn;
            where.Should().NotBeNull();
            where.Details.Name.Should().Be(nameof(Person.LastName));
            where.Details.Source.Alias.Should().Be("p");
            where.ValueFunction.Should().BeAssignableTo(typeof(StringContains));
        }
    }
}

