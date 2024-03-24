using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    [TestFixture]
    public class ImagesRollupTests
    {
        [Test]
        public void RollupImages_DoesNotThrowException_WhenUsingImplementation()
        {
            // Arrange
            var imagesRollup = new EmptyImagesRollup(); // Using the implementation for testing

            // Act & Assert
            Assert.DoesNotThrow(() => imagesRollup.RollupImages(new ObjectExportInfo()));
        }

        [Test]
        public void RollupImages_DoesNotThrowException_WhenUsingMock()
        {
            // Arrange
            var mockArtifact = new Mock<ObjectExportInfo>(); // Using a mock for ObjectExportInfo
            var imagesRollupMock = new Mock<IImagesRollup>(); // Using a mock for IImagesRollup

            // Act & Assert
            Assert.DoesNotThrow(() => imagesRollupMock.Object.RollupImages(mockArtifact.Object));
        }
    }
}
