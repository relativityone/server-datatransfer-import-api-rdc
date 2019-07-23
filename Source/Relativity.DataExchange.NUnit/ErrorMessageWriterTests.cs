// -----------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageWriterTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using Moq;
	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents <see cref="ErrorMessageWriter{T}"/> tests.
	/// </summary>
	[TestFixture]
	public static class ErrorMessageWriterTests
	{
		[Test(Description = "Error message writer should not throw on concurrent access.")]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "This is a test, and this is not a problem here.")]
		public static void ErrorMessageWriterShouldNotThrowOnConcurrentAccess()
		{
			// Arrange
			const int numberOfIterations = 500;
			var path = Path.GetRandomFileName();
			var errorArguments = GetSomeErrorArguments();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				Parallel.ForEach(Enumerable.Repeat(errorArguments, numberOfIterations), a => errorWriter.WriteErrorMessage(a.Object));
			}

			// Assert
			var result = File.ReadAllLines(path);
			Assert.AreEqual(numberOfIterations, result.Length, $"The file should contain {numberOfIterations} lines, while it only contains {result.Length} lines.");

			// Clean
			File.Delete(path);
		}

		[Test(Description = "Error message writer should not create file if created, or disposed.")]
		public static void ErrorMessageWriterShouldNotCreateFileIfCreated()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				Assert.IsFalse(File.Exists(path));
				Assert.IsFalse(errorWriter.FileCreated);
			}

			// Assert
			Assert.IsFalse(File.Exists(path));
		}

		[Test(Description = "Error message writer should create file if it has written something to disk.")]
		public static void ErrorMessageWriterShouldCreateFileIfItHasWrittenSomethingToDisk()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				errorWriter.WriteErrorMessage(GetSomeErrorArguments().Object);
				Assert.IsTrue(File.Exists(path));
				Assert.IsTrue(errorWriter.FileCreated);
			}

			// Assert
			Assert.IsTrue(File.Exists(path));

			// Clean
			File.Delete(path);
		}

		[Test(Description = "Error message writer should create filepath if instantiated without file path.")]
		public static void ErrorMessageWriterShouldCreateFilePathIfInstantiatedWithoutFilePath()
		{
			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>())
			{
				// Assert
				Assert.IsNotNull(errorWriter.FilePath);
				Assert.IsFalse(File.Exists(errorWriter.FilePath));
			}
		}

		[Test(Description = "Error message writer should create filepath equal to file path given.")]
		public static void ErrorMessageWriterShouldCreateFilePathEqualToFilePathGiven()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				Assert.AreSame(path, errorWriter.FilePath);
			}
		}

		private static IEnumerable<string> SomeArguments()
		{
			yield return "arg1";
			yield return "arg\"2";
			yield return "arg\"\"3";
			yield return "arg4";
			yield return "arg5";
		}

		private static Mock<IErrorArguments> GetSomeErrorArguments()
		{
			var errorArguments = new Mock<IErrorArguments>();
			errorArguments.Setup(x => x.ValuesForErrorFile()).Returns(SomeArguments);
			return errorArguments;
		}
	}
}