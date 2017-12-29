using System;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class FullTextLineWriter : IFullTextLineWriter
	{
		public void WriteLine(string batesNumber, long pageOffset, IRetryableStreamWriter writer, TextReader textReader, CancellationToken token)
		{
			writer.WriteChunk("FT,", token);
			writer.WriteChunk(batesNumber, token);
			writer.WriteChunk(",1,1,", token);
			if (pageOffset == long.MinValue)
			{
				int c = textReader.Read();
				while (c != -1)
				{
					writer.WriteChunk(GetLfpFullTextTransform(c), token);
					c = textReader.Read();
				}
			}
			else
			{
				int i = 0;
				while (i < pageOffset)
				{
					int c = textReader.Read();
					if (c == -1)
					{
						break;
					}
					writer.WriteChunk(GetLfpFullTextTransform(c), token);
					i++;
				}
			}

			writer.WriteChunk(Environment.NewLine, token);
			writer.FlushChunks(token);
		}

		private string GetLfpFullTextTransform(int c)
		{
			const int lineFeed = 10;
			if (c == lineFeed || c == ' ')
			{
				return "|0|0|0|0^";
			}

			if (c == ',')
			{
				return "";
			}

			return Convert.ToChar(c).ToString();
		}
	}
}