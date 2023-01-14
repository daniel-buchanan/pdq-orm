﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using pdq.db.common;
using pdq.state.Utilities;
using Xunit;

namespace pdq.npgsql.tests
{
	public class NpgsqlValueParserTests
	{
		private readonly IValueParser parser;

		public NpgsqlValueParserTests()
		{
			this.parser = new NpgsqlValueParser(new ReflectionHelper());
		}

		[Theory]
		[MemberData(nameof(FromStringCases))]
		public void FromStringSucceeds<T>(string input, T expected)
		{
			// Act
			var result = parser.FromString<T>(input);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		[Theory]
		[MemberData(nameof(ToStringCases))]
		public void ToStringSucceeds<T>(T input, string expected)
		{
			// Act
			var result = parser.ToString(input);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		public static IEnumerable<object[]> ToStringCases
		{
            get
            {
                var dt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
                yield return new object[] { 42, "42" };
                yield return new object[] { (short)42, "42" };
                yield return new object[] { (long)42, "42" };
                yield return new object[] { (double)42.2, "42.2" };
                yield return new object[] { (uint)42, "42" };
                yield return new object[] { new byte[] { 0, 1, 2 }, "\\x000102" };
                yield return new object[] { true, "1" };
                yield return new object[] { false, "0" };
                yield return new object[] { "hello world", "hello world" };
                yield return new object[] { "hello 'world", "hello ''world" };
                yield return new object[] { "hello %%world", "hello world" };
                yield return new object[] { "--hello world", "hello world" };
                yield return new object[] { "%%hello' --world", "hello'' world" };
                yield return new object[] { string.Empty, string.Empty };
                yield return new object[] { dt, dt.ToString("yyyy-MM-ddTHH:mm:ss") };
                yield return new object[] { (string)null, null };
            }
		}

		public static IEnumerable<object[]> FromStringCases
		{
			get
			{
                var dt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
                yield return new object[] { "42", 42 };
                yield return new object[] { "42", (short)42 };
                yield return new object[] { "42", (long)42 };
                yield return new object[] { "42.2", (double)42.2 };
                yield return new object[] { "42", (uint)42 };
                yield return new object[] { "\\x000102", new byte[] { 0, 1, 2 } };
                yield return new object[] { "1" , true};
                yield return new object[] { "0", false };
                yield return new object[] { "hello world", "hello world" };
                yield return new object[] { string.Empty, string.Empty };
                yield return new object[] { dt.ToString("yyyy-MM-ddTHH:mm:ss"), dt };
                yield return new object[] { null, (string)null };
            }
		}
	}
}

