// ----------------------------------------------------------------------------
// <copyright file="DefaultImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;

	using Relativity.DataExchange.TestFramework;

	public class DefaultImportDto
	{
		public DefaultImportDto(string controlNumber, string filePath)
		{
			this.ControlNumber = controlNumber;
			this.FilePath = filePath;
		}

		public DefaultImportDto(string filePath)
			: this(Path.GetFileName(filePath), filePath)
		{
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName(WellKnownFields.FilePath)]
		public string FilePath { get; }

		public static IEnumerable<DefaultImportDto> GetRandomTextFiles(string directory, int maxFiles)
		{
			const int MinTestFileLength = 1024;
			const int MaxTestFileLength = 10 * MinTestFileLength;
			for (int i = 0; i < maxFiles; i++)
			{
				string file = RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					directory,
					false);

				yield return new DefaultImportDto(file);
			}
		}
	}
}
