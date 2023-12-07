using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    using global::NUnit.Framework;

    using kCura.WinEDDS;

    using Moq;

    using Relativity.DataExchange.Export.Natives.Name.Factories;

    public class FileNameProviderContainerFactoryTests
    {
        private FileNameProviderContainerFactory _factory;
        private IDictionary<ExportNativeWithFilenameFrom, IFileNameProvider> _fileNameProviders;

        [SetUp]
        public void Setup()
        {
            _fileNameProviders = new Dictionary<ExportNativeWithFilenameFrom, IFileNameProvider>
                                     {
                                         { ExportNativeWithFilenameFrom.Select, new Mock<IFileNameProvider>().Object },
                                         { ExportNativeWithFilenameFrom.Custom, new Mock<IFileNameProvider>().Object },
                                         { ExportNativeWithFilenameFrom.Identifier, new Mock<IFileNameProvider>().Object },
                                         { ExportNativeWithFilenameFrom.Production, new Mock<IFileNameProvider>().Object }
                                     };
            _factory = new FileNameProviderContainerFactory(_fileNameProviders);
        }

        [Test]
        public void ConstructorWithValidDictionaryInitializesFileNameProviders()
        {
            //Act
            var fieldInfo = typeof(FileNameProviderContainerFactory).GetField("_fileNameProviders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var FileProviders = (IDictionary<ExportNativeWithFilenameFrom, IFileNameProvider>)fieldInfo.GetValue(_factory);

            // Assert
            Assert.IsNotNull(_factory);
            Assert.IsNotNull(FileProviders);
        }

        [Test]
        public void CreateWithValidExportFileReturnsFileNameProviderContainer()
        {
            //Act
            IFileNameProvider result = _factory.Create(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<FileNameProviderContainer>(result);
        }
    }


}
