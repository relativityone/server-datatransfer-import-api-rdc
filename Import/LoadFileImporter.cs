using kCura.WinEDDS.Api;
using kCura.WinEDDS.CodeValidator;

namespace kCura.WinEDDS.Core.Import
{
	public class LoadFileImporter : LoadFileBase, ILoadFileImporter
	{
		public LoadFileImporter(LoadFile args, int timezoneoffset, bool doRetryLogic, bool autoDetect) : base(args, timezoneoffset, doRetryLogic, autoDetect)
		{
		}

		public LoadFileImporter(LoadFile args, int timezoneoffset, bool doRetryLogic, bool autoDetect, bool initializeArtifactReader) : base(args, timezoneoffset, doRetryLogic, autoDetect, initializeArtifactReader)
		{
		}

		protected override bool UseTimeZoneOffset { get; }
		protected override Base GetSingleCodeValidator()
		{
			return new SingleImporter(_settings.CaseInfo, _codeManager);
		}

		protected override IArtifactReader GetArtifactReader()
		{
			return new LoadFileReader(_settings, false);
		}

		public object ReadFile(string path)
		{
			throw new System.NotImplementedException();
		}
	}
}
