// <copyright file="TextValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	[Serializable]
	public class TextValueSource : IFieldValueSource
	{
		private static readonly Dictionary<int, string> Cache = new Dictionary<int, string>();

		public TextValueSource(int textLength)
			: this(textLength, false)
		{
		}

		public TextValueSource(int textLength, bool useCache)
		{
			this.TextLength = textLength;
			this.UseCache = useCache;
		}

		public int TextLength { get; }

		public bool UseCache { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			if (UseCache)
			{
				if (!Cache.ContainsKey(this.TextLength))
				{
					Cache[this.TextLength] = RandomHelper.NextString(minValue: this.TextLength, maxValue: this.TextLength);
				}

				string value = Cache[this.TextLength];

				while (true)
				{
					yield return value;
				}
			}

			while (true)
			{
				yield return RandomHelper.NextString(minValue: this.TextLength, maxValue: this.TextLength);
			}
		}
	}
}
