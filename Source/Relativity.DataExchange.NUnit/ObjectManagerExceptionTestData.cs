// -----------------------------------------------------------------------------------------------------
// <copyright file="ObjectManagerExceptionTestData.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents Object Manager specific error data that can be used by unit tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Relativity.Services.Exceptions;

	/// <summary>
	/// Represents Object Manager specific error data that can be used by unit tests.
	/// </summary>
	internal static class ObjectManagerExceptionTestData
	{
		private const string SampleFilePath1 =
			@"\\files\T005\Files\EDDS1028050\relativity_vm-t005-files1_edds1028050_10\Fields.ExtractedText\1bd\d93\32abc8ab-f438-489f-b191-102dc85576e4.txt";

		private const string SampleFilePath2 =
			@"/files/T005/Files/EDDS1028050/relativity_vm-t005-files1_edds1028050_10/Fields.ExtractedText/1bd/d93/32abc8ab-f438-489f-b191-102dc85576e4.txt";

		public static IEnumerable<Exception> AllExpectedObjectManagerErrors
		{
			get
			{
				List<Exception> exceptions = new List<Exception>();
				exceptions.AddRange(NonFatalInvalidParameterErrors);
				exceptions.AddRange(NonFatalServerSideDirectoryNotFoundErrors);
				exceptions.AddRange(NonFatalServerSideFileNotFoundErrors);
				exceptions.AddRange(NonFatalServerSideFilePermissionErrors);
				return exceptions;
			}
		}

		public static IEnumerable<Exception> NonFatalInvalidParameterErrors
		{
			get
			{
				// Field not found
				yield return new ValidationException();
				yield return new ValidationException("Error");
				yield return new ValidationException("Error1", new ServiceException("Error2"));

				// General
				yield return new InvalidInputException();
				yield return new InvalidInputException("Invalid Parameter");
				yield return new InvalidInputException("Invalid Parameter1", new InvalidInputException("Invalid Parameter2"));

				// Object not found
				yield return new ServiceException("Read Failed");
				yield return new ServiceException("read failed", new ServiceException("Read Failed2"));
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Naming",
			"CA2204:Literals should be spelled correctly",
			Justification = "These messages are OK.")]
		public static IEnumerable<Exception> NonFatalServerSideDirectoryNotFoundErrors
		{
			get
			{
				foreach (string filePath in new[] { SampleFilePath1, SampleFilePath2 })
				{
					string errorMessage = $@"Could not find a part of the path '{filePath}'";
					yield return new ServiceException(errorMessage);
					yield return new ServiceException(errorMessage.ToLowerInvariant());
					yield return new ServiceException(errorMessage.ToUpperInvariant());
					yield return new ServiceException(errorMessage + ".");
					string messageExtended1 =
						$@"DataGrid action have failed after maximum number of retries. Total number of tries: 3 -> Error reading file -> {errorMessage}";
					yield return new ServiceException(messageExtended1);
					yield return new ServiceException(messageExtended1.ToLowerInvariant());
					yield return new ServiceException(messageExtended1.ToUpperInvariant());
					yield return new ServiceException(messageExtended1 + ".");
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Naming",
			"CA2204:Literals should be spelled correctly",
			Justification = "These messages are required.")]
		public static IEnumerable<Exception> NonFatalServerSideFileNotFoundErrors
		{
			get
			{
				foreach (string filePath in new[] { SampleFilePath1, SampleFilePath2 })
				{
					string errorMessage = $@"Could not find file '{filePath}'";
					yield return new ServiceException(errorMessage);
					yield return new ServiceException(errorMessage.ToLowerInvariant());
					yield return new ServiceException(errorMessage.ToUpperInvariant());
					yield return new ServiceException(errorMessage + ".");
					string messageExtended = $@"DataGrid action have failed after maximum number of retries. Total number of tries: 3 -> Error reading file -> {errorMessage}";
					yield return new ServiceException(messageExtended);
					yield return new ServiceException(messageExtended.ToLowerInvariant());
					yield return new ServiceException(messageExtended.ToUpperInvariant());
					yield return new ServiceException(messageExtended + ".");
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Naming",
			"CA2204:Literals should be spelled correctly",
			Justification = "These messages are required.")]
		public static IEnumerable<Exception> NonFatalServerSideFilePermissionErrors
		{
			get
			{
				foreach (string filePath in new[] { SampleFilePath1, SampleFilePath2 })
				{
					string errorMessage = $@"Access to the path '{filePath}' is denied";
					yield return new ServiceException(errorMessage);
					yield return new ServiceException(errorMessage + ".");
					string messageExtended = $@"Error during readchunk -> {errorMessage}";
					yield return new ServiceException(messageExtended);
					yield return new ServiceException(messageExtended + ".");
				}
			}
		}

		public static IEnumerable<Exception> NotObjectManagerErrors
		{
			get
			{
				List<Exception> exceptions = new List<Exception>();
				exceptions.AddRange(
					ExceptionHelper.FatalExceptionCandidates.Where(x => x != typeof(ThreadAbortException))
						.Concat(ExceptionHelper.FatalKeplerExceptionCandidates).Select(Activator.CreateInstance)
						.Cast<Exception>());
				exceptions.AddRange(
					new[]
						{
							new ServiceException(), new ServiceException("General error"),
							new ServiceException("General error", new ServiceException("Some inner exception")),
							new ConflictException(), new ConflictException("Conflict error"),
							new ConflictException("Conflict error", new ServiceException("Some inner exception")),
							new DataConcurrencyException(), new DataConcurrencyException("Data concurrency error"),
							new DataConcurrencyException(
								"Data concurrency error",
								new ServiceException("Some inner exception")),
							new NotFoundException(), new NotFoundException("Not found error"),
							new NotFoundException("Not found error", new ServiceException("Some inner exception")),
						});
				return exceptions;
			}
		}
	}
}