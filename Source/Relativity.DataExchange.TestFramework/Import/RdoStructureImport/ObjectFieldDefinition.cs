// <copyright file="ObjectFieldDefinition.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.RdoStructureImport
{
	public class ObjectFieldDefinition : FieldDefinition<RdoStructureDefinition>
	{
		public ObjectFieldDefinition(int numberOfFieldsToCreate, RdoStructureDefinition valuesSource)
			: this(numberOfFieldsToCreate, valuesSource, 1)
		{
		}

		public ObjectFieldDefinition(int numberOfFieldsToCreate, RdoStructureDefinition valuesSource, int maxNumberOfMultiValues)
			: this(numberOfFieldsToCreate, valuesSource, maxNumberOfMultiValues, SettingsConstants.DefaultMultiValueDelimiter.ToString())
		{
		}

		public ObjectFieldDefinition(int numberOfFieldsToCreate, RdoStructureDefinition valuesSource, int maxNumberOfMultiValues, string multiValuesDelimiter)
			: base(numberOfFieldsToCreate, isOpenToAssociations: false, valuesSource)
		{
			this.MaxNumberOfMultiValues = maxNumberOfMultiValues;
			this.MultiValuesDelimiter = multiValuesDelimiter;
		}

		public int MaxNumberOfMultiValues { get; }

		public string MultiValuesDelimiter { get; }
	}
}
