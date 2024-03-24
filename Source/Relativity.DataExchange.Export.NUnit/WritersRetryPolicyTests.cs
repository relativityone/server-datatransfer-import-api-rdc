using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global::NUnit.Framework;

    using kCura.WinEDDS;

    using Moq;

    using Relativity.DataExchange.Export.VolumeManagerV2.Metadata;

    [TestFixture]
    public class WritersRetryPolicyTests
    {

        [Test]
        public void CreateRetryPolicy_WhenCalled_ReturnsPolicy()
        {
            // Arrange
            var exportConfig = new ExportConfig();
            var writersRetryPolicy = new WritersRetryPolicy(exportConfig);

            // Act
            var result = writersRetryPolicy.CreateRetryPolicy((exception, timeSpan, retryCount, context) => { });

            // Assert
            Assert.IsNotNull(result);
        }
        [Test]
        public void SleepDurationProvider_WhenCalled_ReturnsTimeSpan()
        {
            // Arrange
            var exportConfig = new ExportConfig();
            var writersRetryPolicy = new WritersRetryPolicy(exportConfig);
            var retryAttempt = 3;

            // Act
            // Fix the accessibility issue by using reflection to invoke the SleepDurationProvider method
            var result = (TimeSpan)typeof(WritersRetryPolicy).GetMethod("SleepDurationProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(writersRetryPolicy, new object[] { retryAttempt });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TimeSpan>(result);
        }
        [Test]
        public void SleepDurationProvider_Returns_Zero()
        {
            // Arrange
            var exportConfig = new ExportConfig();
            var retryPolicy = new WritersRetryPolicy(exportConfig);

            // Act
            var result = (TimeSpan)typeof(WritersRetryPolicy).GetMethod("SleepDurationProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(retryPolicy, new object[] { 1 }); // Passing 1 to ensure we test the zero case

            // Assert
            Assert.AreEqual(TimeSpan.Zero, result);
        }

    }
}
