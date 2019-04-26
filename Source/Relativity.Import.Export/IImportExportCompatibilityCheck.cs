// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImportExportCompatibilityCheck.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   This class represents abstraction of the validation methods between IAPI client and Relativity/WebAPi.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	/// <summary>
	/// This class represents abstraction of the validation methods between IAPI client and Relativity/WebAPi.
	/// </summary>
	public interface IImportExportCompatibilityCheck
	{
		/// <summary>
		/// This method checks compatibility between IAPI client represented by <see cref="ImportExportApiClientVersion"/> and Relativity/WebApi service.
		/// </summary>
		/// <returns>Result of version compatibility check represented by <see cref="VersionCompatibilityCheckResult"/>.</returns>
		VersionCompatibilityCheckResult ValidateCompatibility();
	}
}
