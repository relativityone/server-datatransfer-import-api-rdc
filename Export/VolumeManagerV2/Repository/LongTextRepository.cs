using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class LongTextRepository : IClearable, ILongTextRepository
	{
		private List<LongText> _longTexts;
		private Dictionary<int, List<LongText>> _longTextsByArtifactIdDictionary;
		private List<LongTextExportRequest> _exportRequests;

		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;	

		private readonly object _syncLock = new object();

		public LongTextRepository(IFileHelper fileHelper, ILog logger)
		{
			_fileHelper = fileHelper;
			_logger = logger;

			InitializeCollections();
		}

		public void Add(IList<LongText> longTexts)
		{
			lock (_syncLock)
			{
				_longTexts.AddRange(longTexts);

				IndexLongTexts(longTexts);
			}
		}

		public string GetTextFileLocation(int artifactId, int fieldArtifactId)
		{
			return GetLongText(artifactId, fieldArtifactId).Location;
		}

		public LongText GetLongText(int artifactId, int fieldArtifactId)
		{
			lock (_syncLock)
			{
				IEnumerable<LongText> longTextsForArtifact = GetArtifactLongTexts(artifactId);
				return longTextsForArtifact.First(x => x.FieldArtifactId == fieldArtifactId);
			}
		}

		public IList<LongText> GetLongTexts()
		{
			lock (_syncLock)
			{
				return _longTexts;
			}
		}

		public IEnumerable<LongTextExportRequest> GetExportRequests()
		{
			lock (_syncLock)
			{
				return _exportRequests;
			}
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			lock (_syncLock)
			{
				return GetExportRequests().Any(x => x.DestinationLocation == destinationLocation);
			}
		}

		public LongText GetByLineNumber(int lineNumber)
		{
			lock (_syncLock)
			{
				return _longTexts.FirstOrDefault(x => x.ExportRequest != null && x.ExportRequest.Order == lineNumber);
			}
		}

		public IEnumerable<LongText> GetArtifactLongTexts(int artifactId)
		{
			lock (_syncLock)
			{
				List<LongText> longTextsForArtifact;
				return _longTextsByArtifactIdDictionary.TryGetValue(artifactId, out longTextsForArtifact)
					? longTextsForArtifact
					: Enumerable.Empty<LongText>();
			}
		}

		public void Clear()
		{
			lock (_syncLock)
			{
				foreach (var longText in _longTexts)
				{
					if (longText.RequireDeletion)
					{
						_logger.LogInformation("Removing long text temp file {file}.", longText.Location);
						try
						{
							_fileHelper.Delete(longText.Location);
						}
						catch (Exception)
						{
							_logger.LogError("Failed to delete temp file {file} with LongText.", longText.Location);
						}
					}
				}

				InitializeCollections();
			}
		}

		private void IndexLongTexts(IList<LongText> longTexts)
		{
			foreach (LongText longText in longTexts)
			{
				IndexLongText(longText);
			}
		}

		private void IndexLongText(LongText longText)
		{
			List<LongText> dictionaryItemForLongText = GetOrCreateLongTextListInDictionary(longText);

			dictionaryItemForLongText.Add(longText);

			LongTextExportRequest longTextExportRequest = longText.ExportRequest;
			if (longTextExportRequest != null)
			{
				_exportRequests.Add(longTextExportRequest);
			}
		}

		private List<LongText> GetOrCreateLongTextListInDictionary(LongText longText)
		{
			List<LongText> dictionaryItemForLongText;
			int longTextArtifactId = longText.ArtifactId;

			if (!_longTextsByArtifactIdDictionary.TryGetValue(longTextArtifactId, out dictionaryItemForLongText))
			{
				dictionaryItemForLongText = new List<LongText>();
				_longTextsByArtifactIdDictionary[longTextArtifactId] = dictionaryItemForLongText;
			}

			return dictionaryItemForLongText;
		}

		private void InitializeCollections()
		{
			_longTexts = new List<LongText>();
			_longTextsByArtifactIdDictionary = new Dictionary<int, List<LongText>>();
			_exportRequests = new List<LongTextExportRequest>();
		}
	}
}