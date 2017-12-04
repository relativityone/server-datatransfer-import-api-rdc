using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class ExportCleanUp : IExportCleanUp
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileHelper _fileHelper;

		private readonly ILog _logger;

		public ExportCleanUp(LongTextRepository longTextRepository, ILog logger, IFileHelper fileHelper)
		{
			_longTextRepository = longTextRepository;
			_logger = logger;
			_fileHelper = fileHelper;
		}

		public void CleanUp()
		{
			_logger.LogVerbose("Cleaning up after export.");
			RemoveLongTextTempFiles();
			_logger.LogVerbose("Clean up finished.");
		}

		private void RemoveLongTextTempFiles()
		{
			foreach (var longText in _longTextRepository.GetLongTexts())
			{
				if (longText.RequireDeletion)
				{
					_logger.LogInformation("Removing long text temp file {file}.", longText.Location);
					_fileHelper.Delete(longText.Location);
				}
			}
		}
	}
}