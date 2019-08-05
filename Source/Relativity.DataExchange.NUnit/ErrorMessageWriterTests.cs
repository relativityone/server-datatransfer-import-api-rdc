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

		[Test(Description = "Error message writer should write escaped messages.")]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "This is a test, and this is not a problem here.")]
		public static void ErrorMessageWriterShouldWriteEscapedErrorMessages()
		{
			// Arrange
			var path = Path.GetRandomFileName();
			var errorArguments = GetSomeErrorArguments();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				errorWriter.WriteErrorMessage(errorArguments.Object);
			}

			var result = File.ReadAllLines(path);

			// Assert
			Assert.AreEqual(1, result.Length, "If you write 1 line, the number of lines should be 1.");
			Assert.AreEqual("\"arg1\",\"arg\"\"2\",\"arg\"\"\"\"3\",\"arg4\",\"arg5\"", result.First(), "The escaping of error messages is not going well.");

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

		[Test(Description = "Error message writer should not create file if hold released.")]
		public static void ErrorMessageWriterShouldNotCreateFileIfHoldReleased()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				errorWriter.ReleaseLock();
				Assert.IsFalse(File.Exists(path));
				Assert.IsFalse(errorWriter.FileCreated);
			}

			// Assert
			Assert.IsFalse(File.Exists(path));
		}

		[Test(Description = "If hold is released file should not be held by writer.")]
		public static void IfHoldIsReleasedFileShouldNotBeHeldByWriter()
		{
			// Arrange
			var path = Path.GetRandomFileName();
			var errorArguments = GetSomeErrorArguments();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				errorWriter.WriteErrorMessage(errorArguments.Object);
				var ex = Assert.Catch<IOException>(() => File.ReadAllLines(path));
				Assert.That(ex.Message.Contains("because it is being used"));
				errorWriter.ReleaseLock();
				Assert.DoesNotThrow(() => File.ReadAllLines(path));
			}

			// Clean
			File.Delete(path);
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

		[Test(Description = "Error message writer should create file if it has written something to disk, after releasing hold.")]
		public static void ErrorMessageWriterShouldCreateFileIfItHasWrittenSomethingToDiskAfterReleasingHold()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				errorWriter.WriteErrorMessage(GetSomeErrorArguments().Object);
				errorWriter.ReleaseLock();
				Assert.IsTrue(File.Exists(path));
				Assert.IsTrue(errorWriter.FileCreated);
			}

			// Assert
			Assert.IsTrue(File.Exists(path));

			// Clean
			File.Delete(path);
		}

		[Test(Description = "Error message writer should create file if it has written something to disk, after releasing hold v2.")]
		public static void ErrorMessageWriterShouldCreateFileIfItHasWrittenSomethingToDiskAfterReleasingHoldV2()
		{
			// Arrange
			var path = Path.GetRandomFileName();

			// Act
			using (var errorWriter = new ErrorMessageWriter<IErrorArguments>(path))
			{
				// Assert
				errorWriter.WriteErrorMessage(GetSomeErrorArguments().Object);
				errorWriter.ReleaseLock();
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