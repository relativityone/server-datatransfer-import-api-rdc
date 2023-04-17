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
	using System.Dynamic;
	using System.Linq;
	using System.Threading;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;

	using Relativity.Services.Exceptions;
	using Relativity.Services.Objects.Exceptions;

	/// <summary>
	/// Represents Object Manager specific error data that can be used by unit tests.
	/// </summary>
	internal static class ObjectManagerExceptionTestData
	{
		private const string SampleFilePath =
			@"\\files\T005\Files\EDDS1028050\relativity_vm-t005-files1_edds1028050_10\Fields.ExtractedText\1bd\d93\32abc8ab-f438-489f-b191-102dc85576e4.txt";

		private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
			                                                                              {
				                                                                              Formatting = Formatting.Indented,
				                                                                              ContractResolver =
					                                                                              new DefaultContractResolver(),
				                                                                              Converters =
					                                                                              new List<JsonConverter>(
						                                                                              new[]
							                                                                              {
								                                                                              new
									                                                                              ExpandoObjectConverter(),
							                                                                              }),
			                                                                              };

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
				yield return SerializeErrorDetails(new ValidationException());
				yield return SerializeErrorDetails(new ValidationException("Error"));
				yield return SerializeErrorDetails(new ValidationException("Error1", new ServiceException("Error2")));

				yield return SerializeErrorDetails(new Relativity.Services.Objects.Exceptions.PermissionDeniedException());
				yield return SerializeErrorDetails(new Relativity.Services.Objects.Exceptions.PermissionDeniedException("Invalid Permissions"));
				yield return SerializeErrorDetails(new Relativity.Services.Objects.Exceptions.PermissionDeniedException("Invalid Permissions", new ServiceException("Invalid Permissions 2")));

				// General
				yield return SerializeErrorDetails(new InvalidInputException());
				yield return SerializeErrorDetails(new InvalidInputException("Invalid Parameter"));
				yield return SerializeErrorDetails(
					new InvalidInputException("Invalid Parameter1", new InvalidInputException("Invalid Parameter2")));

				// Object not found
				yield return SerializeErrorDetails(new ServiceException("Read Failed"));
				yield return SerializeErrorDetails(new ServiceException("read failed", new ServiceException("Read Failed2")));
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
				string errorMessage = $@"Could not find a part of the path '{SampleFilePath}'";
				yield return SerializeErrorDetails(
					new ServiceException("Failed", new System.IO.DirectoryNotFoundException(errorMessage)));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException("Failed", new System.IO.DirectoryNotFoundException(errorMessage))));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException(
							"Failed",
							new ServiceException("Failed", new System.IO.DirectoryNotFoundException(errorMessage)))));
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
				string errorMessage = $@"Could not find file '{SampleFilePath}'";
				yield return SerializeErrorDetails(
					new ServiceException("Failed", new System.IO.FileNotFoundException(errorMessage)));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException("Failed", new System.IO.FileNotFoundException(errorMessage))));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException(
							"Failed",
							new ServiceException("Failed", new System.IO.FileNotFoundException(errorMessage)))));
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
				string errorMessage = $@"Access to the path '{SampleFilePath}' is denied";
				yield return SerializeErrorDetails(
					new ServiceException("Failed", new System.UnauthorizedAccessException(errorMessage)));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException("Failed", new System.UnauthorizedAccessException(errorMessage))));
				yield return SerializeErrorDetails(
					new ServiceException(
						"Failed",
						new ServiceException(
							"Failed",
							new ServiceException("Failed", new System.UnauthorizedAccessException(errorMessage)))));
			}
		}

		public static IEnumerable<Exception> NotObjectManagerErrors
		{
			get
			{
				List<Exception> exceptions = new List<Exception>();
				exceptions.AddRange(
					ExceptionHelper.FatalExceptionCandidates
						.Where(x => x != typeof(ThreadAbortException))
						.Select(Activator.CreateInstance)
						.Cast<Exception>());

				// PermissionDeniedException is a part of test NonFatalInvalidParameterErrors and is handled the same as ValidationException
				// we exclude it from the list to keep the logic of ObjectManagerExceptionHelper unchanged
				exceptions.AddRange(
					ExceptionHelper.FatalKeplerExceptionCandidates
						.Where(x => x != typeof(Relativity.Services.Objects.Exceptions.PermissionDeniedException))
						.Select(Activator.CreateInstance)
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

		public static ServiceException SerializeErrorDetails(ServiceException exception)
		{
			// Note: this attempts to simulate how ServiceException objects are serialized by Kepler.
			ServiceException currentServiceException = exception;
			while (currentServiceException != null)
			{
				if (currentServiceException.ErrorDetails is Exception)
				{
					// Note: this is incredibly slow!
					string json = JsonConvert.SerializeObject(currentServiceException.ErrorDetails, DefaultJsonSerializerSettings);
					currentServiceException.ErrorDetails =
						JsonConvert.DeserializeObject<ExpandoObject>(json, DefaultJsonSerializerSettings);
				}

				currentServiceException = currentServiceException.InnerException as ServiceException;
			}

			return exception;
		}
	}
}