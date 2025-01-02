using System;
using System.Collections.Generic;
using decaf.common.Utilities.Reflection;
using decaf.db.common.ANSISQL;
using FluentAssertions;
using Xunit;

namespace decaf.npgsql.tests;

public class NpgsqlValueParserTests
{
	private readonly NpgsqlValueParser parser = new(new ReflectionHelper(), new Constants());

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
			yield return [42, "42"];
			yield return [(short)42, "42"];
			yield return [(long)42, "42"];
			yield return [(double)42.2, "42.2"];
			yield return [(uint)42, "42"];
			yield return [new byte[] { 0, 1, 2 }, @"\x000102"];
			yield return [true, "1"];
			yield return [false, "0"];
			yield return ["hello world", "hello world"];
			yield return ["hello 'world", "hello ''world"];
			yield return ["hello %%world", "hello world"];
			yield return ["--hello world", "hello world"];
			yield return ["%%hello' --world", "hello'' world"];
			yield return [string.Empty, null];
			yield return [dt, dt.ToString("yyyy-MM-ddTHH:mm:ss")];
			yield return [(string)null, string.Empty];
		}
	}

	public static IEnumerable<object[]> FromStringCases
	{
		get
		{
			var dt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"));
			yield return ["42", 42];
			yield return ["42", (short)42];
			yield return ["42", (long)42];
			yield return ["42.2", (double)42.2];
			yield return ["42", (uint)42];
			yield return [@"\x000102", new byte[] { 0, 1, 2 }];
			yield return ["\\\\x000102", new byte[] { 0, 1, 2 }];
			yield return ["1" , true];
			yield return ["0", false];
			yield return ["hello world", "hello world"];
			yield return [string.Empty, null];
			yield return [dt.ToString("yyyy-MM-ddTHH:mm:ss"), dt];
			yield return [null, (string)null];
		}
	}
}