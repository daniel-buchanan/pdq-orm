using decaf.Implementation.Execute;
using FluentAssertions;
using Xunit;

namespace decaf.core_tests
{
	public class SelectColumnBuilderTests
	{
        private readonly SelectColumnBuilder builder = new();

        [Fact]
		public void IsReturnsNull()
        {
			var value = builder.Is("name");
			value.Should().BeNull();
        }

        [Fact]
        public void IsWithAliasReturnsNull()
        {
            var value = builder.Is("name", "p");
            value.Should().BeNull();
        }

        [Fact]
        public void IsGenericReturnsNull()
        {
            var value = builder.Is<string>("name");
            value.Should().BeNull();
        }

        [Fact]
        public void IsGenericWithAliasReturnsNull()
        {
            var value = builder.Is<string>("name", "p");
            value.Should().BeNull();
        }
    }
}

