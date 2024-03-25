using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::NUnit.Framework;
using Relativity.DataExchange.Export.VolumeManagerV2.Directories;

namespace Relativity.DataExchange.Export.NUnit
{

    public class FilePathTransformerTests
    {
        [Test]
        public void TransformFilePath_WhenFilePathIsRelative_ReturnsAbsolutePath()
        {
            // Arrange
            string filePath = "C:\\temp\\file.txt";
            AbsoluteFilePathTransformer transformer = new AbsoluteFilePathTransformer();

            //Act
            string result = transformer.TransformPath(filePath);

            //Assert
            Assert.AreEqual(filePath, result);
        }
     }
}
