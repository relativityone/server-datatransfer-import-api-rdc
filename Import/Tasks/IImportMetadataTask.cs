using System;


namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportMetadataTask
	{
		void Execute(MetaDocument metadataDoc);
	}
}
