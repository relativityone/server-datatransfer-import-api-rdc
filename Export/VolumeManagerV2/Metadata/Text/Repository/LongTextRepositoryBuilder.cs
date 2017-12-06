using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepositoryBuilder
	{
		private List<LongText> _longTexts;

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

		public void AddLongTextForBatchToRepository(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			foreach (var artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				AddLongTextForArtifact(artifact, cancellationToken);
			}
		}

		private void AddLongTextForArtifact(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Attempting to build LongText repository.");
			_longTexts = new List<LongText>();

			_logger.LogVerbose("Creating LongText entries for text precedence.");
			IList<LongText> precedenceLongTexts = _longTextPrecedenceBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", precedenceLongTexts.Count);
			Add(precedenceLongTexts);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Creating LongText entries for fields.");
			IList<LongText> fieldLongTexts = _longTextFieldBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", fieldLongTexts.Count);
			Add(fieldLongTexts);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Creating LongText entries for missing Extracted Text.");
			IList<LongText> iproFullTexts = _longTextIproFullTextBuilder.CreateLongText(artifact, cancellationToken);
			_logger.LogVerbose("{count} LongText entries created.", iproFullTexts.Count);
			Add(iproFullTexts);

			//TODO think of something better...
			_longTextRepository.Add(_longTexts);
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