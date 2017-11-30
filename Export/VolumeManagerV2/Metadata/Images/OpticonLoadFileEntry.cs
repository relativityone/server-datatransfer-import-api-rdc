using System;
using System.Collections.Generic;
using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class OpticonLoadFileEntry : ILoadFileEntry
	{
		public KeyValuePair<string, string> Create(string batesNumber, string filePath, int pageNumber, long pageOffset, int numberOfImages)
		{
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