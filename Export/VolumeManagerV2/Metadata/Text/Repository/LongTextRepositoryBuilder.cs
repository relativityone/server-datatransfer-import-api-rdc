using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepositoryBuilder
	{
		private List<LongText> _longTexts;

		private readonly LongTextHelper _longTextHelper;

		private readonly ILongTextBuilder _longTextPrecedenceBuilder;
		private readonly ILongTextBuilder _longTextFieldBuilder;
		private readonly ILongTextBuilder _longTextIproFullTextBuilder;

		public LongTextRepositoryBuilder(LongTextHelper longTextHelper, ILongTextBuilder longTextPrecedenceBuilder, ILongTextBuilder longTextFieldBuilder,
			ILongTextBuilder longTextIproFullTextBuilder)
		{
			_longTextHelper = longTextHelper;
			_longTextPrecedenceBuilder = longTextPrecedenceBuilder;
			_longTextFieldBuilder = longTextFieldBuilder;
			_longTextIproFullTextBuilder = longTextIproFullTextBuilder;
		}

		public List<LongText> AddLongTextForArtifact(ObjectExportInfo artifact)
		{
			_longTexts = new List<LongText>();

			if (_longTextHelper.IsTextPrecedenceSet())
			{
				IList<LongText> precedenceLongTexts = _longTextPrecedenceBuilder.CreateLongText(artifact);
				Add(precedenceLongTexts);
			}

			IList<LongText> fieldLongTexts = _longTextFieldBuilder.CreateLongText(artifact);
			Add(fieldLongTexts);

			IList<LongText> iproFullTexts = _longTextIproFullTextBuilder.CreateLongText(artifact);
			Add(iproFullTexts);

			return _longTexts;
		}


		private void Add(IList<LongText> longTexts)
		{
			foreach (var longText in longTexts)
			{
				if (!_longTexts.Any(x => x.ArtifactId == longText.ArtifactId && x.FieldArtifactId == longText.FieldArtifactId))
				{
					_longTexts.Add(longText);
				}
			}
		}
	}
}