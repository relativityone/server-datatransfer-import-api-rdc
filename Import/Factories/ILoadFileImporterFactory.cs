using System;
using kCura.Windows.Process;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public interface ILoadFileImporterFactory
	{
		LoadFileImporter Create(LoadFile args, Controller processController, Guid processId, int timezoneoffset, bool autoDetect, bool initializeUploaders, bool doRetryLogic,
			string bulkLoadFileFieldDelimiter, bool isCloudInstance, ExecutionSource executionSource);
	}
}