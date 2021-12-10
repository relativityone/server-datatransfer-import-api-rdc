using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FileGenerator
{
	public class Config
	{
		/// <summary>
		/// File size distribution - number of files per range of sizes.
		/// 1024  5			- generates 5 files with size randomly taken from [1024, 1024+1024/2)
		/// 2048  10		- generates 10 files with size randomly taken from [2048, 2048+2048/2)
		///
		/// For each size generation the number of files per given range is decreased.
		/// </summary>
		/// <remarks>This distribution is also used for generation of the extracted text files.</remarks>
		public Dictionary<string, int> FileSizeDistribution { get; set; }

		/// <summary>
		/// Path to folder where generated files with extracted Text will be placed.
		/// </summary>
		public string ExtractedFilesPath { get; set; }

		/// <summary>
		/// Path to folder where generated native files will be placed.
		/// </summary>
		public string NativeFilesPath { get; set; }

		/// <summary>
		/// Path where generated load file will be placed.
		/// </summary>
		public string LoadFilePath { get; set; }

		/// <summary>
		/// Name of the loadfile.
		/// </summary>
		public string LoadFileName { get; set; }

		/// <summary>
		/// true - enable generation extracted text files.
		/// </summary>
		public bool GenerateExtractedTextFiles { get; set; }

		/// <summary>
		/// Number of documents generated for single custodian.
		/// </summary>
		public int NumberOfDocumentsPerCustodian { get; set; }

		/// <summary>
		/// Seed for uniform random generator used to pick size ranges from FileSizeDistribution
		/// when generating size for single file.
		/// </summary>
		public int Seed1 { get; set; }

		/// <summary>
		/// Seed for uniform random generator used to generate single file size in a given range
		/// when generating size for single file.
		/// </summary>
		public int Seed2 { get; set; }

		/// <summary>
		/// Number of files per folder to generate.
		/// Generation takes place until all size ranges from FileSizeDistribution are exhausted.
		/// </summary>
		public int FilesInFolder { get; set; }

		/// <summary>
		///  Number of folders to generate.
		/// Generation takes place until all size ranges from FileSizeDistribution are exhausted.
		///  </summary>
		public int NumberOfFolders { get; set; }

		public void Dump()
		{
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
			{
				string name = descriptor.Name;
				object value = descriptor.GetValue(this);
				if(descriptor.Name.Equals("FileSizeDistribution"))
				{
					Console.WriteLine("FileSizeDistribution:");
					foreach (KeyValuePair<string, int> range in FileSizeDistribution)
					{
						Console.WriteLine($"Size range: {range.Key}, #files: {range.Value}");
					}

					Console.WriteLine();
				} 
				else
				{
					Console.WriteLine("{0}={1}", name, value);
				}
			}
			Console.WriteLine();
		}
	}
}
