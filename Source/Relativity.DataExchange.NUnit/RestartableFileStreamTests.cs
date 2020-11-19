// -----------------------------------------------------------------------------------------------------
// <copyright file="RestartableFileStreamTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="RestartableFileStream"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Text;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Data;

	public static class RestartableFileStreamTests
	{
		private static readonly IEnumerable<string> InfiniteFileContent = Enumerable.Range(0, int.MaxValue).Select(a => a.ToString());

		private static IEnumerable<TestCaseData> FileStreamShouldBeRestartableTestData
		{
			get
			{
				yield return new TestCaseData(200_000, 20, Encoding.ASCII, true);
				yield return new TestCaseData(10_000, 3, Encoding.ASCII, true);
				yield return new TestCaseData(100, 3, Encoding.ASCII, true);
				yield return new TestCaseData(100_000, 1, Encoding.ASCII, true);
				yield return new TestCaseData(100_000, 0, Encoding.ASCII, true);

				yield return new TestCaseData(200_000, 20, Encoding.BigEndianUnicode, true);
				yield return new TestCaseData(10_000, 3, Encoding.BigEndianUnicode, true);
				yield return new TestCaseData(100, 3, Encoding.BigEndianUnicode, true);
				yield return new TestCaseData(100_000, 1, Encoding.BigEndianUnicode, true);
				yield return new TestCaseData(100_000, 0, Encoding.BigEndianUnicode, true);

				yield return new TestCaseData(200_000, 20, Encoding.UTF8, true);
				yield return new TestCaseData(10_000, 3, Encoding.UTF8, true);
				yield return new TestCaseData(100, 3, Encoding.UTF8, true);
				yield return new TestCaseData(100_000, 1, Encoding.UTF8, true);
				yield return new TestCaseData(100_000, 0, Encoding.UTF8, true);

				yield return new TestCaseData(200_000, 20, Encoding.Unicode, true);
				yield return new TestCaseData(10_000, 3, Encoding.Unicode, true);
				yield return new TestCaseData(100, 3, Encoding.Unicode, true);
				yield return new TestCaseData(100_000, 1, Encoding.Unicode, true);
				yield return new TestCaseData(100_000, 0, Encoding.Unicode, true);

				yield return new TestCaseData(200_000, 20, Encoding.ASCII, false);
				yield return new TestCaseData(10_000, 3, Encoding.ASCII, false);
				yield return new TestCaseData(100, 3, Encoding.ASCII, false);
				yield return new TestCaseData(100_000, 1, Encoding.ASCII, false);
				yield return new TestCaseData(100_000, 0, Encoding.ASCII, false);

				yield return new TestCaseData(200_000, 20, Encoding.BigEndianUnicode, false);
				yield return new TestCaseData(10_000, 3, Encoding.BigEndianUnicode, false);
				yield return new TestCaseData(100, 3, Encoding.BigEndianUnicode, false);
				yield return new TestCaseData(100_000, 1, Encoding.BigEndianUnicode, false);
				yield return new TestCaseData(100_000, 0, Encoding.BigEndianUnicode, false);

				yield return new TestCaseData(200_000, 20, Encoding.UTF8, false);
				yield return new TestCaseData(10_000, 3, Encoding.UTF8, false);
				yield return new TestCaseData(100, 3, Encoding.UTF8, false);
				yield return new TestCaseData(100_000, 1, Encoding.UTF8, false);
				yield return new TestCaseData(100_000, 0, Encoding.UTF8, false);

				yield return new TestCaseData(200_000, 20, Encoding.Unicode, false);
				yield return new TestCaseData(10_000, 3, Encoding.Unicode, false);
				yield return new TestCaseData(100, 3, Encoding.Unicode, false);
				yield return new TestCaseData(100_000, 1, Encoding.Unicode, false);
				yield return new TestCaseData(100_000, 0, Encoding.Unicode, false);
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "it's a test, and it can handle this")]
		[SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "numberOfRestarts+1", Justification = "never happens, as we pass the number in")]
		[TestCaseSource(nameof(FileStreamShouldBeRestartableTestData))]
		public static void FileStreamShouldBeRestartable(int size, int numberOfRestarts, Encoding encoding, bool detectBOM)
		{
			// Arrange
			string fileName = $"FileStreamShouldBeRestartableTestFile_{Guid.NewGuid()}";

			var fileFunction = InfiniteFileContent.Take(size);
			File.AppendAllLines(fileName, fileFunction, encoding);

			// Act
			using (var retryStream = new RestartableFileStream(fileName))
			{
				// Assert
				using (var reader = new StreamReader(retryStream, encoding, detectBOM))
				{
					foreach (var filePiece in fileFunction.Select((a, b) => new { stringValue = a, index = b }))
					{
						Assert.AreEqual(filePiece.stringValue, reader.ReadLine());

						if (size / (numberOfRestarts + 1) == filePiece.index)
						{
							retryStream.Restart();
						}
					}
				}
			}

			File.Delete(fileName);
		}
	}
}
