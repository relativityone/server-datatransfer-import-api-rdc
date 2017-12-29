using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Natives
{
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
			_loadFileCellFormatter = new Mock<ILoadFileCellFormatter>();
			_fieldsValue = new Mock<ILineFieldsValue>();
			_nativeFilePath = new Mock<ILineNativeFilePath>();

			NullLogger nullLogger = new NullLogger();
			LinePrefix linePrefix = new LinePrefix(_loadFileCellFormatter.Object, nullLogger);
			LineImageField lineImageField = new LineImageField(_loadFileCellFormatter.Object, nullLogger);
			LineSuffix lineSuffix = new LineSuffix(_loadFileCellFormatter.Object, nullLogger);

			_instance = new LoadFileLine(linePrefix, _fieldsValue.Object, lineImageField, _nativeFilePath.Object, lineSuffix, new LineNewLine(), nullLogger);
		}

		[Test]
		[TestCase("prefix", "fields_value", "image_cell", "native_file_path", "suffix")]
		[TestCase("", "", "", "", "")]
		[TestCase(null, null, null, null, null)]
		public void ItShouldPrepareLine(string prefix, string fieldsValue, string imageCell, string nativeFilePath, string suffix)
		{
			_loadFileCellFormatter.Setup(x => x.RowPrefix).Returns(prefix);
			_loadFileCellFormatter.Setup(x => x.CreateImageCell(It.IsAny<ObjectExportInfo>())).Returns(imageCell);
			_loadFileCellFormatter.Setup(x => x.RowSuffix).Returns(suffix);

			_fieldsValue.Setup(x => x.AddFieldsValue(It.IsAny<DeferredEntry>(), It.IsAny<ObjectExportInfo>()))
				.Callback((DeferredEntry l, ObjectExportInfo a) => l.AddStringEntry(fieldsValue));
			_nativeFilePath.Setup(x => x.AddNativeFilePath(It.IsAny<DeferredEntry>(), It.IsAny<ObjectExportInfo>()))
				.Callback((DeferredEntry l, ObjectExportInfo a) => l.AddStringEntry(nativeFilePath));

			//ACT
			ILoadFileEntry loadFileEntry = _instance.CreateLine(new ObjectExportInfo());

			//ASSERT
			DeferredEntry entry = loadFileEntry as DeferredEntry;
			Assert.That(entry, Is.Not.Null);

			string expectedResult = $"{prefix}{fieldsValue}{imageCell}{nativeFilePath}{suffix}{Environment.NewLine}";
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(entry), Is.EqualTo(expectedResult));
		}
	}
}