// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
    using System.Reflection;

    using global::NUnit.Framework;
    using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export;
	using Relativity.DataExchange.Service;

	public class ExportFileFormatterFactoryTests
	{
		private Mock<IFieldNameProvider> _fieldNameProviderMock;
		private ExportFileFormatterFactory _subjectUnderTest;

		[SetUp]
		public void Init()
		{
			this._fieldNameProviderMock = new Mock<IFieldNameProvider>();
			this._subjectUnderTest = new ExportFileFormatterFactory(this._fieldNameProviderMock.Object);
		}

		[TestCase(typeof(ExportFileFormatter), false)]
		[TestCase(typeof(HtmlExportFileFormatter), true)]
		public void ItShouldReturnCorrectFormatterType(Type formatterType, bool isHtml)
		{
			// ARRANGE
			ExportFile exportFile = new ExportFile((int)ArtifactType.Document)
			{
				LoadFileIsHtml = isHtml
			};

			// ACT
			ILoadFileHeaderFormatter retFormatter = this._subjectUnderTest.Create(exportFile);

			// ASSERT
			Assert.IsInstanceOf(formatterType, retFormatter);
		}

        [Test]
        public void Parameterless_Constructor_Initializes_FieldNameProvider_With_New_Instance()
        {
            // Act
            var factory = new ExportFileFormatterFactory();

            // Get the private _fieldNameProvider field using reflection
            var fieldInfo = typeof(ExportFileFormatterFactory).GetField("_fieldNameProvider", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.IsNotNull(factory); // Ensure constructor succeeded
            Assert.IsNotNull(fieldInfo.GetValue(factory)); // Ensure _fieldNameProvider is not null
            Assert.IsInstanceOf<FieldNameProvider>(fieldInfo.GetValue(factory)); // Ensure correct type
        }
    }
}