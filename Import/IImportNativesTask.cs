using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportNativesTask
	{
		void Execute(ArtifactFieldCollection artifactFieldCollection);
	}
}
