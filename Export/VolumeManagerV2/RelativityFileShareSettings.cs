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
			FileshareName = fileshareName;
			FileshareUri = new Uri(fileshareName);
			TransferCredential = credential;
		}

		public RelativityFileShareSettings(RelativityFileShare fileShare) : this(fileShare.Url, fileShare.TransferCredential)
		{

		}
	}
}