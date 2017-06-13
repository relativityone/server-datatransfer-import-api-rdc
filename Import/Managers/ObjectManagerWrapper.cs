using System.Data;
using kCura.WinEDDS.Service;

namespace kCura.WinEDDS.Core.Import.Managers
{
	public class ObjectManagerWrapper : IObjectManager
	{
		private readonly ObjectManager _objectManager;

		public ObjectManagerWrapper(ObjectManager objectManager)
		{
			_objectManager = objectManager;
		}

		public DataSet RetrieveArtifactIdOfMappedParentObject(int caseContextArtifactId, string textIdentifier, int artifactTypeId)
		{
			return _objectManager.RetrieveArtifactIdOfMappedParentObject(caseContextArtifactId, textIdentifier, artifactTypeId);
		}
	}
}