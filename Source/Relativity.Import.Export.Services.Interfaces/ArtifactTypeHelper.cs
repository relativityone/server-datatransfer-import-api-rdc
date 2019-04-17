using System;

namespace Relativity.Import.Export.Services
{
	public static class ArtifactTypeHelper
	{
		public static bool IsDynamic(int artifactTypeID)
		{
			ArtifactType artifactType = (ArtifactType)artifactTypeID;
			return artifactTypeID > 1000003 || artifactType == ArtifactType.Document
			                                || artifactType == ArtifactType.InstallEventHandler
			                                || artifactType == ArtifactType.MassOperation
			                                || artifactType == ArtifactType.MarkupSet
			                                || artifactType == ArtifactType.Production;
		}
	}

}
