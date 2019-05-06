// ----------------------------------------------------------------------------
// <copyright file="IAppSettingsInternal.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	/// <summary>
	/// Represents an abstract object that provides thread-safe internal import/export application settings that are never exposed to API users.
	/// </summary>
	internal interface IAppSettingsInternal
	{
		/// <summary>
		/// Gets or sets a value indicating whether to enforce the version compatibility check.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enforce the version compatibility check; otherwise, <see langword="false" />.
		/// </value>
		bool EnforceVersionCompatibilityCheck
		{
			get;
			set;
		}
	}
}