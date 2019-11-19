// ----------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.DataExchange.TestFramework;

	public static class TestData
	{
		/// <summary>
		/// The dummy UNC path. This should never be used for positive tests.
		/// </summary>
		public const string DummyUncPath = @"\\files\T001\Files\EDDS123456\";

		public static IEnumerable<string> SampleDocFiles =>
			ResourceFileHelper.GetResourceFolderFiles("Docs");

		public static IEnumerable<string> SampleImageFiles =>
			ResourceFileHelper.GetResourceFolderFiles("Images");

		public static IEnumerable<string> SampleFiles => SampleDocFiles.Concat(SampleImageFiles);
	}
}