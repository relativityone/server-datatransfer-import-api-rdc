using System;
using System.Collections.Generic;
using kCura.Config;
using kCura.WinEDDS.Api;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;
using Renci.SshNet;

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    [TestFixture]
    public class IoWarningPublisherTests
	{
        private IoWarningPublisher _ioWarningPublisher;
		private Dictionary<long, string> _results;
       
        private const string _FILE_NAME = "TestFileName";

        [SetUp]
        public void Setup()
        {
			_results = new Dictionary<long, string>();
        }

        [TestCase("Event message", 10)]
        [TestCase("Event message from huge load file", 3000000000)]
        public void ItShouldGetFileLength(string message, long lineNubmer)
        {
            //Arrange
	        GivenTheInstanceOfPublisher();
	        GivenTheMethodWhichHandlesTheEvent(TestEventHandler);

            //Act
	        WhenEventOccursWithMessageAndLineNumber(message, lineNubmer);

            //Assert
	        ResultsDictionaryContains(message, lineNubmer);
        }

		

		#region helper methods
		private void WhenEventOccursWithMessageAndLineNumber(string message, long lineNumber)
		{
			_ioWarningPublisher.OnIoWarningEvent(new IoWarningEventArgs(message, lineNumber));
		}

		private void GivenTheMethodWhichHandlesTheEvent(IoWarningPublisher.IoWarningEventHandler testEventHandler)
		{
			_ioWarningPublisher.IoWarningEvent += testEventHandler;
		}

		private void GivenTheInstanceOfPublisher()
		{
			_ioWarningPublisher = new IoWarningPublisher();
		}

		void TestEventHandler(IoWarningEventArgs eventArgs)
		{
			_results.Add(eventArgs.CurrentLineNumber, eventArgs.Message);
		}

		private void ResultsDictionaryContains(string message, long lineNubmer)
		{
			Assert.That(_results.Count == 1);
			Assert.That(_results.ContainsKey(lineNubmer));
			Assert.That(_results.ContainsValue(message));
		}
		#endregion
	}
}
