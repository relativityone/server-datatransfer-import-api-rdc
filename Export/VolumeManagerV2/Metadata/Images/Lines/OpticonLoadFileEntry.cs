using System;
using System.Collections.Generic;
using System.Text;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class OpticonLoadFileEntry : IImageLoadFileEntry
	{
		private readonly ILog _logger;

		public OpticonLoadFileEntry(ILog logger)
		{
			_logger = logger;
		}

		public KeyValuePair<string, string> Create(string batesNumber, string filePath, int pageNumber, long pageOffset, int numberOfImages)
		{
			_logger.LogVerbose("Creating Opticon load file entry for image {batesNumber}.", batesNumber);
			StringBuilder line = new StringBuilder();
			//TODO
			line.AppendFormat("{0},{1},{2},", batesNumber, "TODO", filePath);
			if (pageNumber == 1)
			{
				line.Append("Y");
			}
			line.Append(",,,");
			if (pageNumber == 1)
			{
				line.Append(numberOfImages);
			}
			line.Append(Environment.NewLine);

			return new KeyValuePair<string, string>(batesNumber, line.ToString());
		}
	}
}