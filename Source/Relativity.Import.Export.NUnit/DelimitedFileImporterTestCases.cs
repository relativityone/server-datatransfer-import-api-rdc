// -----------------------------------------------------------------------------------------------------
// <copyright file="DelimitedFileImporterTestCases.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines static properties to provide <see cref="DelimitedFileImporter"/> test cases.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Collections;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Importer;

	/// <summary>
	/// Defines static properties to provide <see cref="DelimitedFileImporter"/> test cases.
	/// </summary>
	internal static class DelimitedFileImporterTestCases
	{
		/// <summary>
		/// Gets the test case data source.
		/// </summary>
		/// <value>
		/// The <see cref="IEnumerable"/> instance.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used via NUnit's TestCaseSource feature.")]
		public static IEnumerable ReadFileTestCaseSource
		{
			get
			{
				yield return new TestCaseData("1. No quotes, No newline", "One Two Three", new[] { "One Two Three" });
				yield return new TestCaseData(
					"2. No quotes, Newline=CR+LF",
					"One Two Three\r\nFour Five",
					new[] { "One Two Three", "Four Five" });
				yield return new TestCaseData(
					"3. No quotes, Newline=LF",
					"One Two Three\nFour Five",
					new[] { "One Two Three\nFour Five" });
				yield return new TestCaseData(
					"4. Quote starts in middle; No newline",
					"One \"Two\" Three",
					new[] { "One \"Two\" Three" });
				yield return new TestCaseData(
					"BAD-- 6. Quote starts at beginning; No newline",
					"\"One\" Two Three",
					new[] { "OneTwo Three" });
				yield return new TestCaseData(
					"BAD-- 7. Quoted text at the beginning and in the middle; No newline",
					"\"One\" \"Two\" Three",
					new[] { "One\"TwoThree" });
				yield return new TestCaseData(
					"8. Quotes spanning multiple lines, Newline=CR+LF",
					"\"One Two Three\r\nFour Five\"",
					new[] { "One Two Three\nFour Five" });
				yield return new TestCaseData(
					"9. Quotes spanning multiple lines, Newline=LF",
					"\"One Two Three\nFour Five\"",
					new[] { "One Two Three\nFour Five" });
				yield return new TestCaseData(
					"10. Quotes around the whole line, No newline",
					"\"One Two Three\"",
					new[] { "One Two Three" });
				yield return new TestCaseData(
					"11. Double Quotes around the whole line, No newline",
					"\"\"One Two Three\"\"",
					new[] { "\"One Two Three\"" });
				yield return new TestCaseData(
					"12. Triple Quotes around the whole line, No newline",
					"\"\"\"One Two Three\"\"\"",
					new[] { "\"One Two Three\"" });
				yield return new TestCaseData(
					"13. Four Quotes around the whole line, No newline",
					"\"\"\"\"One Two Three\"\"\"\"",
					new[] { "\"\"One Two Three\"\"" });
				yield return new TestCaseData(
					"14. Unbalanced quote at beginning, No newline",
					"\"One Two Three",
					new[] { "One Two Three" });
				yield return new TestCaseData(
					"15. Unbalanced double quote at beginning, No newline",
					"\"\"One Two Three",
					new[] { "\"One Two Three" });
				yield return new TestCaseData(
					"16. Unbalanced quote at beginning with commas, No newline",
					"\"One, Two, Three",
					new[] { "One, Two, Three" });
				yield return new TestCaseData(
					"17. Unbalanced double quote at beginning with commas, No newline",
					"\"\"One, Two, Three",
					new[] { "\"One, Two, Three" });
				yield return new TestCaseData(
					"18. Unbalanced quote in the middle, No newline",
					"One \"Two Three",
					new[] { "One \"Two Three" });
				yield return new TestCaseData(
					"19. Unbalanced double quote in the middle, No newline",
					"One \"\"Two Three",
					new[] { "One \"\"Two Three" });
				yield return new TestCaseData(
					"20. Unbalanced quote in the middle with commas, No newline",
					"One \"Two, Three",
					new[] { "One \"Two", "Three" });
				yield return new TestCaseData(
					"21. Unbalanced double quote in the middle with commas, No newline",
					"One \"\"Two, Three",
					new[] { "One \"\"Two", "Three" });
				yield return new TestCaseData(
					"22. Unbalanced quote at the end, No newline",
					"One Two Three\"",
					new[] { "One Two Three\"" });
				yield return new TestCaseData(
					"23. Unbalanced quote at beginning, Newline=CR+LF",
					"\"One Two Three\r\nFour Five",
					new[] { "One Two Three\r\nFour Five" });
				yield return new TestCaseData(
					"24. Unbalanced quote in the middle, Newline=CR+LF",
					"One \"Two Three\r\nFour Five",
					new[] { "One \"Two Three", "Four Five" });
				yield return new TestCaseData(
					"25. Unbalanced quote at the end of the 1st line, Newline=CR+LF ",
					"One Two Three\"\r\nFour Five",
					new[] { "One Two Three\"", "Four Five" });
				yield return new TestCaseData(
					"26. Unbalanced quote at the beginning of the 2nd line, Newline=CR+LF",
					"One Two Three\r\n\"Four Five",
					new[] { "One Two Three", "Four Five" });
				yield return new TestCaseData(
					"27. Unbalanced quote in the middle of the 2nd line, Newline=CR+LF",
					"One Two Three\r\nFour \"Five",
					new[] { "One Two Three", "Four \"Five" });
				yield return new TestCaseData(
					"28. BAD-- Three quotes, No newline",
					"\"One \"Two\" Three",
					new[] { "One TwoThree" });
				yield return new TestCaseData(
					"29. Use a comma delimiter, No newline",
					"One, Two, Three",
					new[] { "One", "Two", "Three" });
				yield return new TestCaseData(
					"30. Use a comma delimiter, Newline=CR+LF",
					"One, Two, Three\r\nFour, Five",
					new[] { "One", "Two", "Three", "Four", "Five" });
				yield return new TestCaseData(
					"31. Use a comma delimiter, Newline=LF",
					"One, Two, Three\nFour, Five",
					new[] { "One", "Two", "Three\nFour", "Five" });
				yield return new TestCaseData(
					"32. Use a comma delimiter with quoted terms, No newline",
					"\"One\", \"Two\", \"Three\"",
					new[] { "One", "Two", "Three" });
				yield return new TestCaseData(
					"33. Use a comma delimiter with quoted terms, Newline=CR+LF",
					"\"One\", \"Two\", \"Three\"\r\n\"Four\", \"Five\"",
					new[] { "One", "Two", "Three", "Four", "Five" });
				yield return new TestCaseData(
					"34. Use a comma delimiter, Newline=LF",
					"\"One\", \"Two\", \"Three\"\n\"Four\", \"Five\"",
					new[] { "One", "Two", "Three\nFour", "Five" });
				yield return new TestCaseData(
					"35. Use a comma delimiter with leading and trailing spaces, No newline",
					" \"One\" , \"Two\" , \"Three\" ",
					new[] { "One", "Two", "Three" });
				yield return new TestCaseData(
					"36. Use a quotes to hide inner commas and also comma delimiters with spaces, No newline",
					" \"One, Two\" , \"Three, Four\" ",
					new[] { "One, Two", "Three, Four" });
				yield return new TestCaseData(
					"37. Use a quote within a word, No newline",
					"Super\"awesome, yeah",
					new[] { "Super\"awesome", "yeah" });
				yield return new TestCaseData(
					"38. Use a quote and space within a word, No newline",
					"Super\" awesome, yeah",
					new[] { "Super\" awesome", "yeah" });
				yield return new TestCaseData(
					"39. Use a space and quote within a word, No newline",
					"Super \"awesome, yeah",
					new[] { "Super \"awesome", "yeah" });
				yield return new TestCaseData(
					"40. Use a double quote within a word, No newline",
					"Super\"\"awesome, yeah",
					new[] { "Super\"\"awesome", "yeah" });
				yield return new TestCaseData(
					"41. Use a double quote and space within a word, No newline",
					"Super\"\" awesome, yeah",
					new[] { "Super\"\" awesome", "yeah" });
				yield return new TestCaseData(
					"42. Use a space and quote within a word, No newline",
					"Super \"\"awesome, yeah",
					new[] { "Super \"\"awesome", "yeah" });
				yield return new TestCaseData(
					"43. Quote the entire string, inner commas, No newline",
					"\"One, Two, Three\"",
					new[] { "One, Two, Three" });
				yield return new TestCaseData(
					"44. Double quotes on the entire string, inner commas, No newline",
					"\"\"One, Two, Three\"\"",
					new[] { "\"One, Two, Three\"" });
				yield return new TestCaseData(
					"45. Triple quotes on the entire string, inner commas, No newline",
					"\"\"\"One, Two, Three\"\"\"",
					new[] { "\"One, Two, Three\"" });
				yield return new TestCaseData(
					"46. Three terms separated by newline, Newline=LF",
					"One\nTwo\nThree",
					new[] { "One\nTwo\nThree" });
				yield return new TestCaseData(
					"47. Double quotes in the front and middle, No newline",
					"\"\"One\"\", \"\"Two, Three\"\", \"\"Four\"\"",
					new[] { "\"One\", \"Two, Three\", \"Four\"" });
				yield return new TestCaseData(
					"48. Double quotes in the middle, No newline",
					"One, \"\"Two, Three\"\", \"\"Four\"\"",
					new[] { "One", "\"Two, Three\", \"Four\"" });
				yield return new TestCaseData(
					"49. Outer quotes and quotes in the middle, No newline",
					"\"One, \"Two, Three\", Four\"",
					new[] { "One, Two, Three", "Four\"" });
				yield return new TestCaseData(
					"50. BAD-- Double quotes in the front, two separated quotes later, No newline",
					"\"\"One\" Two\" Three",
					new[] { "\"OneTwoThree" });
				yield return new TestCaseData(
					"51. BAD-- Double quotes in the front, one quote in the middle, one at the end, No newline",
					"\"\"One Two\" Three\"",
					new[] { "\"One TwoThree" });
				yield return new TestCaseData(
					"52. Special character <CR>, No newline",
					"One\r Two",
					new[] { "One\r Two" });
				yield return new TestCaseData(
					"53. Special character <Tab>, No newline",
					"One\t Two",
					new[] { "One\t Two" });
				yield return new TestCaseData(
					"54. Comma and newline separators, Newline=CR+LF",
					"One,\r\nTwo,\r\nThree",
					new[] { "One", string.Empty, "Two", string.Empty, "Three" });
				yield return new TestCaseData(
					"55. Quotes in the middle with inner comma, No newline",
					"One \"Two, Three\", Four",
					new[] { "One \"Two", "Three\"", "Four" });
				yield return new TestCaseData(
					"56. Double quotes in the middle with inner comma, No newline",
					"One \"\"Two, Three\"\", Four",
					new[] { "One \"\"Two", "Three\"\"", "Four" });
				yield return new TestCaseData(
					"57. Space surrounding each term, multiline",
					" One \r\n Two \r\n Three ",
					new[] { "One ", "Two ", "Three " });
				yield return new TestCaseData(
					"58. Space quote space surrounding each term, multiline",
					" \" One \" \r\n \" Two \" \r\n \" Three \" ",
					new[] { "One", "Two", "Three" });
				yield return new TestCaseData(
					"59. Space literal quote space surrounding each term, multiline",
					" \"\" One \"\" \r\n \"\" Two \"\" \r\n \"\" Three \"\" ",
					new[] { "\" One \" \r\n \" Two \" \r\n \" Three \" " });
			}
		}
	}
}