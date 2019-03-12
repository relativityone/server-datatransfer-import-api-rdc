﻿// ----------------------------------------------------------------------------
// <copyright file="AppSettingsReader.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Defines static methods to retrieve application settings.
	/// </summary>
	public static class AppSettingsReader
	{
		/// <summary>
		/// Gets the application setting attributes dictionary.
		/// </summary>
		/// <value>
		/// The dictionary.
		/// </value>
		internal static Dictionary<string, AppSettingAttribute> AppSettingAttributes { get; } = new Dictionary<string, AppSettingAttribute>();

		/// <summary>
		/// Gets or sets the Registry sub-key name.
		/// </summary>
		/// <value>
		/// The sub-key name.
		/// </value>
		/// <remarks>
		/// This is provided strictly for testing purposes.
		/// </remarks>
		internal static string RegistrySubKeyName { get; set; } = @"Software\kCura\Relativity";

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
				AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
			Dictionary<string, Dictionary<string, object>> sectionDictionaries = ReadAllSectionValues();
			BuildAttributeDictionary();
			foreach (var prop in GetProperties())
			{
				string propertyKey = GetPropertyKey(prop);
				if (!AppSettingAttributes.ContainsKey(propertyKey))
				{
					continue;
				}

				AppSettingAttribute attribute = AppSettingAttributes[propertyKey];
				if (!attribute.IsMapped)
				{
					continue;
				}

				string sectionKey = GetSectionKey(attribute);
				if (!sectionDictionaries.ContainsKey(sectionKey))
				{
					// This is guaranteed.
					sectionKey = AppSettingsConstants.Section;
				}

				var sectionDictionary = sectionDictionaries[sectionKey];
				object value = null;
				string nameValuePairKey = GetSectionNameValuePairKey(attribute);
				if (sectionDictionary.ContainsKey(nameValuePairKey))
				{
					value = sectionDictionary[nameValuePairKey];
				}

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

			// This complexity is due to 3 possible values including the Windows Registry.
			AppDotNetSettings dotNetSettings = settings as AppDotNetSettings;
			Uri webApiServiceUrl = null;
			if (sectionDictionaries.ContainsKey(AppSettingsConstants.SectionLegacyWinEdds))
			{
				var sectionDictionary = sectionDictionaries[AppSettingsConstants.SectionLegacyWinEdds];
				webApiServiceUrl = sectionDictionary.GetUriValue(AppSettingsConstants.WebApiServiceUrlRegistryKey, null);
			}

			if (dotNetSettings != null)
			{
				dotNetSettings.RefreshWebApiServiceUrl(webApiServiceUrl);
			}
			else
			{
				settings.WebApiServiceUrl = webApiServiceUrl;
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
		internal static void SetRegistryKeyValue(string keyName, string value)
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
		internal static Microsoft.Win32.RegistryKey GetRegistryKey(bool write)
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
		internal static string GetRegistryKeyValue(string keyName)
		{
			const bool Write = false;
			using (Microsoft.Win32.RegistryKey key = GetRegistryKey(Write))
			{
				string value = key.GetValue(keyName, string.Empty) as string;
				return value;
			}
		}

		/// <summary>
		/// Validates to ensure the URI format is proper.
		/// </summary>
		/// <param name="value">
		/// The string representation of the URI.
		/// </param>
		/// <returns>
		/// The validated URI string.
		/// </returns>
		internal static string ValidateUriFormat(string value)
		{
			if (!string.IsNullOrEmpty(value) && !value.Trim().EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Trim() + "/";
			}

			Uri result;
			if (Uri.TryCreate(value, UriKind.Absolute, out result))
			{
				return value;
			}

			return string.Empty;
		}

		/// <summary>
		/// Builds the attribute dictionary.
		/// </summary>
		internal static void BuildAttributeDictionary()
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

		internal static IEnumerable<PropertyInfo> GetProperties()
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
		internal static string GetPropertyKey(PropertyInfo info)
		{
			return info.Name.ToUpperInvariant();
		}

		internal static string GetSectionKey(AppSettingAttribute setting)
		{
			return GetSectionKey(setting.Section);
		}

		internal static string GetSectionKey(string section)
		{
			return section.ToUpperInvariant();
		}

		internal static string GetSectionNameValuePairKey(AppSettingAttribute setting)
		{
			return setting.Key.ToUpperInvariant();
		}

		/// <summary>
		/// Reads all app config section name/value pairs and return the dictionary.
		/// </summary>
		/// <returns>
		/// The dictionary that contains all read section name/value pairs.
		/// </returns>
		private static Dictionary<string, Dictionary<string, object>> ReadAllSectionValues()
		{
			Dictionary<string, Dictionary<string, object>> settings = new Dictionary<string, Dictionary<string, object>>();
			settings.Add(
				GetSectionKey(AppSettingsConstants.SectionLegacyWindowsProcess),
				ReadSectionValues(AppSettingsConstants.SectionLegacyWindowsProcess));
			settings.Add(
				GetSectionKey(AppSettingsConstants.SectionLegacyUtility),
				ReadSectionValues(AppSettingsConstants.SectionLegacyUtility));
			settings.Add(
				GetSectionKey(AppSettingsConstants.SectionLegacyWinEdds),
				ReadSectionValues(AppSettingsConstants.SectionLegacyWinEdds));
			settings.Add(
				GetSectionKey(AppSettingsConstants.Section),
				ReadSectionValues(AppSettingsConstants.Section));
			return settings;
		}

		/// <summary>
		/// Reads the app config section name/value pairs and add to the dictionary.
		/// </summary>
		/// <param name="sectionName">
		/// Name of the section.
		/// </param>
		/// <returns>
		/// The dictionary.
		/// </returns>
		private static Dictionary<string, object> ReadSectionValues(string sectionName)
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

		private static void AssignString(IAppSettings settings, PropertyInfo prop, object value)
		{
			string stringValue = value?.ToString();
			string key = GetPropertyKey(prop);
			AppSettingAttribute attribute = AppSettingAttributes[key];
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
			AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
			string propertyKey = GetPropertyKey(prop);
			if (!AppSettingAttributes.ContainsKey(propertyKey))
			{
				return;
			}

			object defaultValue = null;
			AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
			AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
			AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
			AppSettingAttribute attribute = AppSettingAttributes[GetPropertyKey(prop)];
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
	}
}