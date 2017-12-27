using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepository : IRepository, ILongTextRepository
	{
		private List<LongText> _longTexts;

		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public LongTextRepository(IFileHelper fileHelper, ILog logger)
		{
			_fileHelper = fileHelper;
			_logger = logger;
			_longTexts = new List<LongText>();
		}

		public void Add(IList<LongText> longTexts)
		{
			_longTexts.AddRange(longTexts);
		}

		public string GetTextFileLocation(int artifactId, int fieldArtifactId)
		{
			return GetLongText(artifactId, fieldArtifactId).Location;
		}

		public LongText GetLongText(int artifactId, int fieldArtifactId)
		{
			return _longTexts.First(x => x.FieldArtifactId == fieldArtifactId && x.ArtifactId == artifactId);
		}

		public IList<LongText> GetLongTexts()
		{
			return _longTexts;
		}

		public IList<LongTextExportRequest> GetExportRequests()
		{
			return _longTexts.Select(x => x.ExportRequest).Where(x => x != null).ToList();
		}

		public LongText GetByUniqueId(string id)
		{
			return _longTexts.FirstOrDefault(x => x.ExportRequest != null && x.ExportRequest.UniqueId == id);
		}

		public IList<LongText> GetArtifactLongTexts(int artifactId)
		{
			return _longTexts.Where(x => x.ArtifactId == artifactId).ToList();
		}

		public void Clear()
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

			_longTexts = new List<LongText>();
		}
	}
}