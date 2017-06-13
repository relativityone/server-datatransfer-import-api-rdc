

using System.Text;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	static class UploadModeHelper
	{
		private static readonly string _aspera = "Aspera";

		public static string GetAsperaModeDescriptor(LoadFile loadFile)
		{
			return GetTemplate(loadFile, _aspera);
		}

		private static string GetTemplate(LoadFile loadFile, string mode)
		{
			var stringBuilder = new StringBuilder($"Metadata: {mode}");
			var fileMode = "not copied";
			if (loadFile.CopyFilesToDocumentRepository && !string.IsNullOrEmpty(loadFile.NativeFilePathColumn))
			{
				fileMode = mode;
			}
			stringBuilder.Append($" - Files: {fileMode}");
			return stringBuilder.ToString();
		}
	}
}
