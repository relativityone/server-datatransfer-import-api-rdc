namespace Relativity.Export.VolumeManagerV2
{
	using System.Collections.Generic;

	public static class ListHelper
	{
		public static IList<T> InList<T>(this T item)
		{
			return new List<T> {item};
		}
	}
}