// ----------------------------------------------------------------------------
// <copyright file="FileExportHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Net;

	using Relativity.DataExchange.TestFramework;

	public static class FileExportHelper
	{
		public static IEnumerable<FileDto> QueryImageFileInfo(int artifactId)
		{
			using (kCura.WinEDDS.Service.Export.ISearchManager searchManager = CreateExportSearchManager())
			{
				var dataSet = searchManager.RetrieveImagesForDocuments(
					AssemblySetup.TestParameters.WorkspaceId,
					new[] { artifactId });

				if (dataSet == null || dataSet.Tables.Count == 0)
				{
					return new List<FileDto>();
				}

				DataTable dataTable = dataSet.Tables[0];
				return dataTable.Rows.Cast<DataRow>().Select(dataRow => new FileDto(dataRow));
			}
		}

		private static kCura.WinEDDS.Service.Export.ISearchManager CreateExportSearchManager()
		{
			var credentials = new NetworkCredential(AssemblySetup.TestParameters.RelativityUserName, AssemblySetup.TestParameters.RelativityPassword);
			return new kCura.WinEDDS.Service.SearchManager(credentials, new CookieContainer());
		}
	}
}