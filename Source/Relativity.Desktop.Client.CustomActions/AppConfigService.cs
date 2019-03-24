// -----------------------------------------------------------------------------------------------------
// <copyright file="XmlConfigurationManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Desktop.Client.CustomActions
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;

	using Microsoft.Deployment.WindowsInstaller;

	/// <summary>
	/// Represents a class object to perform custom action backup and restore operations.
	/// </summary>
	public class AppConfigService
	{
		public const string PropertyNameSourceAppConfigFile = "SOURCEAPPCONFIGFILE";
		public const string PropertyNameTargetAppConfigFile = "TARGETAPPCONFIGFILE";
		private const string LegacyConfigFileName = "kCura.EDDS.WinForm.exe.config";
		private readonly IWixSession session;
		private readonly string directory;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfigService"/> class.
		/// </summary>
		/// <param name="session">
		/// The session.
		/// </param>
		public AppConfigService(IWixSession session)
			: this(session, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfigService"/> class.
		/// </summary>
		/// <param name="session">
		/// The session.
		/// </param>
		/// <param name="directory">
		/// The optional backup directory. If not specified, <see cref="System.IO.Path.GetTempPath"/> is used.
		/// </param>
		public AppConfigService(IWixSession session, string directory)
		{
			if (session == null)
			{
				throw new ArgumentNullException(nameof(session));
			}

			this.session = session;
			if (string.IsNullOrEmpty(directory))
			{
				directory = System.IO.Path.GetTempPath();
			}

			this.directory = directory;
		}

		/// <summary>
		/// Gets the full path to the backup configuration file. This is only set after <see cref="Backup"/> is called and the source configuration file has been copied.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		/// <remarks>
		/// This property is only used for testing purposes.
		/// </remarks>
		public string BackupConfigFile
		{
			get;
			private set;
		}

		/// <summary>
		/// Backup the configuration file whose path is defined by the <see cref="PropertyNameSourceAppConfigFile"/> property.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/> value.
		/// </returns>
		public ActionResult Backup()
		{
			this.BackupConfigFile = null;
			string sourceConfigFile = string.Empty;
			try
			{
				this.session.Log($"Retrieving the '{PropertyNameSourceAppConfigFile}' source config file...");
				sourceConfigFile = this.session.GetStringPropertyValue(PropertyNameSourceAppConfigFile);
				this.session.Log($"Retrieved the '{PropertyNameSourceAppConfigFile}' source config file. Value='{sourceConfigFile}'");
				if (string.IsNullOrEmpty(sourceConfigFile))
				{
					// This is expected when NOT upgrading.
					this.session.Log("Configuration file is not defined.");
					return ActionResult.Success;
				}

				if (!File.Exists(sourceConfigFile))
				{
					this.session.Log($"The configuration file '{sourceConfigFile}' doesn't exist. Checking to see if a migration is required...");

					// Is this a legacy upgrade?
					string sourceConfigDirectory = System.IO.Path.GetDirectoryName(sourceConfigFile);
					string migratedConfigFile = !string.IsNullOrEmpty(sourceConfigDirectory)
						? System.IO.Path.Combine(sourceConfigDirectory, LegacyConfigFileName)
						: LegacyConfigFileName;
					if (!File.Exists(migratedConfigFile))
					{
						this.session.Log($"The legacy configuration file '{migratedConfigFile}' doesn't exist. No migration is required.");
						return ActionResult.Success;
					}

					this.session.Log($"The legacy configuration file '{migratedConfigFile}' exists and will be backed up and migrated.");
					sourceConfigFile = migratedConfigFile;
				}
				else
				{
					this.session.Log($"The configuration file '{sourceConfigFile}' exists and will be backed up.");
				}

				string tempConfigFile = Path.Combine(this.directory, Path.GetFileName(sourceConfigFile) + Guid.NewGuid());
				File.Copy(sourceConfigFile, tempConfigFile, true);
				this.BackupConfigFile = tempConfigFile;

				// This property must be set to be accessed by the deferred custom action.
				this.session.SetPropertyValue(PropertyNameSourceAppConfigFile, tempConfigFile);
				this.session.Log($"The configuration file '{sourceConfigFile}' backed up and stored in the '{PropertyNameSourceAppConfigFile}' property.");
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				this.session.Log($"Unable to backup configuration file '{sourceConfigFile}' due to exception: {e}");
				return ActionResult.Success;
			}
		}

		/// <summary>
		/// Deletes the temporary backup whose path is defined by the <see cref="PropertyNameTargetAppConfigFile"/> property.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/> value.
		/// </returns>
		public ActionResult Delete()
		{
			string sourceConfig = string.Empty;

			try
			{
				sourceConfig = this.session.GetStringPropertyValue(PropertyNameSourceAppConfigFile);
				if (File.Exists(sourceConfig))
				{
					File.Delete(sourceConfig);
				}

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				this.session.Log($"Unable remove temporary configuration file '{sourceConfig}' due to exception: {e}");
				return ActionResult.Success;
			}
			finally
			{
				this.BackupConfigFile = null;
			}
		}

		/// <summary>
		/// Merges the setting values contained within <see cref="PropertyNameSourceAppConfigFile"/> to the sections contained within <see cref="PropertyNameTargetAppConfigFile"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="ActionResult"/> value.
		/// </returns>
		public ActionResult Merge()
		{
			string sourceConfig = string.Empty;
			string targetConfig = string.Empty;

			try
			{
				sourceConfig = this.session.GetStringPropertyValue(PropertyNameSourceAppConfigFile);
				targetConfig = this.session.GetStringPropertyValue(PropertyNameTargetAppConfigFile);
				if (File.Exists(sourceConfig))
				{
					XmlConfigurationManager sourceSectionManager = new XmlConfigurationManager(sourceConfig);
					XmlConfigurationManager targetSectionManager = new XmlConfigurationManager(targetConfig);

					// Note: the last 3 entries are for possible migration purposes
					// only and must always be handled last.
					Dictionary<string, string> sectionMap = new Dictionary<string, string>
					{
						{ XmlConfigurationManager.SectionNameAppSettings, XmlConfigurationManager.SectionNameAppSettings },
						{ XmlConfigurationManager.SectionNameRelativityImportExport, XmlConfigurationManager.SectionNameRelativityImportExport },
						{ XmlConfigurationManager.SectionNameLegacyWinEdds, XmlConfigurationManager.SectionNameRelativityImportExport },
						{ XmlConfigurationManager.SectionNameLegacyUtility, XmlConfigurationManager.SectionNameRelativityImportExport },
						{ XmlConfigurationManager.SectionNameLegacyProcess, XmlConfigurationManager.SectionNameRelativityImportExport }
					};

					int modifiedSettings = 0;
					foreach (string key in sectionMap.Keys)
					{
						var sourceSectionName = key;
						var targetSectionName = sectionMap[key];
						KeyValueConfigurationCollection sourceSettings = sourceSectionManager.GetSectionSettings(sourceSectionName);
						if (sourceSettings.Count == 0)
						{
							this.session.Log($"The '{sourceConfig}' configuration file doesn't contain any settings within the '{sourceSectionName}' section.");
							continue;
						}

						KeyValueConfigurationCollection targetSettings = targetSectionManager.GetSectionSettings(targetSectionName);
						this.session.Log($"Attempting to merge {sourceSettings.Count} settings from the '{sourceSectionName}' section to the '{targetSectionName}' section.");
						KeyValueConfigurationCollection mergedSettings = Merge(sourceSettings, targetSettings);
						this.session.Log($"Successfully merged {sourceSettings.Count} settings from the '{sourceSectionName}' section to the '{targetSectionName}' section.");
						this.session.Log($"Attempting to restore {mergedSettings.Count} total settings to the '{targetSectionName}' section.");
						foreach (KeyValueConfigurationElement pair in mergedSettings)
						{
							targetSectionManager.SetSectionSettings(targetSectionName, pair);
							modifiedSettings++;
							this.session.Log($"Successfully restored configuration setting. Key: '{pair.Key}', Value: '{pair.Value}'");
						}

						this.session.Log($"Successfully restored {mergedSettings.Count} total settings for the '{targetSectionName}' section.");
					}

					if (modifiedSettings > 0)
					{
						targetSectionManager.Save();
						this.session.Log($"The target configuration file '{targetConfig}' has been successfully restored with {modifiedSettings} total merged settings.");
					}

					return ActionResult.Success;
				}

				this.session.Log($"The source configuration file {sourceConfig} does not exist. No configuration settings will be restored.");
				return ActionResult.Failure;
			}
			catch (Exception e)
			{
				this.session.Log($"Unable to restore configuration from file '{sourceConfig}' due to exception: {e}");
				return ActionResult.Failure;
			}
		}

		/// <summary>
		/// Merges key/value pairs from both collections. If key exists in both collections <paramref name="source"/> has precedence over <paramref name="target"/>
		/// </summary>
		/// <param name="source">
		/// The source configuration.
		/// </param>
		/// <param name="target">
		/// The target configuration.
		/// </param>
		/// <returns>
		/// The merged <see cref="KeyValueConfigurationCollection"/> instance.
		/// </returns>
		private static KeyValueConfigurationCollection Merge(KeyValueConfigurationCollection source, KeyValueConfigurationCollection target)
		{
			var result = new KeyValueConfigurationCollection();

			// non colliding keys from target
			foreach (KeyValueConfigurationElement configurationElement in target)
			{
				if (source.AllKeys.All(x => x != configurationElement.Key))
				{
					result.Add(new KeyValueConfigurationElement(configurationElement.Key, configurationElement.Value));
				}
			}

			// non colliding keys from source
			foreach (KeyValueConfigurationElement configurationElement in source)
			{
				if (target.AllKeys.All(x => x != configurationElement.Key))
				{
					result.Add(new KeyValueConfigurationElement(configurationElement.Key, configurationElement.Value));
				}
			}

			// colliding keys
			foreach (KeyValueConfigurationElement configurationElement in target)
			{
				if (source.AllKeys.Contains(configurationElement.Key))
				{
					result.Add(new KeyValueConfigurationElement(configurationElement.Key, source[configurationElement.Key].Value));
				}
			}

			return result;
		}
	}
}