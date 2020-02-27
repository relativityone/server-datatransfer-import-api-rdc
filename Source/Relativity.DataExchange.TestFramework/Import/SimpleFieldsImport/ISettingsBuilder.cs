// <copyright file="ISettingsBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using kCura.Relativity.DataReaderClient;

	public interface ISettingsBuilder<out TSettings>
		where TSettings : ImportSettingsBase
	{
		TSettings Build();
	}
}
