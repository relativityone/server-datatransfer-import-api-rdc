﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="IoWarningPublisherTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IoWarningPublisher"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
    using System;
    using System.Collections.Generic;

    using global::NUnit.Framework;

    using Relativity.Import.Export.Io;

    [TestFixture]
    public class IoWarningPublisherTests
	{
        private IoWarningPublisher ioWarningPublisher;
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
			this.ioWarningPublisher.PublishIoWarningEvent(new IoWarningEventArgs(message, lineNumber));
		}

		private void GivenTheMethodWhichHandlesTheEvent(EventHandler<IoWarningEventArgs> testEventHandler)
		{
			this.ioWarningPublisher.IoWarningEvent += testEventHandler;
		}

		private void GivenTheInstanceOfPublisher()
		{
			this.ioWarningPublisher = new IoWarningPublisher();
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