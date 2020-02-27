// ----------------------------------------------------------------------------
// <copyright file="RandomPathGenerator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.TestFramework;

	public class RandomPathGenerator
	{
		private readonly string[] elements;
		private readonly List<List<int>> paths;

		public RandomPathGenerator(IEnumerable<string> elements, int numOfDifferentPaths, int maxPathDepth)
		{
			this.elements = elements.ToArray();

			var random = new Random(42);

			this.paths = new List<List<int>>();
			for (int i = 0; i < numOfDifferentPaths; ++i)
			{
				int depth = random.NextBiased(1, maxPathDepth);
				var last = new List<int>();
				for (int j = 0; j < depth; j++)
				{
					last.Add(random.NextIndex(this.elements));
				}

				this.paths.Add(last);
			}
		}

		public static RandomPathGenerator GetFolderGenerator(int numOfDifferentElements, int maxElementLength, int numOfDifferentPaths, int maxPathDepth)
		{
			const string Special = @"*/:?<>""|$ ";
			List<char> characters = GetCharacters().Where(p => p != Path.DirectorySeparatorChar).ToList();
			const int TimesSpecial = 3;
			for (int i = 0; i < TimesSpecial; i++)
			{
				characters.AddRange(Special);
			}

			IEnumerable<string> elements = GetElements(characters, numOfDifferentElements, maxElementLength);

			return new RandomPathGenerator(elements, numOfDifferentPaths, maxPathDepth);
		}

		public static RandomPathGenerator GetChoiceGenerator(int numOfDifferentElements, int maxElementLength, int numOfDifferentPaths, int maxPathDepth, char multiValueDelimiter, char nestedValueDelimiter)
		{
			List<char> characters = GetCharacters().Where(p => p != multiValueDelimiter && p != nestedValueDelimiter).ToList();
			IEnumerable<string> elements = GetElements(characters, numOfDifferentElements, maxElementLength);
			return new RandomPathGenerator(elements, numOfDifferentPaths, maxPathDepth);
		}

		public static RandomPathGenerator GetObjectGenerator(int numOfDifferentElements, int maxElementLength, int numOfDifferentPaths, int maxPathDepth, char multiValueDelimiter)
		{
			List<char> characters = GetCharacters().Where(p => p != multiValueDelimiter).ToList();
			IEnumerable<string> elements = GetElements(characters, numOfDifferentElements, maxElementLength);
			return new RandomPathGenerator(elements, numOfDifferentPaths, maxPathDepth);
		}

		/// <summary>
		/// This method calculates folder paths.
		/// </summary>
		/// <returns>Infinite enumerable of folders.</returns>
		public IEnumerable<string> ToFolders()
		{
			return this.ToEnumerable(Path.DirectorySeparatorChar.ToString());
		}

		/// <summary>
		/// This method calculates folder paths.
		/// </summary>
		/// <param name="numOfPaths">Number of paths to return.</param>
		/// <returns>It returns <paramref name="numOfPaths"/> folder paths.</returns>
		public IEnumerable<string> ToFolders(int numOfPaths)
		{
			return this.ToFolders().Take(numOfPaths);
		}

		public IEnumerable<string> ToEnumerable()
		{
			return this.ToEnumerable(separator: string.Empty);
		}

		public IEnumerable<string> ToEnumerable(int numOfPaths)
		{
			return this.ToEnumerable(string.Empty).Take(numOfPaths);
		}

		public IEnumerable<string> ToEnumerable(char separator)
		{
			return this.ToEnumerable(separator.ToString());
		}

		public IEnumerable<string> ToEnumerable(char separator, int numOfPaths)
		{
			return this.ToEnumerable(separator).Take(numOfPaths);
		}

		private static IEnumerable<string> GetElements(List<char> characters, int numOfDifferentElements, int maxElementLength)
		{
			var random = new Random(42);

			for (int i = 0; i < numOfDifferentElements; i++)
			{
				int folderLength = random.NextBiased(1, maxElementLength);
				var builder = new StringBuilder(folderLength);
				for (int j = 0; j < folderLength; j++)
				{
					builder.Append(random.NextElement(characters));
				}

				yield return builder.ToString();
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "todo")]
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

		private static List<char> GetCharacters()
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

		private IEnumerable<string> ToEnumerable(string separator)
		{
			// Create the random object with the same seed to make the IEnumerable repeatable for validation purposes.
			var random = new Random(42 * 42);
			List<Func<string, string>> modifiers = GetModifiers();

			while (true)
			{
				IEnumerable<string> selectedFolders = random.NextElement(this.paths)
					.Select(p => random.NextElement(modifiers)(this.elements[p]));

				string path = string.Join(separator, selectedFolders);
				yield return path;
			}
		}
	}
}