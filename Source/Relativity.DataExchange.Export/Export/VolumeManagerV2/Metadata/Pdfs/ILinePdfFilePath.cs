// ----------------------------------------------------------------------------
// <copyright file="ILinePdfFilePath.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Pdfs
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public interface ILinePdfFilePath
	{
		void AddPdfFilePath(DeferredEntry loadFileEntry, ObjectExportInfo artifact);
	}
}