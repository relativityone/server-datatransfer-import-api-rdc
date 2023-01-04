// ----------------------------------------------------------------------------
// <copyright file="PdfFilePathProvider.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	public class PdfFilePathProvider : FilePathProvider
	{
		public PdfFilePathProvider(ILabelManagerForArtifact labelManagerForArtifact, ExportFile exportSettings, IDirectory directoryHelper, ILog logger)
			: base(labelManagerForArtifact, exportSettings, directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return LabelManagerForArtifact.GetPdfSubdirectoryLabel(objectExportInfoArtifactId);
		}
	}
}