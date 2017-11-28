using kCura.WinEDDS.Service;
using kCura.WinEDDS.Service.Export;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class ExportPermissionCheck
	{
		private readonly IExportManager _exportManager;
		private readonly ILog _logger;

		public ExportPermissionCheck(IExportManager exportManager, ILog logger)
		{
			_exportManager = exportManager;
			_logger = logger;
		}

		public void CheckPermissions(int workspaceArtifactId)
		{
			if (_exportManager.HasExportPermissions(workspaceArtifactId))
			{
				_logger.LogVerbose("User has export permissions.");
				return;
			}
			_logger.LogWarning("User doesn't have export permissions.");
			throw new ExportManager.InsufficientPermissionsForExportException("Export permissions revoked!  Please contact your system administrator to re-instate export permissions.");
		}
	}
}