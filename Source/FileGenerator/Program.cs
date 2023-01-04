using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileGenerator
{
	public class Program
	{
		private readonly LoadFileHelper _lfh;
		private readonly FileUtilities _fileUtilities;
		private readonly RandomGen _randomGen;
		private readonly Config _config;
		private const FileUtilities.Unit NativeSizeUnit = FileUtilities.Unit.B;
		private const FileUtilities.Unit ExtractedTextSizeUnit = FileUtilities.Unit.B;
		private DateTime _startTime;

		public static void Main(string[] args)
		{
			var prg = new Program();
			prg.GenerateFiles();
		}

		public Program()
		{
			_config = new Config();
			IConfigurationBuilder configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			IConfigurationRoot configurationRoot = configuration.Build();

			configurationRoot.Bind(_config);
			_lfh = new LoadFileHelper();
			_fileUtilities = new FileUtilities();
			configurationRoot.Bind(_config);
			if (_config.FileSizeDistribution == null) throw new Exception("FileSizeDistribution must not be empty");
			_randomGen = new RandomGen(_config.FileSizeDistribution, _config.Seed1, _config.Seed2);

			_config.Dump();
		}

		private void GenerateFiles()
		{
			FileStream fs = null;
			try
			{
				if (_config.LoadFilePath == null) throw new Exception("Load file path must not be empty");
				if (_config.LoadFileName == null) throw new Exception("LoadFileName must not be empty");
				fs = _fileUtilities.CreateLoadFile(_config.LoadFilePath, _config.LoadFileName);
				string header = _lfh.CreateLoadFileHeader();
				_fileUtilities.WriteToLoadFile(fs, header + "\r\n");

				var ctrlNum = 1;
				var stopNewFilesCreation = false;

				for (var f = 1; f <= _config.NumberOfFolders; f++)
				{
					if (stopNewFilesCreation)
						break;

					Console.WriteLine($"Current time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
					_startTime = DateTime.Now;
					string nativesPath = _config.NativeFilesPath + "\\" + $"NATIVE{f:D5}";
					_fileUtilities.CreateDirectory(nativesPath);
					Console.WriteLine($"Creating folder path: {nativesPath}");

					var extractedPath = "";
					if (_config.GenerateExtractedTextFiles)
					{
						extractedPath = _config.ExtractedFilesPath + "\\" + $"TEXT{f:D5}";
						_fileUtilities.CreateDirectory(extractedPath);
						Console.WriteLine($"Creating folder path: {extractedPath}");
					}

					for (var i = 1; i <= _config.FilesInFolder; i++)
					{
						int size = _randomGen.GenerateSize();
						if (size > 0)
						{
							var nativeFileName = $"NATIVE_{ctrlNum,7:D8}.txt";
							var fileSize = new FileUtilities.FileSize(size, NativeSizeUnit);
							_fileUtilities.CreateFile(nativesPath, nativeFileName, fileSize);

							var extractedFileName = "";
							if (_config.GenerateExtractedTextFiles)
							{
								int extractedTextSize = _randomGen.GenerateSize();
								if (extractedTextSize > 0)
								{
									extractedFileName = $"EXTRACTED_{ctrlNum,7:D8}.txt";
									fileSize = new FileUtilities.FileSize(extractedTextSize, ExtractedTextSizeUnit);
									_fileUtilities.CreateFile(extractedPath, extractedFileName, fileSize);
								}
								else
								{
									stopNewFilesCreation = true;
									break;
								}
							}

							string line = _lfh.CreateLoadFileLine(
								ctrlNum,
								GetCustodianNumberFrom(ctrlNum, _config.NumberOfDocumentsPerCustodian),
								"srcFile",
								_config.GenerateExtractedTextFiles
									? Path.Combine(new string[] {@".\", GetRelativePath(_config.LoadFilePath, extractedPath), extractedFileName})
									: "",
								_config.GenerateExtractedTextFiles ? extractedFileName : "",
								(int) fileSize.ByteCount,
								Path.Combine(new string[]
									{@".\", GetRelativePath(_config.LoadFilePath, nativesPath), nativeFileName}));

							_fileUtilities.WriteToLoadFile(fs, line + "\r\n");
							ctrlNum++;
						}
						else
						{
							stopNewFilesCreation = true;
							break;
						}
					}
				}
				Console.WriteLine($"Current time: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
				Console.WriteLine($"Duration : {DateTime.Now.Subtract(_startTime)}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				if (fs != null)
					_fileUtilities.CloseStream(fs);
			}
		}

		private static int GetCustodianNumberFrom(int ctrlNum, int custodianNumberOfDocuments)
		{
			return ctrlNum/custodianNumberOfDocuments + 1;
		}

		public string GetRelativePath(string relativeTo, string path)
		{
			var uri = new Uri(relativeTo);
			string rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
			{
				rel = $".{ Path.DirectorySeparatorChar }{ rel }";
			}
			return rel;
		}
	}
}