// ----------------------------------------------------------------------------
// <copyright file="AppSettingAttribute.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Represents an application setting property-based attribute. This class cannot be inherited.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	internal sealed class AppSettingAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingAttribute"/> class.
		/// </summary>
		public AppSettingAttribute()
			: this(false, string.Empty, string.Empty, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingAttribute"/> class.
		/// </summary>
		/// <param name="section">
		/// The configuration section.
		/// </param>
		/// <param name="key">
		/// The name of the configuration key.
		/// </param>
		public AppSettingAttribute(string section, string key)
			: this(section, key, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingAttribute"/> class.
		/// </summary>
		/// <param name="section">
		/// The configuration section.
		/// </param>
		/// <param name="key">
		/// The name of the configuration key.
		/// </param>
		/// <param name="defaultValue">
		/// The default value.
		/// </param>
		public AppSettingAttribute(string section, string key, object defaultValue)
			: this(true, section, key, defaultValue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingAttribute"/> class.
		/// </summary>
		/// <param name="mapped">
		/// <see langword="true" /> if the parameter is mapped; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="section">
		/// The configuration section.
		/// </param>
		/// <param name="key">
		/// The name of the configuration key.
		/// </param>
		/// <param name="defaultValue">
		/// The default value.
		/// </param>
		public AppSettingAttribute(bool mapped, string section, string key, object defaultValue)
		{
			this.IsMapped = mapped;
			this.Section = section;
			this.Key = key;
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the default value.
		/// </summary>
		/// <value>
		/// The default value.
		/// </value>
		public object DefaultValue
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the associated setting is mapped to an App.Config setting.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the setting is mapped; otherwise, <see langword="false" />.
		/// </value>
		public bool IsMapped
		{
			get;
		}

		/// <summary>
		/// Gets the name of the key for this individual setting. This corresponds with the key found in the <c>app.config</c> file.
		/// </summary>
		/// <value>
		/// The key name.
		/// </value>
		public string Key
		{
			get;
		}

		/// <summary>
		/// Gets the application settings configuration section.
		/// </summary>
		/// <value>
		/// The section name.
		/// </value>
		public string Section
		{
			get;
		}
	}
}