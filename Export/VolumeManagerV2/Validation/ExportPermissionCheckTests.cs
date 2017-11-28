using kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation;
using kCura.WinEDDS.Service;
using kCura.WinEDDS.Service.Export;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Validation
{
	[TestFixture]
	public class ExportPermissionCheckTests
	{
		private ExportPermissionCheck _instance;

		private Mock<IExportManager> _exportManagerMock;

		[SetUp]
		public void SetUp()
		{
			_exportManagerMock = new Mock<IExportManager>();

			_instance = new ExportPermissionCheck(_exportManagerMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		public void ItShouldFailOnMissingPermissions()
		{
			const int workspaceId = 368144;

			_exportManagerMock.Setup(x => x.HasExportPermissions(workspaceId)).Returns(false);

			Assert.Throws<ExportManager.InsufficientPermissionsForExportException>(() => _instance.CheckPermissions(workspaceId));
		}

		[Test]
		public void ItShouldPassWithSufficientPermissions()
		{
			const int workspaceId = 143521;

			_exportManagerMock.Setup(x => x.HasExportPermissions(workspaceId)).Returns(true);

			Assert.DoesNotThrow(() => _instance.CheckPermissions(workspaceId));
		}
	}
}