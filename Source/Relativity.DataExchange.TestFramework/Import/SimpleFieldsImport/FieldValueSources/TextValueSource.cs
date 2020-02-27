// <copyright file="TextValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System.Collections;
	using System.Collections.Generic;

	public class TextValueSource : IFieldValueSource
	{
		private static readonly Dictionary<int, string> Cache = new Dictionary<int, string>();

		public TextValueSource(int textLength)
		{
			this.TextLength = textLength;
		}

		public int TextLength { get; }

		public IEnumerable CreateValuesEnumerator()
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
	}
}
