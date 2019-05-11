// -----------------------------------------------------------------------------------------------------
// <copyright file="IoReporterContextTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IoWarningPublisher"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Io;

	[TestFixture]
	public class IoReporterContextTests
	{
		private IoReporterContext context;
		private Dictionary<long, string> results;

		[SetUp]
		public void Setup()
		{
			this.results = new Dictionary<long, string>();
		}

		[TestCase("Event message", 10)]
		[TestCase("Event message from huge load file", 3000000000)]
		public void ItShouldPublishWarningMessage(string message, long lineNubmer)
		{
			this.GivenTheInstanceOfPublisher();
			this.GivenTheMethodWhichHandlesTheEvent(this.TestEventHandler);
			this.WhenEventOccursWithMessageAndLineNumber(message, lineNubmer);
			this.ResultsDictionaryContains(message, lineNubmer);
		}

		private void WhenEventOccursWithMessageAndLineNumber(string message, long lineNumber)
		{
			this.context.PublishIoWarningEvent(new IoWarningEventArgs(message, lineNumber));
		}

		private void GivenTheMethodWhichHandlesTheEvent(EventHandler<IoWarningEventArgs> testEventHandler)
		{
			this.context.IoWarningEvent += testEventHandler;
		}

		private void GivenTheInstanceOfPublisher()
		{
			this.context = new IoReporterContext();
		}

		private void TestEventHandler(object sender, IoWarningEventArgs eventArgs)
		{
			this.results.Add(eventArgs.CurrentLineNumber, eventArgs.Message);
		}

		private void ResultsDictionaryContains(string message, long lineNubmer)
		{
			Assert.That(this.results.Count == 1);
			Assert.That(this.results.ContainsKey(lineNubmer));
			Assert.That(this.results.ContainsValue(message));
		}
	}
}