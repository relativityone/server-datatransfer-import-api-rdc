using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Text.Repository
{
	[TestFixture]
	public class LongTextRepositoryBuilderTests
	{
		private LongTextRepositoryBuilder _instance;

		private LongTextRepository _longTextRepository;

		private Mock<ILongTextBuilder> _longTextPrecedenceBuilder;
		private Mock<ILongTextBuilder> _longTextFieldBuilder;
		private Mock<ILongTextBuilder> _longTextIproFullTextBuilder;

		[SetUp]
		public void SetUp()
		{
			_longTextPrecedenceBuilder = new Mock<ILongTextBuilder>();
			_longTextFieldBuilder = new Mock<ILongTextBuilder>();
			_longTextIproFullTextBuilder = new Mock<ILongTextBuilder>();

			_longTextRepository = new LongTextRepository(null, new NullLogger());

			_instance = new LongTextRepositoryBuilder(_longTextPrecedenceBuilder.Object, _longTextFieldBuilder.Object, _longTextIproFullTextBuilder.Object, _longTextRepository,
				new NullLogger());
		}

		[Test]
		public void ItShouldAddAllDistinctLongTexts()
		{
			LongText longText1 = LongText.CreateFromExistingValue(1, 10, "text");
			LongText longText2 = LongText.CreateFromExistingValue(2, 20, "text");
			LongText longText3 = LongText.CreateFromExistingValue(3, 30, "text");
			LongText longText4 = LongText.CreateFromExistingValue(4, 40, "text");
			LongText longText5 = LongText.CreateFromExistingValue(5, 50, "text");
			LongText longText6 = LongText.CreateFromExistingValue(6, 60, "text");

			ObjectExportInfo artifact = new ObjectExportInfo();

			_longTextPrecedenceBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None)).Returns(new List<LongText> {longText1, longText3, longText5, longText5});
			_longTextFieldBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None)).Returns(new List<LongText> {longText2, longText4});
			_longTextIproFullTextBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None)).Returns(new List<LongText> {longText4, longText6});

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.AreEquivalent(new List<LongText> {longText1, longText2, longText3, longText4, longText5, longText6}, _longTextRepository.GetLongTexts());
		}
	}
}