﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="OpticonLoadFileEntryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.Logging;

	[TestFixture]
	public class OpticonLoadFileEntryTests
    {
		private OpticonLoadFileEntry _instance;

		[SetUp]
		public void SetUp()
		{
			this._instance = new OpticonLoadFileEntry(new NullLogger());
		}

		[Test]
		[TestCaseSource(nameof(DataSets))]
		public void ItShouldCreateOpticonEntry(ImageLoadFileEntryDataSet dataSet)
		{
			// ACT
			string actualResult = this._instance.Create(dataSet.BatesNumber, dataSet.FilePath, dataSet.Volume, dataSet.PageNumber, dataSet.NumberOfImages);

			// ASSERT
			Assert.That(actualResult, Is.EqualTo(dataSet.ExpectedResult));
		}

		private static IEnumerable<ImageLoadFileEntryDataSet> DataSets()
		{
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = "F",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 1,
				ExpectedResult = $"B,V,F,Y,,,1{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = "F",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 999,
				ExpectedResult = $"B,V,F,Y,,,999{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = "F",
				Volume = "V",
				PageNumber = 2,
				NumberOfImages = 1,
				ExpectedResult = $"B,V,F,,,,{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = "F",
				Volume = "V",
				PageNumber = 2,
				NumberOfImages = 999,
				ExpectedResult = $"B,V,F,,,,{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "veeeery_loooong_bates_number",
				FilePath = "veeeery_loooong_file_path",
				Volume = "veeeery_loooong_volume",
				PageNumber = 1,
				NumberOfImages = 5,
				ExpectedResult = $"veeeery_loooong_bates_number,veeeery_loooong_volume,veeeery_loooong_file_path,Y,,,5{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "veeeery_loooong_bates_number",
				FilePath = "veeeery_loooong_file_path",
				Volume = "veeeery_loooong_volume",
				PageNumber = 2,
				NumberOfImages = 5,
				ExpectedResult = $"veeeery_loooong_bates_number,veeeery_loooong_volume,veeeery_loooong_file_path,,,,{Environment.NewLine}"
			};
		}
	}
}