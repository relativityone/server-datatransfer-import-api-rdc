using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchCleanUp : IBatchCleanUp
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileHelper _fileHelper;

		private readonly ILog _logger;

		public BatchCleanUp(LongTextRepository longTextRepository, IFileHelper fileHelper, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public void CleanUp()
		{
			_logger.LogVerbose("Cleaning up after exporting batch.");
			try
			{
				RemoveLongTextTempFiles();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during batch cleanup. Ignoring exception and continuing with export.");
			}
			_logger.LogVerbose("Clean up finished.");
		}

		private void RemoveLongTextTempFiles()
		{
			foreach (var longText in _longTextRepository.GetLongTexts())
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
			_longTextRepository.Clear();
		}
	}
}