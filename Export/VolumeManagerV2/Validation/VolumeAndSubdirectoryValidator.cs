using System;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation
{
	public class VolumeAndSubdirectoryValidator
	{
		private readonly PaddingWarningValidator _validator;
		private readonly IUserNotification _interactionManager;
		private readonly ILog _logger;

		public VolumeAndSubdirectoryValidator(PaddingWarningValidator validator, IUserNotification interactionManager, ILog logger)
		{
			_validator = validator;
			_interactionManager = interactionManager;
			_logger = logger;
		}

		public bool Validate(ExportFile exportSettings, long totalFiles)
		{
			return Validate(exportSettings, exportSettings.VolumeInfo.VolumeStartNumber, exportSettings.VolumeInfo.SubdirectoryStartNumber, totalFiles);
		}

		private bool Validate(ExportFile exportSettings, int volumeStartNumber, int subdirectoryStartNumber, long totalFiles)
		{
			int volumeNumberPaddingWidth = (int) Math.Floor(Math.Log10(volumeStartNumber + 1) + 1);
			int subdirectoryNumberPaddingWidth = (int) Math.Floor(Math.Log10(subdirectoryStartNumber + 1) + 1);
			int totalFilesNumberPaddingWidth = (int) Math.Floor(Math.Log10(totalFiles + volumeStartNumber + 1) + 1);

			int volumeLabelPaddingWidth = Math.Max(totalFilesNumberPaddingWidth, volumeNumberPaddingWidth);
			totalFilesNumberPaddingWidth = (int) Math.Floor(Math.Log10(totalFiles + subdirectoryStartNumber) + 1);
			int subdirectoryLabelPaddingWidth = Math.Max(totalFilesNumberPaddingWidth, subdirectoryNumberPaddingWidth);

			if (!_validator.IsValid(exportSettings, volumeLabelPaddingWidth, subdirectoryLabelPaddingWidth))
			{
				if (!_interactionManager.AlertWarningSkippable(_validator.ErrorMessages))
				{
					_logger.LogWarning("Selected padding is invalid. Canceling export.");
					return false;
				}
			}

			return true;
		}
	}
}