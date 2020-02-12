// <copyright file="IFieldValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources
{
	using System.Collections;

	public interface IFieldValueSource
	{
		IEnumerable CreateValuesEnumerator();
	}
}
