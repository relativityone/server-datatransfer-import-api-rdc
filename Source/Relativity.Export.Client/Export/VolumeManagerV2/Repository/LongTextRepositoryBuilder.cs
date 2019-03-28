using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class LongTextRepositoryBuilder : IRepositoryBuilder
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly ILongTextBuilder _longTextPrecedenceBuilder;
		private readonly ILongTextBuilder _longTextFieldBuilder;
		private readonly ILongTextBuilder _longTextIproFullTextBuilder;
		private readonly ILog _logger;

		public LongTextRepositoryBuilder(ILongTextBuilder longTextPrecedenceBuilder, ILongTextBuilder longTextFieldBuilder, ILongTextBuilder longTextIproFullTextBuilder,
			LongTextRepository longTextRepository, ILog logger)
		{
			_longTextPrecedenceBuilder = longTextPrecedenceBuilder;
			_longTextFieldBuilder = longTextFieldBuilder;
			_longTextIproFullTextBuilder = longTextIproFullTextBuilder;
			_longTextRepository = longTextRepository;
			_logger = logger;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Attempting to build LongText repository.");
			var longTextsToAdd = new List<LongText>();

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Creating LongText entries for text precedence.");
			IList<LongText> precedenceLongTexts = _longTextPrecedenceBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", precedenceLongTexts.Count);
			AddDistinct(precedenceLongTexts, longTextsToAdd);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Creating LongText entries for fields.");
			IList<LongText> fieldLongTexts = _longTextFieldBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", fieldLongTexts.Count);
			AddDistinct(fieldLongTexts, longTextsToAdd);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Creating LongText entries for missing Extracted Text.");
			IList<LongText> iproFullTexts = _longTextIproFullTextBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", iproFullTexts.Count);
			AddDistinct(iproFullTexts, longTextsToAdd);

			_longTextRepository.Add(longTextsToAdd);
		}


		private void AddDistinct(IList<LongText> longTexts, List<LongText> targetCollection)
		{
			foreach (var longText in longTexts)
			{
				if (!targetCollection.Any(x => x.ArtifactId == longText.ArtifactId && x.FieldArtifactId == longText.FieldArtifactId))
				{
					targetCollection.Add(longText);
				}
			}
		}
	}
}