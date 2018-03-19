using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class RelativityFileShareSettings
	{
		public AsperaCredential TransferCredential { get; set; }

		public string FileshareName { get; set; }

		public Uri FileshareUri { get; set; }


		public RelativityFileShareSettings() { }

		public RelativityFileShareSettings(string fileshareName, AsperaCredential credential)
		{
			//todo Adrian: this is a hack because of tapirap env do not stick to naming conventions.
			FileshareName = fileshareName.Replace('/', '\\') + "files";
			FileshareUri = new Uri(fileshareName);
			TransferCredential = credential;
		}

		//todo Adrian: this is a hack because of tapirap env do not stick to naming conventions. Use RelativityFileShare.Url instead of RelativityFileShare.DocRoot!
		public RelativityFileShareSettings(RelativityFileShare fileShare) : this(fileShare.DocRoot, fileShare.TransferCredential)
		{

		}
	}
}