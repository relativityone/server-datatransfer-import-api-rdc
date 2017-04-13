using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.Natives.Name;
using kCura.WinEDDS.Core.Export.Natives.Name.Factories;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class CustomFileNameProviderTest
	{
		private CustomFileNameProvider _subjectUnderTest;

		private ObjectExportInfo _exportObjectInfo;

		private Mock<IFileNamePartProviderContainer> _fileNamePartProviderContainerMock;

		[SetUp]
		public void Init()
		{
			_exportObjectInfo = new ObjectExportInfo();

			_fileNamePartProviderContainerMock = new Mock<IFileNamePartProviderContainer>();
		}

		[Test]
		public void ItShouldReturnFileNameTest()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			var secondDescriptor = new SeparatorDescriptorPart("");

			string firstPartName = "First";
			string secondPartName = "Second";

			Mock<IFileNamePartProvider> separatorProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> fieldProviderMock = new Mock<IFileNamePartProvider>();

			separatorProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo)).Returns(firstPartName);
			fieldProviderMock.Setup(mock => mock.GetPartName(secondDescriptor, _exportObjectInfo)).Returns(secondPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor)).Returns(separatorProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(secondDescriptor)).Returns(fieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor,
					secondDescriptor
				}, 
				_fileNamePartProviderContainerMock.Object);

			// Act
			string retFileName = _subjectUnderTest.GetName(_exportObjectInfo);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{secondPartName}"));
		}
	}
}
