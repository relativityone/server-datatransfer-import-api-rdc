// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableTypesHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents the <see cref="NullableTypesHelper"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange;

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
				Assert.That(DataExchange.NullableTypesHelper.GetNullableBoolean(value), Is.Null);
			}
			else
			{
				Assert.That(DataExchange.NullableTypesHelper.GetNullableBoolean(value), Is.EqualTo(expected));
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
				Assert.That(DataExchange.NullableTypesHelper.GetNullableDateTime(value), Is.Null);
			}
			else
			{
				DateTime expected = new System.DateTime(1985, 5, 25);
				Assert.That(DataExchange.NullableTypesHelper.GetNullableDateTime(value), Is.EqualTo(expected));
			}
		}

		[Test]
		[TestCase("0.125", 0.125d)]
		[TestCase("1.2", 1.2d)]
		[TestCase("", null)]
		[TestCase(null, null)]
		public static void ShouldConvertTheDecimalValue(string value, decimal? expected)
		{
			Assert.That(DataExchange.NullableTypesHelper.ToNullableDecimal(value), Is.EqualTo(expected));
		}

		[Test]
		public static void ShouldConvertTheDateTimeToAnEmptyStringOrValue()
		{
			DateTime? value = DateTime.Now;
			string convertedValue = DataExchange.NullableTypesHelper.ToEmptyStringOrValue(value);
			Assert.That(convertedValue, Is.EqualTo(value.ToString()));
			convertedValue = DataExchange.NullableTypesHelper.ToEmptyStringOrValue(value, false);
			Assert.That(convertedValue, Is.EqualTo(value.ToString()));
			convertedValue = DataExchange.NullableTypesHelper.ToEmptyStringOrValue(value, true);
			Assert.That(convertedValue, Is.EqualTo(value.Value.ToSqlCultureNeutralString()));
			convertedValue = DataExchange.NullableTypesHelper.ToEmptyStringOrValue(null, false);
			Assert.That(convertedValue, Is.EqualTo(string.Empty));
			convertedValue = DataExchange.NullableTypesHelper.ToEmptyStringOrValue(null, true);
			Assert.That(convertedValue, Is.EqualTo(string.Empty));
		}

		[Test]
		public static void ShouldOnlyReturnNullWhenTheValueIsDBNull()
		{
			Assert.That(DataExchange.NullableTypesHelper.DBNullString("abc"), Is.EqualTo("abc"));
			Assert.That(DataExchange.NullableTypesHelper.DBNullString(1), Is.EqualTo("1"));
			Assert.That(DataExchange.NullableTypesHelper.DBNullString(1L), Is.EqualTo("1"));
			Assert.That(DataExchange.NullableTypesHelper.DBNullString(1.5d), Is.EqualTo("1.5"));
			Assert.That(DataExchange.NullableTypesHelper.DBNullString(DBNull.Value), Is.Null);
			Assert.That(DataExchange.NullableTypesHelper.DBNullString(null), Is.Null);
		}

		[Test]
		public static void ShouldOnlyConvertToNullableWhenTheValueIsNotDBNull()
		{
			// Verified using VB.NET NUnit code + the kCura assembly that these behaviors are identical.
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<long>(DBNull.Value), Is.Null);
			int? intTest = 1;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<int>(intTest), Is.EqualTo(1));
			intTest = null;
			Assert.Throws<NullReferenceException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<int>(intTest); });
			bool? boolTest = true;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<bool>(boolTest), Is.True);
			boolTest = false;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<bool>(boolTest), Is.False);
			boolTest = null;
			Assert.Throws<NullReferenceException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<bool>(boolTest); });
			double? doubleTest = 1.25d;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<double>(doubleTest), Is.EqualTo(1.25d));
			doubleTest = null;
			Assert.Throws<NullReferenceException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<double>(doubleTest); });
			decimal? decimalTest = 1.75m;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<decimal>(decimalTest), Is.EqualTo(1.75d));
			decimalTest = null;
			Assert.Throws<NullReferenceException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<decimal>(decimalTest); });
			DateTime? dateTimeTest = DateTime.Now;
			Assert.That(DataExchange.NullableTypesHelper.DBNullConvertToNullable<DateTime>(dateTimeTest), Is.EqualTo(dateTimeTest));
			dateTimeTest = null;
			Assert.Throws<NullReferenceException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<DateTime>(dateTimeTest); });

			// Verify cast exceptions
			dateTimeTest = DateTime.Now;
			Assert.Throws<InvalidCastException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<int>(dateTimeTest); });
			Assert.Throws<InvalidCastException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<bool>(dateTimeTest); });
			Assert.Throws<InvalidCastException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<double>(dateTimeTest); });
			Assert.Throws<InvalidCastException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<decimal>(dateTimeTest); });
			Assert.Throws<InvalidCastException>(
				() => { DataExchange.NullableTypesHelper.DBNullConvertToNullable<long>(dateTimeTest); });
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
			Assert.Throws<SystemException>(() => DataExchange.NullableTypesHelper.GetNullableDateTime(value));
		}
	}
}