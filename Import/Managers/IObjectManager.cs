using System.Data;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public interface IObjectManager
	{
		DataSet RetrieveArtifactIdOfMappedParentObject(int caseContextArtifactId, string textIdentifier, int artifactTypeId);
	}
}