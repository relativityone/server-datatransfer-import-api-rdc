using System;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportJobInitilizer
	{
		event EventHandler<ImportContext> Initialized;
	}
}
