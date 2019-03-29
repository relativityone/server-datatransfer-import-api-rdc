// -----------------------------------------------------------------------------------------------------
// <copyright file="XmlConfigurationManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Desktop.Client.CustomActions
{
	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using System.Xml.XPath;

	/// <summary>
	/// Represents a replacement for <see cref="ConfigurationManager"/> that
	/// uses <see cref="XDocument"/> to perform similar operations for
	/// external configuration files that contain custom sections.
	/// </summary>
	/// <remarks>
	/// The .NET Configuration API is one of the worst aspects of the CLR and working
	/// with custom sections has always been excessively difficult. Since the API only
	/// provides limited functionality when working with external files, this class
	/// includes the minimal operations needed by the custom action to preserve application
	/// settings.
	/// </remarks>
	public class XmlConfigurationManager
	{
		public const string SectionNameRelativityImportExport = "Relativity.Import.Export";
		public const string SectionNameAppSettings = "appSettings";
		public const string SectionNameLegacyWinEdds = "kCura.WinEDDS";
		public const string SectionNameLegacyUtility = "kCura.Utility";
		public const string SectionNameLegacyProcess = "kCura.Windows.Process";
		private const string AddElement = "add";
		private const string KeyAttribute = "key";
		private const string ValueAttribute = "value";
		private readonly string file;
		private XDocument document;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationManager"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the configuration file.
		/// </param>
		public XmlConfigurationManager(string file)
		{
			this.file = file;
		}

		/// <summary>
		/// Ensures the Xml document is loaded.
		/// </summary>
		private void LoadDoc()
		{
			if (this.document == null)
			{
				this.document = XDocument.Load(file);
			}
		}

		/// <summary>
		/// Retrieves a list of all sections.
		/// </summary>
		/// <returns>
		/// The list of sections.
		/// </returns>
		public IList<string> GetSections()
		{
			// The code below is an indirect but simplified way to identify sections.
			this.LoadDoc();
			var elements = this.document.XPathSelectElements($"//*/{AddElement}")
				.Where(x => x.Parent != null && x.FirstAttribute != null && x.LastAttribute != null);
			return elements
				.Select(x => x.Parent.Name.LocalName).Distinct().ToList();
		}

		/// <summary>
		/// Gets the total number of settings for this configuration file.
		/// </summary>
		/// <returns>
		/// The settings count.
		/// </returns>
		public int GetSettingsCount()
		{
			return this.GetSections().Sum(section => this.GetSectionSettings(section).Count);
		}

		/// <summary>
		/// Retrieves all settings for the specified section.
		/// </summary>
		/// <param name="section">
		/// The section to retrieve.
		/// </param>
		/// <returns>
		/// The <see cref="KeyValueConfigurationCollection"/> instance.
		/// </returns>
		public KeyValueConfigurationCollection GetSectionSettings(string section)
		{
			this.LoadDoc();
			var elements = this.document.XPathSelectElements($"//{section}/{AddElement}")
				.Where(x => x.FirstAttribute != null && x.LastAttribute != null);
			var collection = new KeyValueConfigurationCollection();
			foreach (var element in elements)
			{
				XAttribute nameAttr = element.Attribute(KeyAttribute);
				XAttribute valueAttr = element.Attribute(ValueAttribute);
				if (nameAttr != null && valueAttr != null)
				{
					collection.Add(nameAttr.Value, valueAttr.Value);
				}
			}

			return collection;
		}

		// KeyValueConfigurationElement

		/// <summary>
		/// Updates the key value pair within the specified section.
		/// </summary>
		/// <param name="section">
		/// The section name to update.
		/// </param>
		/// <param name="pair">
		/// The key value pair to update.
		/// </param>
		public void SetSectionSettings(string section, KeyValueConfigurationElement pair)
		{
			if (pair == null)
			{
				throw new ArgumentNullException(nameof(pair));
			}

			this.SetSectionSettings(section, pair.Key, pair.Value);
		}

		/// <summary>
		/// Updates the key value pair within the specified section.
		/// </summary>
		/// <param name="section">
		/// The section name to update.
		/// </param>
		/// <param name="key">
		/// The key to update.
		/// </param>
		/// <param name="value">
		/// The key value to update.
		/// </param>
		public void SetSectionSettings(string section, string key, string value)
		{
			if (string.IsNullOrEmpty(section))
			{
				throw new ArgumentNullException(nameof(section));
			}

			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			this.LoadDoc();
			XElement node = this.document.XPathSelectElement($"//{section}/{AddElement}[@{KeyAttribute}='{key}']");
			if (node == null)
			{
				XElement parentNode = this.document.XPathSelectElement($"//{section}");
				if (parentNode == null)
				{
					throw new InvalidOperationException($"This operation cannot be performed because the section '{section}' doesn't exist.");
				}

				XElement pairElement = new XElement("add");
				pairElement.Add(new XAttribute("key", key));
				pairElement.Add(new XAttribute("value", value));
				parentNode.Add(pairElement);
				return;
			}

			XAttribute attr = node.Attribute(ValueAttribute);
			if (attr != null)
			{
				attr.Value = value;
			}
			else
			{
				node.SetAttributeValue(ValueAttribute, value);
			}
		}

		/// <summary>
		/// Saves the configuration file.
		/// </summary>
		public void Save()
		{
			this.document?.Save(this.file);
		}
	}
}
