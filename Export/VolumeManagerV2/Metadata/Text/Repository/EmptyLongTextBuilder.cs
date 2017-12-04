﻿using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class EmptyLongTextBuilder : ILongTextBuilder
	{
		public IList<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			return new List<LongText>();
		}
	}
}