// ----------------------------------------------------------------------------
// <copyright file="ExporterTestData.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.DataExchange.TestFramework;

	public static class ExporterTestData
	{
		/// <summary>
		/// The dummy UNC path. This should never be used for positive tests.
		/// </summary>
		public const string DummyUncPath = @"\\files\T001\Files\EDDS123456\";

		public const string SampleDocPdfFileName = "EDRM-Sample1.pdf";
		public const string SampleDocWordFileName = "EDRM-Sample2.doc";
		public const string SampleDocExcelFileName = "EDRM-Sample3.xlsx";
		public const string SampleDocMsgFileName = "EDRM-Sample4.msg";
		public const string SampleDocHtmFileName = "EDRM-Sample5.htm";
		public const string SampleDocEmfFileName = "EDRM-Sample6.emf";
		public const string SampleDocPptFileName = "EDRM-Sample7.ppt";
		public const string SampleDocPngFileName = "EDRM-Sample8.png";
		public const string SampleDocTxtFileName = "EDRM-Sample9.txt";
		public const string SampleDocWmfFileName = "EDRM-Sample10.wmf";
		public const string SampleImage1FileName = "EDRM-Sample1.tif";
		public const string SampleImage2FileName = "EDRM-Sample2.tif";
		public const string SampleImage3FileName = "EDRM-Sample3.tif";
		public const string SampleProductionImage1FileName = "EDRM-Sample-000001.tif";

		public static IEnumerable<string> AllSampleDocFileNames =>
			new[]
				{
					SampleDocPdfFileName,
					SampleDocWordFileName,
					SampleDocExcelFileName,
					SampleDocMsgFileName,
					SampleDocHtmFileName,
					SampleDocEmfFileName,
					SampleDocPptFileName,
					SampleDocPngFileName,
					SampleDocTxtFileName,
					SampleDocWmfFileName,
				};

		public static IEnumerable<string> AllSampleImageFileNames =>
			new[]
				{
					SampleImage1FileName,
					SampleImage2FileName,
					SampleImage3FileName,
					SampleProductionImage1FileName,
				};

		public static IEnumerable<string> AllSampleFiles =>
			AllSampleDocFileNames
				.Select(ResourceFileHelper.GetDocsResourceFilePath)
				.Concat(AllSampleImageFileNames
					.Select(ResourceFileHelper.GetImagesResourceFilePath));
	}
}