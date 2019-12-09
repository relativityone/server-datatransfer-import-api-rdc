// <copyright file="CopySettingsExtenstion.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Extensions
{
	using System.Collections.Generic;
	using System.Linq;

	using kCura.Relativity.DataReaderClient;

	public static class CopySettingsExtenstion
	{
		// This list can be extended for images. This is the list of properties that are crated by ImportJob class itself
		private static readonly List<string> DoNotCopyPropNames = new List<string>() { $"{nameof(Settings.ArtifactTypeId)}" };

		public static void CopyTo<T>(this T source, T destination)
			where T : ImportSettingsBase
		{
			destination.ThrowIfNull(nameof(destination));

			var properties = source.GetType().GetProperties();

			foreach (var propertyInfo in properties.Where(prop => prop.CanRead && prop.CanWrite))
			{
				if (!DoNotCopyPropNames.Contains(propertyInfo.Name))
				{
					object copyValue = propertyInfo.GetValue(source);
					propertyInfo.SetValue(destination, copyValue);
				}
			}
		}
	}
}
