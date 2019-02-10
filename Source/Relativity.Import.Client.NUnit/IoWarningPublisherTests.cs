// -----------------------------------------------------------------------------------------------------
// <copyright file="IoReporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IoWarningPublisher"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
    using System;
    using System.Collections.Generic;

    using kCura.WinEDDS.TApi;

    using global::NUnit.Framework;
    
    [TestFixture]
    public class IoWarningPublisherTests
	{
        private IoWarningPublisher _ioWarningPublisher;
		private Dictionary<long, string> _results;

        [SetUp]
        public void Setup()
        {
			_results = new Dictionary<long, string>();
        }

        [TestCase("Event message", 10)]
        [TestCase("Event message from huge load file", 3000000000)]
        public void ItShouldPublishWarningMessage(string message, long lineNubmer)
        {
	        GivenTheInstanceOfPublisher();
	        GivenTheMethodWhichHandlesTheEvent(TestEventHandler);

	        WhenEventOccursWithMessageAndLineNumber(message, lineNubmer);

	        ResultsDictionaryContains(message, lineNubmer);
        }
		
		private void WhenEventOccursWithMessageAndLineNumber(string message, long lineNumber)
		{
			_ioWarningPublisher.PublishIoWarningEvent(new IoWarningEventArgs(message, lineNumber));
		}

		private void GivenTheMethodWhichHandlesTheEvent(EventHandler<IoWarningEventArgs> testEventHandler)
		{
			_ioWarningPublisher.IoWarningEvent += testEventHandler;
		}

		private void GivenTheInstanceOfPublisher()
		{
			_ioWarningPublisher = new IoWarningPublisher();
		}

		void TestEventHandler(object sender, IoWarningEventArgs eventArgs)
		{
			_results.Add(eventArgs.CurrentLineNumber, eventArgs.Message);
		}

		private void ResultsDictionaryContains(string message, long lineNubmer)
		{
			Assert.That(_results.Count == 1);
			Assert.That(_results.ContainsKey(lineNubmer));
			Assert.That(_results.ContainsValue(message));
		}
	}
}