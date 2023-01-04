// <copyright file="WholeNumberValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.Linq;

	[Serializable]
	public class WholeNumberValueSource : IFieldValueSource
	{
		public IEnumerable CreateValuesEnumerator() => Enumerable.Range(0, int.MaxValue);
	}
}
