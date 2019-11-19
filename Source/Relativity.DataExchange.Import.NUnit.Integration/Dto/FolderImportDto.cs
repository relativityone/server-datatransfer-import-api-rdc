// ----------------------------------------------------------------------------
// <copyright file="FolderImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.TestFramework;

	public class FolderImportDto
	{
		public FolderImportDto(string controlNumber, string folder)
		{
			this.ControlNumber = controlNumber;
			this.Folder = folder;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; set; }

		[DisplayName(WellKnownFields.FolderName)]
		public string Folder { get; set; }

		public static IEnumerable<FolderImportDto> GetRandomFolders(int numOfPaths, int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength, int percentOfSpecial)
		{
			const string special = @"*/:?<>""|$ ";
			var characters = new List<char>();

			var random = new Random(42);

			for (int i = ' '; i <= '~'; ++i)
			{
				if (i != '\\')
				{
					characters.Add((char)i);
				}
			}

			for (int i = 'À'; i <= 'ÿ'; ++i)
			{
				characters.Add((char)i);
			}

			int count = (int)Math.Ceiling((characters.Count - special.Length) * (percentOfSpecial / 100.0 / special.Length));
			for (int i = 0; i < count; i++)
			{
				characters.AddRange(special);
			}

			var folders = new List<string>();
			for (int i = 0; i < numOfDifferentFolders; i++)
			{
				int folderLength = (int)Math.Round((random.NextDouble() * random.NextDouble() * maxFolderLength) + 1);
				var builder = new StringBuilder(folderLength);
				for (int j = 0; j < folderLength; j++)
				{
					builder.Append(characters[random.Next(characters.Count)]);
				}

				folders.Add(builder.ToString());
			}

			var paths = new List<List<int>>();
			for (int i = 0; i < numOfDifferentPaths; ++i)
			{
				int depth = (int)Math.Round((random.NextDouble() * random.NextDouble() * maxDepth) + 1);
				var last = new List<int>();
				for (int j = 0; j < depth; j++)
				{
					last.Add(random.Next(folders.Count));
				}

				paths.Add(last);
			}

			var modifiers = new List<Func<int, string>>
			{
				folderPos => folders[folderPos],
				folderPos =>
					{
						if (folders[folderPos].Length > 250)
						{
							return folders[folderPos];
						}

						return folders[folderPos] + new string(
								   Enumerable.Repeat(' ', random.Next(1, 5)).ToArray());
					},
				folderPos => folders[folderPos].ToLower(),
				folderPos => folders[folderPos].ToUpper(),
				folderPos => folders[folderPos].Replace('j', 'J'),
				folderPos => folders[folderPos].Replace('S', 's'),
				folderPos =>
					{
						if (folders[folderPos].Length > 254)
						{
							return folders[folderPos];
						}

						return folders[folderPos].Replace("ß", "ss");
					},
			};

			for (int i = 0; i < numOfPaths; i++)
			{
				string path = string.Join("\\", paths[random.Next(paths.Count)]
					.Select(p => modifiers[random.Next(modifiers.Count)](p)));
				yield return new FolderImportDto(i.ToString(), path);
			}
		}
	}
}
