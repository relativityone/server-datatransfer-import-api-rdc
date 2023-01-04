// <copyright file="FieldDefinition.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.RdoStructureImport
{
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;

	public static class FieldDefinition
	{
		public static FieldDefinition<T> CreateAssociative<T>(int numberOfFieldsToCreate, T valuesSource)
			where T : IFieldValueSource
		{
			return new FieldDefinition<T>(numberOfFieldsToCreate, true, valuesSource);
		}

		public static FieldDefinition<T> CreateNonAssociative<T>(int numberOfFieldsToCreate, T valuesSource)
			where T : IFieldValueSource
		{
			return new FieldDefinition<T>(numberOfFieldsToCreate, false, valuesSource);
		}
	}

#pragma warning disable SA1402 // File may only contain a single type
	public class FieldDefinition<T> : IFieldDefinition<T>
#pragma warning restore SA1402 // File may only contain a single type
	{
		public FieldDefinition(int numberOfFieldsToCreate, bool isOpenToAssociations, T valuesSource)
		{
			this.NumberOfFieldsToCreate = numberOfFieldsToCreate;
			this.ValuesSource = valuesSource;
			this.IsOpenToAssociations = isOpenToAssociations;
		}

		public int NumberOfFieldsToCreate { get; }

		public T ValuesSource { get; }

		public bool IsOpenToAssociations { get; }
	}

#pragma warning disable SA1201 // Elements should appear in the correct order
	public interface IFieldDefinition<out T> // Interface is required to make generic type covariant
#pragma warning restore SA1201 // Elements should appear in the correct order
	{
		int NumberOfFieldsToCreate { get; }

		T ValuesSource { get; }
	}
}
