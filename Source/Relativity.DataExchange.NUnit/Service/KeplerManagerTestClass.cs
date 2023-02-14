// ----------------------------------------------------------------------------
// <copyright file="KeplerManagerTestClass.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Service
{
	using System;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS.Mapping;
	using kCura.WinEDDS.Service.Replacement;
	using Relativity.DataExchange.Service;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;

	public class KeplerManagerTestClass : KeplerManager
	{
		public KeplerManagerTestClass(IServiceProxyFactory serviceProxyFactory, IServiceExceptionMapper exceptionMapper, Func<string> correlationIdFunc)
			: base(serviceProxyFactory, exceptionMapper, correlationIdFunc)
		{
		}

		public MassImportResults BulkImportNativeMock(int appID, NativeLoadInfo settings, bool inRepository, bool includeExtractedTextEncoding)
		{
			return this.Execute(async s =>
					{
						using (var importer = s.CreateProxyInstance<IBulkImportService>())
						{
							DataTransfer.Legacy.SDK.ImportExport.V1.Models.MassImportResults result = await importer.BulkImportNativeAsync(appID, KeplerTypeMapper.Map(settings), inRepository, includeExtractedTextEncoding, this.CorrelationIdFunc?.Invoke()).ConfigureAwait(false);
							return KeplerTypeMapper.Map(result);
						}
					});
		}
	}
}