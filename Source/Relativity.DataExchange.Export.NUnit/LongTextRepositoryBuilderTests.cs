// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextRepositoryBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

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
			this._longTextPrecedenceBuilder = new Mock<ILongTextBuilder>();
			this._longTextFieldBuilder = new Mock<ILongTextBuilder>();
			this._longTextIproFullTextBuilder = new Mock<ILongTextBuilder>();

			this._longTextRepository = new LongTextRepository(null, new NullLogger());

			this._instance = new LongTextRepositoryBuilder(
				this._longTextPrecedenceBuilder.Object,
				this._longTextFieldBuilder.Object,
				this._longTextIproFullTextBuilder.Object,
				this._longTextRepository,
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

			this._longTextPrecedenceBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None))
				.Returns(new List<LongText> { longText1, longText3, longText5, longText5 });
			this._longTextFieldBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None))
				.Returns(new List<LongText> { longText2, longText4 });
			this._longTextIproFullTextBuilder.Setup(x => x.CreateLongText(artifact, CancellationToken.None))
				.Returns(new List<LongText> { longText4, longText6 });

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.AreEquivalent(
				new List<LongText>
					{
						longText1,
						longText2,
						longText3,
						longText4,
						longText5,
						longText6
					},
				this._longTextRepository.GetLongTexts());
		}
	}
}