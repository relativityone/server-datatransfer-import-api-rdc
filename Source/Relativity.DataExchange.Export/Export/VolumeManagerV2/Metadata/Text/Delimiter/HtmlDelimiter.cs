﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text.Delimiter
{
	public class HtmlDelimiter : IDelimiter
	{
		public string Start => "<td>";
		public string End => "</td>";
	}
}