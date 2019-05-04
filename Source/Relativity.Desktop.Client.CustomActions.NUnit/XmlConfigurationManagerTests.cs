// -----------------------------------------------------------------------------------------------------
// <copyright file="AppConfigServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;

namespace Relativity.Desktop.Client.CustomActions.NUnit
{
	using global::NUnit.Framework;

	using System.Collections.Generic;
	using System.Configuration;

	[TestFixture]
	public class XmlConfigurationManagerTests
	{
		private string testDirectory;
		private string appConfigFile;

		[SetUp]
		public void Setup()
		{
			this.testDirectory = Path.Combine(Path.GetTempPath(), "RelativityTmpDir_" + DateTime.Now.Ticks + "_" + Guid.NewGuid());
			System.IO.Directory.CreateDirectory(this.testDirectory);
			string fileName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
			string basePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string sourceAppConfigFile = System.IO.Path.Combine(basePath, $"{fileName}.config");
			this.appConfigFile = System.IO.Path.Combine(this.testDirectory, fileName);
			System.IO.File.Copy(sourceAppConfigFile, this.appConfigFile);
		}

		[TearDown]
		public void Teardown()
		{
			if (System.IO.Directory.Exists(this.testDirectory))
			{
				System.IO.Directory.Delete(this.testDirectory, true);
			}
		}

		[Test]
	    public void ShouldGetTheSections()
	    {
			XmlConfigurationManager manager = new XmlConfigurationManager(this.appConfigFile);
			IList<string> sections = manager.GetSections();
			Assert.That(sections.Count, Is.EqualTo(2));
			Assert.That(
				sections,
				Is.EquivalentTo(
					new[]
						{
							XmlConfigurationManager.SectionNameAppSettings,
							XmlConfigurationManager.SectionNameRelativityImportExport
						}));
	    }

	    [Test]
	    public void ShouldGetTheSectionSettings()
	    {
		    XmlConfigurationManager manager = new XmlConfigurationManager(this.appConfigFile);
			KeyValueConfigurationCollection appSettings = manager.GetSectionSettings(XmlConfigurationManager.SectionNameAppSettings);
		    Assert.That(appSettings, Is.Not.Null);
		    Assert.That(appSettings.Count, Is.EqualTo(2));
		    Assert.That(appSettings["Setting1"], Is.Not.Null);
			Assert.That(appSettings["Setting1"].Value, Is.EqualTo("Value1"));
			Assert.That(appSettings["Setting2"], Is.Not.Null);
			Assert.That(appSettings["Setting2"].Value, Is.EqualTo("Value2"));
		    KeyValueConfigurationCollection importExportSettings = manager.GetSectionSettings(XmlConfigurationManager.SectionNameRelativityImportExport);
		    Assert.That(importExportSettings, Is.Not.Null);
		    Assert.That(importExportSettings.Count, Is.EqualTo(2));
		    Assert.That(importExportSettings["Setting3"], Is.Not.Null);
			Assert.That(importExportSettings["Setting3"].Value, Is.EqualTo("Value3"));
		    Assert.That(importExportSettings["Setting4"], Is.Not.Null);
			Assert.That(importExportSettings["Setting4"].Value, Is.EqualTo("Value4"));
		}

	    [Test]
	    public void ShouldModifyTheSectionSettings()
	    {
		    XmlConfigurationManager manager = new XmlConfigurationManager(this.appConfigFile);
		    manager.SetSectionSettings(XmlConfigurationManager.SectionNameAppSettings, "abc", "def");
		    manager.SetSectionSettings(XmlConfigurationManager.SectionNameRelativityImportExport, "ghi", "jkl");
			manager.Save();
		    manager = new XmlConfigurationManager(this.appConfigFile);
		    KeyValueConfigurationCollection appSettings = manager.GetSectionSettings(XmlConfigurationManager.SectionNameAppSettings);
			Assert.That(appSettings, Is.Not.Null);
			Assert.That(appSettings.Count, Is.EqualTo(3));
			Assert.That(appSettings["abc"], Is.Not.Null);
		    Assert.That(appSettings["abc"].Value, Is.EqualTo("def"));
		    KeyValueConfigurationCollection importExportSettings = manager.GetSectionSettings(XmlConfigurationManager.SectionNameRelativityImportExport);
		    Assert.That(importExportSettings, Is.Not.Null);
		    Assert.That(importExportSettings.Count, Is.EqualTo(3));
		    Assert.That(importExportSettings["ghi"], Is.Not.Null);
		    Assert.That(importExportSettings["ghi"].Value, Is.EqualTo("jkl"));
		}
	}
}