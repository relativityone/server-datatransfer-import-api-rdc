// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportPermissionCheckTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Export;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Validation;
	using Relativity.Logging;

	[TestFixture]
	public class ExportPermissionCheckTests
	{
		private ExportPermissionCheck _instance;

		private Mock<IExportManager> _exportManagerMock;

		[SetUp]
		public void SetUp()
		{
			this._exportManagerMock = new Mock<IExportManager>();

			this._instance = new ExportPermissionCheck(this._exportManagerMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		public void ItShouldFailOnMissingPermissions()
		{
			const int workspaceId = 368144;

			this._exportManagerMock.Setup(x => x.HasExportPermissions(workspaceId)).Returns(false);

			Assert.Throws<ExportManager.InsufficientPermissionsForExportException>(() => this._instance.CheckPermissions(workspaceId));
		}

		[Test]
		public void ItShouldPassWithSufficientPermissions()
		{
			const int workspaceId = 143521;

			this._exportManagerMock.Setup(x => x.HasExportPermissions(workspaceId)).Returns(true);

			Assert.DoesNotThrow(() => this._instance.CheckPermissions(workspaceId));
		}
	}
}