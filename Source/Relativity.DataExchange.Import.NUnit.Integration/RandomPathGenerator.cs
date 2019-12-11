// ----------------------------------------------------------------------------
// <copyright file="RandomPathGenerator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.TestFramework;

	public class RandomPathGenerator
	{
		private readonly List<string> folders;
		private readonly List<List<int>> paths;

		public RandomPathGenerator(List<char> characters, int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength)
		{
			var random = new Random(42);

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

		public static RandomPathGenerator GetFolderGenerator(int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength)
		{
			const string Special = @"*/:?<>""|$ ";
			List<char> characters = GetCharactersForFolderName().Where(p => p != Path.PathSeparator).ToList();
			for (int i = 0; i < 3; i++)
			{
				characters.AddRange(Special);
			}

			return new RandomPathGenerator(characters, maxDepth, numOfDifferentFolders, numOfDifferentPaths, maxFolderLength);
		}

		public static RandomPathGenerator GetChoiceGenerator(int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength, char multiValueDelimiter, char nestedValueDelimiter)
		{
			List<char> characters = GetCharactersForFolderName().Where(p => p != multiValueDelimiter && p != nestedValueDelimiter).ToList();

			return new RandomPathGenerator(characters, maxDepth, numOfDifferentFolders, numOfDifferentPaths, maxFolderLength);
		}

		public static RandomPathGenerator GetObjectGenerator(int maxDepth, int numOfDifferentFolders, int numOfDifferentPaths, int maxFolderLength, char multiValueDelimiter)
		{
			List<char> characters = GetCharactersForFolderName().Where(p => p != multiValueDelimiter).ToList();

			return new RandomPathGenerator(characters, maxDepth, numOfDifferentFolders, numOfDifferentPaths, maxFolderLength);
		}

		public IEnumerable<string> ToFolders(int numOfPaths)
		{
			return this.ToEnumerable(numOfPaths, Path.PathSeparator);
		}

		public IEnumerable<string> ToEnumerable(int numOfPaths)
		{
			return this.ToEnumerable(numOfPaths, string.Empty);
		}

		public IEnumerable<string> ToEnumerable(int numOfPaths, char separator)
		{
			return this.ToEnumerable(numOfPaths, separator.ToString());
		}

		public IEnumerable<string> ToEnumerable(int numOfPaths, string separator)
		{
			// Create the random object with the same seed to make the IEnumerable repeatable for validation purposes.
			var random = new Random(42 * 42);
			List<Func<string, string>> modifiers = GetModifiers();

			for (int i = 0; i < numOfPaths; i++)
			{
				IEnumerable<string> selectedFolders = random.NextElement(this.paths)
					.Select(p => random.NextElement(modifiers)(this.folders[p]));

				string path = string.Join(separator, selectedFolders);
				yield return path;
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

		private static List<char> GetCharactersForFolderName()
		{
			var characters = new List<char>();

			for (int i = ' '; i <= '~'; ++i)
			{
				characters.Add((char)i);
			}

			for (int i = 'À'; i <= 'ÿ'; ++i)
			{
				characters.Add((char)i);
			}

			return characters;
		}
	}
}
