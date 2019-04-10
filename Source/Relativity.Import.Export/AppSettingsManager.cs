// ----------------------------------------------------------------------------
// <copyright file="AppSettingsManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Defines static methods to manage application settings.
	/// </summary>
	internal static class AppSettingsManager
	{
		/// <summary>
		/// Gets the application setting attributes dictionary.
		/// </summary>
		/// <value>
		/// The dictionary.
		/// </value>
		public static Dictionary<string, AppSettingAttribute> AppSettingAttributes { get; } = new Dictionary<string, AppSettingAttribute>();

		/// <summary>
		/// Gets or sets the Registry sub-key name.
		/// </summary>
		/// <value>
		/// The sub-key name.
		/// </value>
		/// <remarks>
		/// This is provided strictly for testing purposes.
		/// </remarks>
		public static string RegistrySubKeyName { get; set; } = @"Software\kCura\Relativity";

		/// <summary>
		/// Copies all settings from the <paramref name="source"/> settings object to the <paramref name="target"/> dictionary.
		/// </summary>
		/// <param name="source">
		/// The source settings.
		/// </param>
		/// <param name="target">
		/// The target dictionary.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "This is done intentionally.")]
		public static void Copy(IAppSettings source, IDictionary target)
		{
			try
			{
				Dictionary<string, Dictionary<string, object>> allSectionsDictionary = ReadSettingsDictionaries(source);
				foreach (string section in allSectionsDictionary.Keys)
				{
					Dictionary<string, object> sectionDictionary = allSectionsDictionary[section];
					foreach (string settingKey in sectionDictionary.Keys)
					{
						target[settingKey] = sectionDictionary[settingKey];
					}
				}
			}
			catch
			{
				// Due to legacy concerns, never allow configuration errors to throw.
			}
		}

		/// <summary>
		/// Creates a new application settings object and either defaults or refreshes all values.
		/// </summary>
		/// <param name="refresh">
		/// <see langword="true" /> to refresh all values; otherwise, <see langword="false" /> to default all values.
		/// </param>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		public static IAppSettings Create(bool refresh)
		{
			IAppSettings settings = new AppDotNetSettings();
			Default(settings);
			if (refresh)
			{
				Refresh(settings);
			}

			return settings;
		}

		/// <summary>
		/// Defaults all application settings.
		/// </summary>
		/// <param name="settings">
		/// The application settings to default.
		/// </param>
		public static void Default(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			BuildAttributeDictionary();
			foreach (var prop in GetProperties())
			{
				AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
				if (attribute == null || !attribute.IsMapped)
				{
					continue;
				}

				if (prop.PropertyType == typeof(string))
				{
					AssignString(settings, prop, null);
				}
				else if (prop.PropertyType == typeof(bool))
				{
					AssignBool(settings, prop, null);
				}
				else if (prop.PropertyType == typeof(Uri))
				{
					AssignUri(settings, prop, null);
				}
				else if (prop.PropertyType == typeof(int))
				{
					AssignInt32(settings, prop, null);
				}
				else if (prop.PropertyType == typeof(long))
				{
					AssignInt64(settings, prop, null);
				}
				else if (prop.PropertyType.IsEnum)
				{
					AssignEnum(settings, prop, null);
				}
				else
				{
					throw new InvalidOperationException($"The '{prop.Name}' property of type '{prop.PropertyType}' is not supported by the settings reader.");
				}
			}
		}

		/// <summary>
		/// Reads the settings and refreshes all values.
		/// </summary>
		/// <param name="settings">
		/// The application settings to refresh.
		/// </param>
		public static void Refresh(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			// For backwards compatibility, support all legacy sections.
			Dictionary<string, Dictionary<string, object>> sectionDictionaries = ReadAllSectionDictionaries();
			Dictionary<string, object> defaultSection =
				sectionDictionaries[GetSectionKey(AppSettingsConstants.SectionImportExport)];
			BuildAttributeDictionary();
			foreach (var prop in GetProperties())
			{
				string propertyKey = GetPropertyKey(prop);
				AppSettingAttribute attribute = GetAppSettingAttribute(propertyKey);
				if (attribute == null || !attribute.IsMapped)
				{
					continue;
				}

				string sectionKey = GetSectionKey(attribute);
				if (!sectionDictionaries.ContainsKey(sectionKey))
				{
					// This is guaranteed.
					sectionKey = AppSettingsConstants.SectionImportExport;
				}

				var sectionDictionary = sectionDictionaries[sectionKey];
				object value = null;
				string nameValuePairKey = GetSectionNameValuePairKey(attribute);
				if (sectionDictionary.ContainsKey(nameValuePairKey))
				{
					value = sectionDictionary[nameValuePairKey];
				}
				else if (defaultSection.ContainsKey(nameValuePairKey))
				{
					value = defaultSection[nameValuePairKey];
				}

				AssignPropertyValue(settings, prop, value);
			}
		}

		/// <summary>
		/// Dynamically maps <paramref name="keyName"/> to a property contained within <paramref name="settings"/> and update the value.
		/// </summary>
		/// <param name="settings">
		/// The application settings to refresh.
		/// </param>
		/// <param name="keyName">
		/// The key name.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		public static void SetDynamicValue(IAppSettings settings, string keyName, string value)
		{
			BuildAttributeDictionary();
			foreach (var prop in GetProperties())
			{
				string propertyKey = GetPropertyKey(prop);
				AppSettingAttribute attribute = GetAppSettingAttribute(propertyKey);
				if (attribute == null)
				{
					continue;
				}

				if (string.Compare(keyName, attribute.Key, StringComparison.OrdinalIgnoreCase) == 0)
				{
					AssignPropertyValue(settings, prop, value);
					return;
				}
			}
		}

		/// <summary>
		/// Sets the registry key value.
		/// </summary>
		/// <param name="keyName">
		/// The Registry key name.
		/// </param>
		/// <param name="value">
		/// The setting value.
		/// </param>
		public static void SetRegistryKeyValue(string keyName, string value)
		{
			const bool Write = true;
			using (Microsoft.Win32.RegistryKey key = GetRegistryKey(Write))
			{
				key.SetValue(keyName, value);
			}
		}

		/// <summary>
		/// Gets the registry key.
		/// </summary>
		/// <param name="write">
		/// Specify whether write access is required.
		/// </param>
		/// <returns>
		/// The <see cref="Microsoft.Win32.RegistryKey"/> instance.
		/// </returns>
		public static Microsoft.Win32.RegistryKey GetRegistryKey(bool write)
		{
			Microsoft.Win32.RegistryKey key =
				Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistrySubKeyName, write);
			if (key != null)
			{
				return key;
			}

			Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistrySubKeyName);
			key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistrySubKeyName, write);
			return key;
		}

		/// <summary>
		/// Gets the registry key value.
		/// </summary>
		/// <param name="keyName">
		/// The Registry key name.
		/// </param>
		/// <returns>
		/// The key value.
		/// </returns>
		public static string GetRegistryKeyValue(string keyName)
		{
			const bool Write = false;
			using (Microsoft.Win32.RegistryKey key = GetRegistryKey(Write))
			{
				string value = key.GetValue(keyName, string.Empty) as string;
				return value;
			}
		}

		/// <summary>
		/// Builds the attribute dictionary.
		/// </summary>
		public static void BuildAttributeDictionary()
		{
			if (AppSettingAttributes.Count > 0)
			{
				return;
			}

			foreach (var prop in GetProperties())
			{
				AppSettingAttribute attribute = prop.GetCustomAttribute<AppSettingAttribute>();
				if (prop.ReflectedType != null)
				{
					string key = GetPropertyKey(prop);
					AppSettingAttributes[key] = attribute;
				}
			}

			if (AppSettingAttributes.Count == 0)
			{
				throw new ConfigurationErrorsException("The settings object doesn't contain any application settings attributes.");
			}
		}

		public static IEnumerable<PropertyInfo> GetProperties()
		{
			return typeof(AppDotNetSettings).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}

		/// <summary>
		/// Gets the settings from the property.
		/// </summary>
		/// <param name="info">
		/// The property information.
		/// </param>
		/// <returns>
		/// The key name.
		/// </returns>
		public static string GetPropertyKey(PropertyInfo info)
		{
			return info.Name.ToUpperInvariant();
		}

		public static string GetSectionKey(AppSettingAttribute setting)
		{
			return GetSectionKey(setting.Section);
		}

		public static string GetSectionKey(string section)
		{
			return section.ToUpperInvariant();
		}

		public static string GetSectionNameValuePairKey(AppSettingAttribute setting)
		{
			return setting.Key.ToUpperInvariant();
		}

		private static void AssignPropertyValue(
			IAppSettings settings,
			PropertyInfo prop,
			object value)
		{
			if (prop.PropertyType == typeof(string))
			{
				AssignString(settings, prop, value);
			}
			else if (prop.PropertyType == typeof(bool))
			{
				AssignBool(settings, prop, value);
			}
			else if (prop.PropertyType == typeof(int))
			{
				AssignInt32(settings, prop, value);
			}
			else if (prop.PropertyType == typeof(long))
			{
				AssignInt64(settings, prop, value);
			}
			else if (prop.PropertyType == typeof(Uri))
			{
				AssignUri(settings, prop, value);
			}
			else if (prop.PropertyType.IsEnum)
			{
				object enumValue = null;
				if (value != null)
				{
					try
					{
						enumValue = Enum.Parse(prop.PropertyType, value.ToString());
					}
					catch (ArgumentException)
					{
					}
				}

				AssignEnum(settings, prop, enumValue);
			}
			else
			{
				throw new InvalidOperationException($"The '{prop.Name}' property of type '{prop.PropertyType}' is not supported by the settings reader.");
			}
		}

		private static void AssignString(IAppSettings settings, PropertyInfo prop, object value)
		{
			string stringValue = value?.ToString();
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			string defaultValue = null;
			if (attribute?.DefaultValue != null)
			{
				defaultValue = attribute.DefaultValue.ToString();
			}

			if (stringValue == null)
			{
				stringValue = defaultValue;
			}

			AssignValue(settings, prop, stringValue);
		}

		private static void AssignBool(IAppSettings settings, PropertyInfo prop, object value)
		{
			bool? boolValue = null;
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			if (value != null)
			{
				bool temp;
				if (bool.TryParse(value.ToString(), out temp))
				{
					boolValue = temp;
				}
			}

			bool defaultValue = false;
			if (value == null && attribute?.DefaultValue != null)
			{
				if (!bool.TryParse(attribute.DefaultValue.ToString(), out defaultValue))
				{
					defaultValue = false;
				}
			}

			if (!boolValue.HasValue)
			{
				boolValue = defaultValue;
			}

			AssignValue(settings, prop, boolValue);
		}

		private static void AssignEnum(IAppSettings settings, PropertyInfo prop, object value)
		{
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			if (attribute == null)
			{
				return;
			}

			object defaultValue = null;
			if (value == null && attribute != null)
			{
				if (attribute.DefaultValue != null)
				{
					try
					{
						defaultValue = Enum.Parse(prop.PropertyType, attribute.DefaultValue.ToString());
					}
					catch (ArgumentException)
					{
					}
				}
			}

			if (value == null)
			{
				value = defaultValue;
			}

			AssignValue(settings, prop, value);
		}

		private static void AssignInt32(IAppSettings settings, PropertyInfo prop, object value)
		{
			int? intValue = null;
			if (value != null)
			{
				int temp;
				if (int.TryParse(value.ToString(), out temp))
				{
					intValue = temp;
				}
			}

			int defaultValue = 0;
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			if (!intValue.HasValue && attribute != null)
			{
				if (attribute.DefaultValue != null)
				{
					if (!int.TryParse(attribute.DefaultValue.ToString(), out defaultValue))
					{
						defaultValue = 0;
					}
				}
			}

			if (!intValue.HasValue)
			{
				intValue = defaultValue;
			}

			AssignValue(settings, prop, intValue);
		}

		private static void AssignInt64(IAppSettings settings, PropertyInfo prop, object value)
		{
			long? longValue = null;
			if (value != null)
			{
				long temp;
				if (long.TryParse(value.ToString(), out temp))
				{
					longValue = temp;
				}
			}

			long defaultValue = 0L;
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			if (!longValue.HasValue && attribute != null)
			{
				if (attribute.DefaultValue != null)
				{
					if (!long.TryParse(attribute.DefaultValue.ToString(), out defaultValue))
					{
						defaultValue = 0L;
					}
				}
			}

			if (!longValue.HasValue)
			{
				longValue = defaultValue;
			}

			AssignValue(settings, prop, longValue);
		}

		private static void AssignUri(IAppSettings settings, PropertyInfo prop, object value)
		{
			Uri uriValue = null;
			if (value != null)
			{
				Uri.TryCreate(value.ToString(), UriKind.RelativeOrAbsolute, out uriValue);
			}

			Uri defaultValue = null;
			AppSettingAttribute attribute = GetAppSettingAttribute(GetPropertyKey(prop));
			if (attribute?.DefaultValue != null)
			{
				if (!Uri.TryCreate(attribute.DefaultValue.ToString(), UriKind.RelativeOrAbsolute, out defaultValue))
				{
					defaultValue = null;
				}
			}

			if (uriValue == null)
			{
				uriValue = defaultValue;
			}

			AssignValue(settings, prop, uriValue);
		}

		private static void AssignValue(IAppSettings settings, PropertyInfo prop, object value)
		{
			try
			{
				prop.SetValue(settings, value);
			}
			catch (ArgumentException)
			{
			}
			catch (System.Reflection.TargetException)
			{
			}
			catch (System.MethodAccessException)
			{
			}
			catch (System.Reflection.TargetInvocationException)
			{
			}
		}

		/// <summary>
		/// Reads all setting values from the <paramref name="settings"/> object into one or more setting dictionaries.
		/// </summary>
		/// <param name="settings">
		/// The settings object used to create the section dictionaries.
		/// </param>
		/// <returns>
		/// The section dictionaries.
		/// </returns>
		private static Dictionary<string, Dictionary<string, object>> ReadSettingsDictionaries(IAppSettings settings)
		{
			Dictionary<string, Dictionary<string, object>> dictionaries = new Dictionary<string, Dictionary<string, object>>();
			BuildAttributeDictionary();
			foreach (var prop in GetProperties())
			{
				string propertyKey = GetPropertyKey(prop);
				AppSettingAttribute attribute = GetAppSettingAttribute(propertyKey);
				if (attribute == null || string.IsNullOrEmpty(attribute.Section))
				{
					continue;
				}

				Dictionary<string, object> sectionDictionary;
				if (!dictionaries.TryGetValue(attribute.Section, out sectionDictionary))
				{
					sectionDictionary = new Dictionary<string, object>();
					dictionaries.Add(attribute.Section, sectionDictionary);
				}

				sectionDictionary[attribute.Key] = prop.GetValue(settings);
			}

			return dictionaries;
		}

		/// <summary>
		/// Reads all app config section name/value pairs and returns the dictionary.
		/// </summary>
		/// <returns>
		/// The section dictionaries.
		/// </returns>
		private static Dictionary<string, Dictionary<string, object>> ReadAllSectionDictionaries()
		{
			Dictionary<string, Dictionary<string, object>> dictionaries = new Dictionary<string, Dictionary<string, object>>();
			List<string> sectionNames = new List<string>
				                        {
					                        AppSettingsConstants.SectionLegacykCuraConfig,
					                        AppSettingsConstants.SectionLegacyWindowsProcess,
					                        AppSettingsConstants.SectionLegacyUtility,
					                        AppSettingsConstants.SectionLegacyWinEdds,
					                        AppSettingsConstants.SectionImportExport
				                        };
			foreach (var sectionName in sectionNames)
			{
				dictionaries.Add(GetSectionKey(sectionName), ReadSectionDictionary(sectionName));
			}

			return dictionaries;
		}

		/// <summary>
		/// Reads the app config section name/value pairs and returns a new dictionary.
		/// </summary>
		/// <param name="sectionName">
		/// Name of the section.
		/// </param>
		/// <returns>
		/// The dictionary.
		/// </returns>
		private static Dictionary<string, object> ReadSectionDictionary(string sectionName)
		{
			Dictionary<string, object> settings = new Dictionary<string, object>();

			try
			{
				System.Collections.Hashtable section = ConfigurationManager.GetSection(sectionName) as System.Collections.Hashtable;
				if (section != null)
				{
					var sectionDictionary = section.Cast<System.Collections.DictionaryEntry>().ToDictionary(
						n => n.Key.ToString().ToUpperInvariant(),
						n => n.Value);
					foreach (var key in sectionDictionary.Keys)
					{
						settings[key] = sectionDictionary[key];
					}
				}

				return settings;
			}
			catch (ConfigurationErrorsException)
			{
				// Due to legacy concerns, never allow configuration errors to throw.
				return settings;
			}
		}

		/// <summary>
		/// Gets the application setting attribute for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="AppSettingAttribute"/> instance.
		/// </returns>
		private static AppSettingAttribute GetAppSettingAttribute(string key)
		{
			return !AppSettingAttributes.ContainsKey(key) ? null : AppSettingAttributes[key];
		}
	}
}