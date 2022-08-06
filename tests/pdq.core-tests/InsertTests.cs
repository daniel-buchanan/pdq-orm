﻿using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using pdq.common;
using pdq.core_tests.Mocks;
using pdq.state;
using Xunit;

namespace pdq.core_tests
{
    public class InsertTests
    {
        private IQueryInternal query;

        public InsertTests()
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
        public void SimpleInsertSucceeds()
        {
            // Arrange

            // Act
            this.query.Insert()
                .Into("users")
                .Columns(b => new
                {
                    email = b.Is<string>(),
                    first_name = b.Is<string>(),
                    last_name = b.Is<string>()
                })
                .Value(() => new
                {
                    email = "bob@bob.com",
                    first_name = "Bob",
                    last_name = "Smith"
                });

            // Assert
            var context = this.query.Context as IInsertQueryContext;
            context.Should().NotBeNull();
            context.Target.Name.Should().Be("users");
            context.Columns.Count.Should().Be(3);
            context.Source.Should().BeAssignableTo<IInsertStaticValuesSource>();
        }
    }
}

