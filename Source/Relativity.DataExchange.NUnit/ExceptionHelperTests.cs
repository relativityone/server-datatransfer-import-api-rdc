// -----------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ExceptionHelper"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;
	using System.Security;

	using global::NUnit.Framework;

	using Relativity.Kepler.Exceptions;
	using Relativity.Services.Exceptions;

	/// <summary>
	/// Represents <see cref="ExceptionHelper"/> tests.
	/// </summary>
	[TestFixture]
	public class ExceptionHelperTests
	{
		public static IEnumerable FatalGeneralExceptionTypeTestCases
		{
			get
			{
				yield return new TestCaseData(new IOException("disk full1", ExceptionHelper.DiskFullHResultHResult));
				yield return new TestCaseData(new IOException("disk full2", ExceptionHelper.HandleDiskFullHResult));
				yield return new TestCaseData(new InheritedIOException("disk full1", ExceptionHelper.DiskFullHResultHResult));
				yield return new TestCaseData(new InheritedIOException("disk full2", ExceptionHelper.HandleDiskFullHResult));
				yield return new TestCaseData(new AccessViolationException());
				yield return new TestCaseData(new InheritedAccessViolationException());
				yield return new TestCaseData(new ApplicationException());
				yield return new TestCaseData(new InheritedApplicationException());
				yield return new TestCaseData(new BadImageFormatException());
				yield return new TestCaseData(new InheritedBadImageFormatException());
				yield return new TestCaseData(new DivideByZeroException());
				yield return new TestCaseData(new DllNotFoundException());
				yield return new TestCaseData(new EntryPointNotFoundException());
				yield return new TestCaseData(new InsufficientMemoryException());
				yield return new TestCaseData(new NullReferenceException());
				yield return new TestCaseData(new OutOfMemoryException());
				yield return new TestCaseData(new OverflowException());
				yield return new TestCaseData(new SecurityException());
				yield return new TestCaseData(new StackOverflowException());
				yield return new TestCaseData(new FileNotFoundException());
            }
		}

		public static IEnumerable NotFatalGeneralExceptionTypeTestCases
		{
			get
			{
				yield return new TestCaseData(new Exception());
				yield return new TestCaseData(new ArgumentException("TEST"));
				yield return new TestCaseData(new SomeException());
			}
		}

		public static IEnumerable FatalKeplerExceptionTypeTestCases
		{
			get
			{
				yield return new TestCaseData(new NotAuthorizedException());
				yield return new TestCaseData(new InheritedNotAuthorizedException());
				yield return new TestCaseData(new WireProtocolMismatchException());
				yield return new TestCaseData(new InheritedWireProtocolMismatchException());
			}
		}

		public static IEnumerable NonFatalKeplerExceptionTypeTestCases
		{
			get
			{
				yield return new TestCaseData(new Exception());
				yield return new TestCaseData(new ArgumentException("TEST"));
				yield return new TestCaseData(new SomeException());
			}
		}

		[Test]
		[TestCaseSource(nameof(FatalGeneralExceptionTypeTestCases))]
		public void ShouldBeFatalGeneralException(Exception exception)
		{
			// ACT
			bool result = ExceptionHelper.IsFatalException(exception);

			// ASSERT
			Assert.That(result, Is.True);
		}

		[Test]
		[TestCaseSource(nameof(NotFatalGeneralExceptionTypeTestCases))]
		public void ShouldNotBeFatalGeneralException(Exception exception)
		{
			// ACT
			bool result = ExceptionHelper.IsFatalException(exception);

			// ASSERT
			Assert.That(result, Is.False);
		}

		[Test]
		[TestCaseSource(nameof(FatalKeplerExceptionTypeTestCases))]
		public void ShouldBeFatalKeplerException(Exception exception)
		{
			// ACT
			bool result = ExceptionHelper.IsFatalKeplerException(exception);

			// ASSERT
			Assert.That(result, Is.True);
		}

		[Test]
		[TestCaseSource(nameof(NonFatalKeplerExceptionTypeTestCases))]
		public void ShouldBeNonFatalKeplerException(Exception exception)
		{
			// ACT
			bool result = ExceptionHelper.IsFatalKeplerException(exception);

			// ASSERT
			Assert.That(result, Is.False);
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedIOException : IOException
		{
			public InheritedIOException(string message, int result)
				: base(message, result)
			{
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedAccessViolationException : AccessViolationException
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1058:TypesShouldNotExtendCertainBaseTypes", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedApplicationException : ApplicationException
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedBadImageFormatException : BadImageFormatException
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class SomeException : Exception
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedNotAuthorizedException : NotAuthorizedException
		{
		}

		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "TestClass")]
		[SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "TestClass")]
		private class InheritedWireProtocolMismatchException : WireProtocolMismatchException
		{
		}
	}
}