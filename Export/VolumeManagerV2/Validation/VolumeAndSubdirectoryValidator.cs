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
			int volumeLabelPaddingWidth = CalculatePaddingWidth(volumeStartNumber, totalFiles);
			int subdirectoryLabelPaddingWidth = CalculatePaddingWidth(subdirectoryStartNumber, totalFiles);

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

		private static int CalculatePaddingWidth(int startNumber, long totalFiles)
		{
			long maxPossibleNumber = totalFiles + startNumber;
			return maxPossibleNumber > 0 
				? (int) Math.Floor(Math.Log10(maxPossibleNumber) + 1) 
				: 1;
		}
	}
}