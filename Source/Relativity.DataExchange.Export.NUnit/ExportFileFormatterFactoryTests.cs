﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

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
			// Arrange
			ExportFile exportFile = new ExportFile((int)ArtifactType.Document)
			{
				LoadFileIsHtml = isHtml
			};

			// Act
			ILoadFileHeaderFormatter retFormatter = this._subjectUnderTest.Create(exportFile);

			// Assert
			Assert.IsInstanceOf(formatterType, retFormatter);
		}

        [Test]
        public void ConstructorWithoutParametersShouldInitializeFieldNameProvider()
        {
            //ARRANGE
            var factory = new ExportFileFormatterFactory();

            // ACT & Assert
            var fieldNameProviderField = typeof(ExportFileFormatterFactory)
                .GetField("_fieldNameProvider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            var fieldValue = fieldNameProviderField.GetValue(factory);

            Assert.IsNotNull(fieldValue);
            Assert.IsInstanceOf<IFieldNameProvider>(fieldValue); 
            Assert.IsInstanceOf<FieldNameProvider>(fieldValue); 
        }

    }
}