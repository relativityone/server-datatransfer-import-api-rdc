﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LogExtensionsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="LogExtensions"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Logging;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	[TestFixture]
	public class LogExtensionsTests
	{
		private Mock<Relativity.Logging.ILog> logger;
		private IDictionary<string, string> dictionary;

		[SetUp]
		public void Setup()
		{
			this.logger = new Mock<ILog>();
			this.dictionary = new Dictionary<string, string>();
			this.logger.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>())).Callback(
				(string messageTemplate, object[] propertyValues) =>
					{
						if (propertyValues != null && propertyValues.Length == 1)
						{
							this.dictionary = propertyValues[0] as IDictionary<string, string>
							                  ?? new Dictionary<string, string>();
						}
					});
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldLogTheClassObjectAsDictionary()
		{
			Relativity.DataExchange.Service.CaseInfo value =
				new Relativity.DataExchange.Service.CaseInfo { Name = "MyWorkspace", ArtifactID = 1234567 };
#pragma warning disable CS0618 // Type or member is obsolete
			this.logger.Object.LogObjectAsDictionary("The {@Value}", value);
#pragma warning restore CS0618 // Type or member is obsolete
			Assert.That(this.dictionary["Name"], Is.EqualTo("MyWorkspace"));
			Assert.That(this.dictionary["ArtifactID"], Is.EqualTo("1234567"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldLogTheFilteredClassObjectAsDictionary()
		{
			Relativity.DataExchange.Service.ErrorFileKey value =
				new Relativity.DataExchange.Service.ErrorFileKey { LogKey = "1", OpticonKey = "2" };
#pragma warning disable CS0618 // Type or member is obsolete
			this.logger.Object.LogObjectAsDictionary(
				"The {@Value}",
				value,
				info => !info.Name.Equals(nameof(value.OpticonKey)));
#pragma warning restore CS0618 // Type or member is obsolete
			Assert.That(this.dictionary["LogKey"], Is.EqualTo("1"));
			Assert.That(this.dictionary.ContainsKey("OpticonKey"), Is.False);
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldLogTheAnonymousObjectAsDictionary()
		{
			var value = new { FirstName = "Bob", LastName = "Smith" };
#pragma warning disable CS0618 // Type or member is obsolete
			this.logger.Object.LogObjectAsDictionary("The {@Value}", value);
#pragma warning restore CS0618 // Type or member is obsolete
			Assert.That(this.dictionary["FirstName"], Is.EqualTo("Bob"));
			Assert.That(this.dictionary["LastName"], Is.EqualTo("Smith"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldLogTheStructAsDictionary()
		{
			DateTime value = new DateTime(2000, 3, 9, 6, 10, 33);
#pragma warning disable CS0618 // Type or member is obsolete
			this.logger.Object.LogObjectAsDictionary("The {@Value}", value);
#pragma warning restore CS0618 // Type or member is obsolete
			Assert.That(this.dictionary["Year"], Is.EqualTo("2000"));
			Assert.That(this.dictionary["Month"], Is.EqualTo("3"));
			Assert.That(this.dictionary["Day"], Is.EqualTo("9"));
			Assert.That(this.dictionary["Hour"], Is.EqualTo("6"));
			Assert.That(this.dictionary["Minute"], Is.EqualTo("10"));
			Assert.That(this.dictionary["Second"], Is.EqualTo("33"));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		[Category(TestCategories.Framework)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2201:DoNotRaiseReservedExceptionTypes",
			Justification = "This is used strictly for testing purposes.")]
		public void ShouldHandleTheThrownException(bool fatal)
		{
			this.logger.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>())).Throws(
				fatal ? (Exception)new OutOfMemoryException() : new InvalidOperationException());
			if (fatal)
			{
				Assert.Throws<OutOfMemoryException>(
#pragma warning disable CS0618 // Type or member is obsolete
					() => this.logger.Object.LogObjectAsDictionary("The {@Value}", DateTime.Now));
#pragma warning restore CS0618 // Type or member is obsolete
			}
			else
			{
#pragma warning disable CS0618 // Type or member is obsolete
				this.logger.Object.LogObjectAsDictionary("The {@Value}", DateTime.Now);
#pragma warning restore CS0618 // Type or member is obsolete
				this.logger.Verify(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));
			}
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ItShouldTrackCorrelationId()
		{
			// Arrange
			const int WkspId = 1234;
			const string CorrelationId = "SomeId";

			// Act
			var logContext = new LogContext(CorrelationId, WkspId);
			this.logger.Object.LogImportContextPushProperties(logContext);

			// Assert
			this.logger.Verify(x => x.LogContextPushProperty($"DataExchange.Import.{nameof(logContext.RunId)}", CorrelationId), Times.Once);
			this.logger.Verify(x => x.LogContextPushProperty($"DataExchange.Import.{nameof(logContext.WorkspaceId)}", WkspId.ToString()), Times.Once);
		}
	}
}