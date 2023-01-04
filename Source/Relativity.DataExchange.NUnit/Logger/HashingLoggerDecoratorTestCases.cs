// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingLoggerDecoratorTestCases.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Creates TestCaseData for HashingLoggerDecorator unit tests"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Logger;

	public static class HashingLoggerDecoratorTestCases
	{
		public static IEnumerable<TestCaseData> ShouldHashSensitiveDataTestCaseData
		{
			get
			{
				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg", new[] { 0 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg", new[] { 0 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { null, new[] { 0 } },
					new object[] { null },
					true);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { string.Empty, new[] { 0 } },
					new object[] { "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg", new[] { 1 } },
					new object[] { "C:/photo.jpg" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new[] { 0, 1 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58" },
					true);
				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new[] { 0 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", 1000 },
					true);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new[] { 1 } },
					new object[] { "C:/photo.jpg", "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new int[] { } },
					new object[] { "C:/photo.jpg", 1000 },
					true);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new[] { 0, 2 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", 1000 },
					true);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg" },
					new object[] { "C:/photo.jpg" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { null },
					new object[] { null },
					false);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { string.Empty },
					new object[] { string.Empty },
					false);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg".Secure() },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000 },
					new object[] { "C:/photo.jpg", 1000 },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg".Secure(), 1000.Secure() },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg".Secure(), 1000 },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", 1000 },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000.Secure() },
					new object[] { "C:/photo.jpg", "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000.Secure(), 2000.Secure() },
					new object[] { "C:/photo.jpg", "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58", "81a83544cf93c245178cbc1620030f1123f435af867c79d87135983c52ab39d9" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { 1000.Secure() },
					new object[] { "40510175845988f13f6162ed8526f0b09f73384467fa855e1e79b44a56562a58" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { ((string)null).Secure() },
					new object[] { "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855" },
					false);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000 },
					new object[] { "C:/photo.jpg", 1000 },
					false);

				yield return new TestCaseData(
					"The file is closed.",
					new object[] { },
					new object[] { },
					false);

				yield return new TestCaseData(
					"The file is closed.",
					null,
					null,
					false);

				yield return new TestCaseData(
					null,
					null,
					null,
					false);
			}
		}

		public static IEnumerable<TestCaseData> ShouldHashSensitiveDataLogIndexErrorTestCaseData
		{
			get
			{
				yield return new TestCaseData(
					"The file {path} is closed.",
					new object[] { "C:/photo.jpg", new[] { 1 } },
					new object[] { "C:/photo.jpg" },
					true);

				yield return new TestCaseData(
					"The file {path} is closed. File size {size}.",
					new object[] { "C:/photo.jpg", 1000, new[] { 0, 2 } },
					new object[] { "7de5e48a5b2acd7424a1042a97d59994bf9561ccd54b1c5a6c4db89d564f3539", 1000 },
					true);
			}
		}
	}
}
