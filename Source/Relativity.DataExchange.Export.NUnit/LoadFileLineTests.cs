﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LoadFileLineTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.Logging;

	[TestFixture]
	public class LoadFileLineTests
	{
		private LoadFileLine _instance;

		private Mock<ILoadFileCellFormatter> _loadFileCellFormatter;
		private Mock<ILineFieldsValue> _fieldsValue;
		private Mock<ILineNativeFilePath> _nativeFilePath;

		[SetUp]
		public void SetUp()
		{
			this._loadFileCellFormatter = new Mock<ILoadFileCellFormatter>();
			this._fieldsValue = new Mock<ILineFieldsValue>();
			this._nativeFilePath = new Mock<ILineNativeFilePath>();

			NullLogger nullLogger = new NullLogger();
			LinePrefix linePrefix = new LinePrefix(this._loadFileCellFormatter.Object, nullLogger);
			LineImageField lineImageField = new LineImageField(this._loadFileCellFormatter.Object, nullLogger);
			LineSuffix lineSuffix = new LineSuffix(this._loadFileCellFormatter.Object, nullLogger);

			this._instance = new LoadFileLine(linePrefix, this._fieldsValue.Object, lineImageField, this._nativeFilePath.Object, lineSuffix, new LineNewLine(), nullLogger);
		}

		[Test]
		[TestCase("prefix", "fields_value", "image_cell", "native_file_path", "suffix")]
		[TestCase("", "", "", "", "")]
		[TestCase(null, null, null, null, null)]
		public void ItShouldPrepareLine(string prefix, string fieldsValue, string imageCell, string nativeFilePath, string suffix)
		{
			this._loadFileCellFormatter.Setup(x => x.RowPrefix).Returns(prefix);
			this._loadFileCellFormatter.Setup(x => x.CreateImageCell(It.IsAny<ObjectExportInfo>())).Returns(imageCell);
			this._loadFileCellFormatter.Setup(x => x.RowSuffix).Returns(suffix);

			this._fieldsValue.Setup(x => x.AddFieldsValue(It.IsAny<DeferredEntry>(), It.IsAny<ObjectExportInfo>()))
				.Callback((DeferredEntry l, ObjectExportInfo a) => l.AddStringEntry(fieldsValue));
			this._nativeFilePath.Setup(x => x.AddNativeFilePath(It.IsAny<DeferredEntry>(), It.IsAny<ObjectExportInfo>()))
				.Callback((DeferredEntry l, ObjectExportInfo a) => l.AddStringEntry(nativeFilePath));

			// ACT
			ILoadFileEntry loadFileEntry = this._instance.CreateLine(new ObjectExportInfo());

			// ASSERT
			DeferredEntry entry = loadFileEntry as DeferredEntry;
			Assert.That(entry, Is.Not.Null);

			string expectedResult = $"{prefix}{fieldsValue}{imageCell}{nativeFilePath}{suffix}{Environment.NewLine}";
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(entry), Is.EqualTo(expectedResult));
		}
	}
}