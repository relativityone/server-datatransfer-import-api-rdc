using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public static class ListHelper
	{
		public static IList<T> InList<T>(this T item)
		{
			return new List<T> {item};
		}
	}
}