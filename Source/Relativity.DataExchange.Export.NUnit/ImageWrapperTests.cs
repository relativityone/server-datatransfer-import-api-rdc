using Moq;
using NUnit.Framework;
using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
using Relativity.DataExchange.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    [TestFixture]
    public class ImageWrapperTests
    {
        [Test]
        public void ConvertImagesToMultiPagePdf_Calls_ImageConverter()
        {
            // Arrange
            var mockImageConverter = new Mock<IImageConverter>();
            var imageWrapper = new ImageWrapper(mockImageConverter.Object);
            string[] inputFiles = new string[] { "file1.jpg", "file2.jpg" };
            string outputFile = "output.pdf";

            // Act
            imageWrapper.ConvertImagesToMultiPagePdf(inputFiles, outputFile);

            // Assert
            mockImageConverter.Verify(x => x.ConvertImagesToMultiPagePdf(inputFiles, outputFile), Times.Once);
        }

        [Test]
        public void ConvertTIFFsToMultiPage_Calls_ImageConverter()
        {
            // Arrange
            var mockImageConverter = new Mock<IImageConverter>();
            var imageWrapper = new ImageWrapper(mockImageConverter.Object);
            string[] inputFiles = new string[] { "file1.tiff", "file2.tiff" };
            string outputFile = "output.tif";

            // Act
            imageWrapper.ConvertTIFFsToMultiPage(inputFiles, outputFile);

            // Assert
            mockImageConverter.Verify(x => x.ConvertTiffsToMultiPageTiff(inputFiles, outputFile), Times.Once);
        }
    }
}
