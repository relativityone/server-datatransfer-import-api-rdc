// -----------------------------------------------------------------------------------------------------
// <copyright file="ObjectManagerExceptionHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ObjectManagerExceptionHelper"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Castle.Core.Internal;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.Services.Exceptions;

	/// <summary>
	/// Represents <see cref="ObjectManagerExceptionHelper"/> tests.
	/// </summary>
	[TestFixture]
	public static class ObjectManagerExceptionHelperTests
	{
		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.NonFatalInvalidParameterErrors))]
		public static void ShouldGetTheNonFatalInvalidParameterError(Exception exception)
		{
			// ACT/ASSERT
			VerifyResults(exception, true, true, false, false, false);
		}

		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.NonFatalServerSideDirectoryNotFoundErrors))]
		public static void ShouldGetTheNonFatalServerSideDirectoryNotFoundError(Exception exception)
		{
			// ACT/ASSERT
			VerifyResults(exception, true, false, true, false, false);
		}

		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.NonFatalServerSideFileNotFoundErrors))]
		public static void ShouldGetTheNonFatalServerSideFileNotFoundError(Exception exception)
		{
			// ACT/ASSERT
			VerifyResults(exception, true, false, false, true, false);
		}

		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.NonFatalServerSideFilePermissionErrors))]
		public static void ShouldGetTheNonFatalServerSideFilePermissionError(Exception exception)
		{
			// ACT/ASSERT
			VerifyResults(exception, true, false, false, false, true);
		}

		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.NotObjectManagerErrors))]
		public static void ShouldHandleNonObjectManagerError(Exception exception)
		{
			// ACT/ASSERT
			VerifyResults(exception, false, false, false, false, false);
		}

		[Test]
		public static void ShouldNotFailOnDeepInnerExceptionDepths()
		{
			// ARRANGE
			Stack<ServiceException> stack = new Stack<ServiceException>();
			stack.Push(new ServiceException("Failed", new InvalidOperationException()));
			Enumerable.Range(0, ObjectManagerExceptionHelper.MaxInnerExceptionDepth + 1).ForEach(x => stack.Push(new ServiceException("Failed", stack.Peek())));
			ServiceException top = stack.Peek();
			ServiceException exception = ObjectManagerExceptionTestData.SerializeErrorDetails(top);

			// ACT/ASSERT
			VerifyResults(exception, false, false, false, false, false);
		}

		private static void VerifyResults(
			Exception exception,
			bool nonFatalError,
			bool invalidParameters,
			bool serverSideDirectoryNotFound,
			bool serverSideFileNotFound,
			bool serverSideFilePermissions)
		{
			Assert.That(ObjectManagerExceptionHelper.IsInvalidParametersError(exception), Is.EqualTo(invalidParameters));
			Assert.That(ObjectManagerExceptionHelper.IsServerSideDirectoryNotFoundError(exception), Is.EqualTo(serverSideDirectoryNotFound));
			Assert.That(ObjectManagerExceptionHelper.IsServerSideFileNotFoundError(exception), Is.EqualTo(serverSideFileNotFound));
			Assert.That(ObjectManagerExceptionHelper.IsServerSideFilePermissionsError(exception), Is.EqualTo(serverSideFilePermissions));
			Assert.That(ObjectManagerExceptionHelper.IsNonFatalError(exception), Is.EqualTo(nonFatalError));
		}
	}
}