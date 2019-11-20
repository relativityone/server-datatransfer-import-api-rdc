// ----------------------------------------------------------------------------
// <copyright file="RandomFolderGenerator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;

	public class RandomFolderGenerator
	{
		private readonly int numOfPaths;
		private readonly List<string> folders;
		private readonly List<List<int>> paths;

		public RandomFolderGenerator(int numOfPaths, int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength, int percentOfSpecial)
		{
			this.numOfPaths = numOfPaths;

			var random = new Random(42);
			List<char> characters = GetCharactersForFolderName(percentOfSpecial);

			this.folders = new List<string>();
			for (int i = 0; i < numOfDifferentFolders; i++)
			{
				int folderLength = random.NextBiased(1, maxFolderLength);
				var builder = new StringBuilder(folderLength);
				for (int j = 0; j < folderLength; j++)
				{
					builder.Append(random.NextElement(characters));
				}

				this.folders.Add(builder.ToString());
			}

			this.paths = new List<List<int>>();
			for (int i = 0; i < numOfDifferentPaths; ++i)
			{
				int depth = random.NextBiased(1, maxDepth);
				var last = new List<int>();
				for (int j = 0; j < depth; j++)
				{
					last.Add(random.NextIndex(this.folders));
				}

				this.paths.Add(last);
			}
		}

		public IEnumerable<FolderImportDto> ToEnumerable()
		{
			// Create the random object with the same seed to make the IEnumerable repeatable for validation purposes.
			var random = new Random(42 * 42);
			List<Func<string, string>> modifiers = GetModifiers();

			for (int i = 0; i < this.numOfPaths; i++)
			{
				IEnumerable<string> selectedFolders = random.NextElement(this.paths)
					.Select(p => random.NextElement(modifiers)(this.folders[p]));

				string path = string.Join("\\", selectedFolders);
				yield return new FolderImportDto(i.ToString(), path);
			}
		}

		private static List<Func<string, string>> GetModifiers()
		{
			var modifiers = new List<Func<string, string>>
			{
				folder => folder,
				folder =>
					{
						if (folder.Length > 250)
						{
							return folder;
						}

						return folder + "   ";
					},
				folder => folder.ToLowerInvariant(),
				folder => folder.ToUpperInvariant(),
				folder => folder.Replace('j', 'J'),
				folder => folder.Replace('S', 's'),
				folder =>
					{
						if (folder.Length > 254)
						{
							return folder;
						}

						return folder.Replace("ß", "ss");
					},
			};

			return modifiers;
		}

		private static List<char> GetCharactersForFolderName(int percentOfSpecial)
		{
			const string special = @"*/:?<>""|$ ";
			var characters = new List<char>();

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

			int timesOfSpecialCharacters = (int)Math.Ceiling((characters.Count - special.Length) * (percentOfSpecial / 100.0 / special.Length));
			for (int i = 0; i < timesOfSpecialCharacters; i++)
			{
				characters.AddRange(special);
			}

			return characters;
		}
	}
}
