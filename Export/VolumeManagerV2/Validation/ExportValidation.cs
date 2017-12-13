using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters.Validator;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation
{
	public class ExportValidation : IExportValidation
	{
		private readonly ExportPermissionCheck _permissionCheck;
		private readonly FilesOverwriteValidator _filesOverwriteValidator;
		private readonly VolumeAndSubdirectoryValidator _volumeAndSubdirectoryValidator;

		public ExportValidation(ExportPermissionCheck permissionCheck, FilesOverwriteValidator filesOverwriteValidator, VolumeAndSubdirectoryValidator volumeAndSubdirectoryValidator)
		{
			_permissionCheck = permissionCheck;
			_filesOverwriteValidator = filesOverwriteValidator;
			_volumeAndSubdirectoryValidator = volumeAndSubdirectoryValidator;
		}

		public bool ValidateExport(ExportFile exportFile, long totalFiles)
		{
			if (!_volumeAndSubdirectoryValidator.Validate(exportFile, totalFiles))
			{
				return false;
			}
			_permissionCheck.CheckPermissions(exportFile.CaseArtifactID);
			if (!_filesOverwriteValidator.ValidateLoadFilesOverwriting(exportFile.Overwrite, exportFile.ExportImages, new LoadFileDestinationPath(exportFile),
				new ImageLoadFileDestinationPath(exportFile)))
			{
				return false;
			}
			return true;
		}
	}
}