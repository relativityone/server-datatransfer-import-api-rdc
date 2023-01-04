// ----------------------------------------------------------------------------
// <copyright file="ObjectType.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	public class ObjectType
	{
		public ObjectType(int artifactId, int artifactTypeId, string typeName)
		{
			this.ArtifactId = artifactId;
			this.ArtifactTypeId = artifactTypeId;
			this.TypeName = typeName;
		}

		public int ArtifactId { get; }

		public int ArtifactTypeId { get; }

		public string TypeName { get; }
	}
}