// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableTypesHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents the <see cref="NullableTypesHelper"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents the <see cref="NullableTypesHelper"/> tests.
	/// </summary>
	[TestFixture]
	public static class NullableTypesHelperTests
	{
		[Test]
		[TestCase("true", true)]
		[TestCase("True", true)]
		[TestCase("t", true)]
		[TestCase("yes", true)]
		[TestCase("y", true)]
		[TestCase("1", true)]
		[TestCase("true    ", true)]
		[TestCase("false", false)]
		[TestCase("False", false)]
		[TestCase("f", false)]
		[TestCase("no", false)]
		[TestCase("n", false)]
		[TestCase("0", false)]
		[TestCase("false    ", false)]
		[TestCase("xyzzy", false)]
		[TestCase("", null)]
		[TestCase(null, null)]
		public static void ShouldGetTheBooleanValue(string value, bool? expected)
		{
			if (expected == null)
			{
				Assert.That(NullableTypesHelper.GetNullableBoolean(value), Is.Null);
			}
			else
			{
				Assert.That(NullableTypesHelper.GetNullableBoolean(value), Is.EqualTo(expected));
			}
		}

		[Test]
		[TestCase("5/25/85")]
		[TestCase("5/25/1985")]
		[TestCase("05/25/85")]
		[TestCase("05/25/1985")]
		[TestCase("19850525")]
		[TestCase("")]
		[TestCase(null)]
		public static void ShouldGetTheDateTimeValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				Assert.That(NullableTypesHelper.GetNullableDateTime(value), Is.Null);
			}
			else
			{
				DateTime expected = new System.DateTime(1985, 5, 25);
				Assert.That(NullableTypesHelper.GetNullableDateTime(value), Is.EqualTo(expected));
			}
		}

		[Test]
		[TestCase("123/25/1985")]
		[TestCase("plugh")]
		[TestCase("052585")]
		[TestCase("00/00/0000")]
		[TestCase("0/0/0000")]
		[TestCase("0/0/00")]
		[TestCase("00/00/00")]
		[TestCase("0/00")]
		[TestCase("0/0000")]
		[TestCase("00/00")]
		[TestCase("00/0000")]
		[TestCase("0")]
		[TestCase("00000000")]
		public static void ShouldThrowWhenTheDataTimeIsInvalid(string value)
		{
			Assert.Throws<SystemException>(() => NullableTypesHelper.GetNullableDateTime(value));
		}
	}
}