// ----------------------------------------------------------------------------
// <copyright file="TestCategories.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Represents all import and export API test categories.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class TestCategories
	{
		/// <summary>
		/// The ExtensionMethods test category.
		/// </summary>
		public const string ExtensionMethods = "ExtensionMethods";

		/// <summary>
		/// The file system test category.
		/// </summary>
		public const string FileSystem = "FileSystem";

		/// <summary>
		/// The folder test category.
		/// </summary>
		public const string Folder = "Folder";

		/// <summary>
		/// The framework test category.
		/// </summary>
		public const string Framework = "Framework";

		/// <summary>
		/// The Outside In test category.
		/// </summary>
		public const string OutsideIn = "OutsideIn";

		/// <summary>
		/// The separate domain test category.
		/// </summary>
		public const string SeparateDomain = "SeparateDomain";

		/// <summary>
		/// The transfer API test category.
		/// </summary>
		public const string TransferApi = "TransferApi";

		/// <summary>
		/// Tell tests not to run this in the compatibility pipeline.
		/// If we add some functionality, it might not be available in older versions.
		/// </summary>
		public const string NotInCompatibility = "NotInCompatibility";

		/// <summary>
		/// The SqlComparer test category.
		/// </summary>
		public const string SqlComparer = "SqlComparer";
	}
}