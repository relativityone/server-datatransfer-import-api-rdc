namespace Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup
{
	public interface IImage
	{
		void ConvertImagesToMultiPagePdf(string[] inputFiles, string outputFile);
		void ConvertTIFFsToMultiPage(string[] inputFiles, string outputFile);
	}
}