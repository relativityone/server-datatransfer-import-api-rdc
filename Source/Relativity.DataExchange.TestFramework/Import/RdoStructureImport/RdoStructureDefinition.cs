// <copyright file="RdoStructureDefinition.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.RdoStructureImport
{
	using System.Collections.Generic;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;

	public class RdoStructureDefinition
	{
		private const string DoNotNestGenericTypesJustification = "This design is better than creating a lot of new types.";

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = DoNotNestGenericTypesJustification)]
		public RdoStructureDefinition(
			int numberOfRecordsToImport,
			string identifierFieldName,
			IdentifierValueSource identifierValueSource,
			FoldersValueSource foldersValueSource,
			IEnumerable<FieldDefinition<ChoicesValueSource>> singleChoiceFieldDefinitions,
			IEnumerable<FieldDefinition<ChoicesValueSource>> multiChoiceFieldDefinitions,
			IEnumerable<FieldDefinition<TextValueSource>> textFieldDefinitions,
			IEnumerable<FieldDefinition<WholeNumberValueSource>> wholeNumberFieldDefinitions,
			IEnumerable<ObjectFieldDefinition> singleObjectsFieldDefinitions,
			IEnumerable<ObjectFieldDefinition> multiObjectsFieldDefinitions)
		{
			this.NumberOfRecordsToImport = numberOfRecordsToImport;
			this.IdentifierFieldName = identifierFieldName;
			this.IdentifierValueSource = identifierValueSource;
			this.FoldersValueSource = foldersValueSource;
			this.SingleChoiceFieldDefinitions = singleChoiceFieldDefinitions;
			this.MultiChoiceFieldDefinitions = multiChoiceFieldDefinitions;
			this.LongTextFieldDefinitions = textFieldDefinitions;
			this.WholeNumberFieldDefinitions = wholeNumberFieldDefinitions;
			this.SingleObjectsFieldDefinitions = singleObjectsFieldDefinitions;
			this.MultiObjectsFieldDefinitions = multiObjectsFieldDefinitions;
		}

		public int NumberOfRecordsToImport { get; }

		public string IdentifierFieldName { get; }

		public IdentifierValueSource IdentifierValueSource { get; }

		public FoldersValueSource FoldersValueSource { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = DoNotNestGenericTypesJustification)]
		public IEnumerable<FieldDefinition<ChoicesValueSource>> SingleChoiceFieldDefinitions { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = DoNotNestGenericTypesJustification)]
		public IEnumerable<FieldDefinition<ChoicesValueSource>> MultiChoiceFieldDefinitions { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = DoNotNestGenericTypesJustification)]
		public IEnumerable<FieldDefinition<TextValueSource>> LongTextFieldDefinitions { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = DoNotNestGenericTypesJustification)]
		public IEnumerable<FieldDefinition<WholeNumberValueSource>> WholeNumberFieldDefinitions { get; }

		public IEnumerable<ObjectFieldDefinition> SingleObjectsFieldDefinitions { get; }

		public IEnumerable<ObjectFieldDefinition> MultiObjectsFieldDefinitions { get; }
	}
}