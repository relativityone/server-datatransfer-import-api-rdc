

using System;
using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	/// <summary>
	/// This class stores field artifact Id that will be used to create native file name during export process
	/// </summary>
	/// <typeparam name="T">field artifact id</typeparam>
	public class SeparatorFileNamePartProvider : FileNamePartProvider<SeparatorDescriptorPart>
	{
		public override string GetFileName(SeparatorDescriptorPart descriptorDescriptorPart, int artifactId, int artifactType)
		{
			throw new NotImplementedException();
		}
	}
}
