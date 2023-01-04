// // ----------------------------------------------------------------------------
// <copyright file="IFieldValueSourceWithPrefix.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	public interface IFieldValueSourceWithPrefix : IFieldValueSource
	{
		IFieldValueSourceWithPrefix CreateFieldValueSourceWithPrefix(string prefix);
	}
}