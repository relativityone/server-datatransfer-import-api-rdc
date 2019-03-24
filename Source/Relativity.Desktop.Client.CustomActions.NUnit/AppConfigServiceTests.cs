// -----------------------------------------------------------------------------------------------------
// <copyright file="AppConfigServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Desktop.Client.CustomActions.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Microsoft.Deployment.WindowsInstaller;

	using Moq;

	using Relativity.Import.Export.TestFramework;

	[TestFixture]
	public class AppConfigServiceTests
	{
		private Mock<IWixSession> session;
		private TestDirectory testDirectory;
		private string sourceAppConfigFile;
		private string targetAppConfigFile;

		[SetUp]
		public void Setup()
		{
			this.session = new Mock<IWixSession>();
			this.testDirectory = new TestDirectory();
			this.testDirectory.Create();
			string fileName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
			string projectAppConfigFile = ResourceFileHelper.GetBaseFilePath($"{fileName}.config");
			string sourceAppConfigFileName = "app-config-source.config";
			this.sourceAppConfigFile = System.IO.Path.Combine(this.testDirectory.Directory, sourceAppConfigFileName);
			System.IO.File.Copy(projectAppConfigFile, this.sourceAppConfigFile);
			string targetAppConfigFileName = "app-config-target.config";
			this.targetAppConfigFile = System.IO.Path.Combine(this.testDirectory.Directory, targetAppConfigFileName);
			System.IO.File.Copy(projectAppConfigFile, this.targetAppConfigFile);
		}

		[TearDown]
		public void Teardown()
		{
			this.testDirectory.Dispose();
		}

		[Test]
	    public void ShouldNotFailWhenTheBackupSourceConfigFileIsNotDefined()
	    {
		    this.session.Setup(x => x.GetStringPropertyValue(AppConfigService.PropertyNameSourceAppConfigFile))
			    .Returns(string.Empty);
		    AppConfigService service = new AppConfigService(this.session.Object, this.testDirectory.Directory);
			ActionResult result = service.Backup();
			Assert.That(result, Is.EqualTo(ActionResult.Success));
			Assert.That(service.BackupConfigFile, Is.Null.Or.Empty);
		}

	    [Test]
	    public void ShouldNotFailWhenTheBackupSourceConfigFileDoesNotExist()
	    {
		    string nonExistentFile = System.IO.Path.Combine(this.testDirectory.Directory, $"{Guid.NewGuid()}.config");
		    this.session.Setup(x => x.GetStringPropertyValue(AppConfigService.PropertyNameSourceAppConfigFile))
			    .Returns(nonExistentFile);
		    AppConfigService service = new AppConfigService(this.session.Object, this.testDirectory.Directory);
			ActionResult result = service.Backup();
			Assert.That(result, Is.EqualTo(ActionResult.Success));
			Assert.That(service.BackupConfigFile, Is.Null.Or.Empty);
		}

		[Test]
	    public void ShouldBackupTheSourceSourceConfigFile()
	    {
		    this.session.Setup(x => x.GetStringPropertyValue(AppConfigService.PropertyNameSourceAppConfigFile))
			    .Returns(this.sourceAppConfigFile);
		    AppConfigService service = new AppConfigService(this.session.Object, this.testDirectory.Directory);
		    ActionResult result = service.Backup();
		    Assert.That(result, Is.EqualTo(ActionResult.Success));
			Assert.That(service.BackupConfigFile, Is.Not.Null.Or.Empty);
		    Assert.That(service.BackupConfigFile, Does.Exist);
	    }

	    [Test]
		public void ShouldMergeTheSourceAndTargetConfigFiles()
	    {
		    this.session.Setup(x => x.GetStringPropertyValue(AppConfigService.PropertyNameSourceAppConfigFile))
			    .Returns(this.sourceAppConfigFile);
		    this.session.Setup(x => x.GetStringPropertyValue(AppConfigService.PropertyNameTargetAppConfigFile))
			    .Returns(this.targetAppConfigFile);
			XmlConfigurationManager manager = new XmlConfigurationManager(this.sourceAppConfigFile);
			int originalTotalSettings = manager.GetSettingsCount();
			Assert.That(originalTotalSettings, Is.Not.Zero);
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameAppSettings, "SqlCommandTimeout1", "15");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameAppSettings, "SqlTimeout1", "30");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyWinEdds, "TapiClient1", "Http");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyProcess, "UseLog1", "True");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyUtility, "IORetryCount1", "5");
			manager.Save();
			manager = new XmlConfigurationManager(this.targetAppConfigFile);
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameAppSettings, "SqlCommandTimeout2", "15");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameAppSettings, "SqlTimeout2", "30");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyWinEdds, "TapiClient2", "Http");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyProcess, "UseLog2", "True");
			manager.SetSectionSettings(XmlConfigurationManager.SectionNameLegacyUtility, "IORetryCount2", "5");
			manager.Save();
			AppConfigService service = new AppConfigService(this.session.Object, this.testDirectory.Directory);
			ActionResult result = service.Merge();
			Assert.That(result, Is.EqualTo(ActionResult.Success));

			manager = new XmlConfigurationManager(this.targetAppConfigFile);
			int mergedTotalSettings = manager.GetSettingsCount();
			Assert.That(mergedTotalSettings, Is.EqualTo(originalTotalSettings + 10));
	    }
	}
}