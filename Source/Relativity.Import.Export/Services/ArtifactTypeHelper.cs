// ----------------------------------------------------------------------------
// <copyright file="ArtifactTypeHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Services
{
	/// <summary>
	/// Defines static helper methods to perform common artifact type operations.
	/// </summary>
	internal static class ArtifactTypeHelper
	{
		/// <summary>
		/// Determines whether the specified artifact type identifier is associated with a dynamic object.
		/// </summary>
		/// <param name="artifactTypeId">
		/// The artifact type identifier.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the specified artifact type identifier is associated with a dynamic object; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsDynamic(int artifactTypeId)
		{
			ArtifactType artifactType = (ArtifactType)artifactTypeId;
			return artifactTypeId > 1000003 || artifactType == ArtifactType.Document
			                                || artifactType == ArtifactType.InstallEventHandler
			                                || artifactType == ArtifactType.MassOperation
			                                || artifactType == ArtifactType.MarkupSet
			                                || artifactType == ArtifactType.Production;
		}
	}
}