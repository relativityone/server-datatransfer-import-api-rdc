﻿namespace Relativity.Import.Export.Services
{
	public class ViewFieldInfo
	{
		public string AllowFieldName { get; set; }

		public bool AllowHtml { get; set; }

		public int ArtifactTypeID { get; set; }

		public string ArtifactTypeTableName { get; set; }

		public int AssociativeArtifactTypeID { get; set; }

		public string AvfColumnName { get; set; }

		public string AvfHeaderName { get; set; }

		public int AvfId { get; set; }

		public Relativity.Import.Export.Services.FieldCategory Category { get; set; }

		public Relativity.Import.Export.Services.ColumnSourceType ColumnSource { get; set; }

		public int ConnectorFieldArtifactID { get; set; }

		public Relativity.Import.Export.Services.FieldCategory ConnectorFieldCategory { get; set; }

		public string ConnectorFieldName { get; set; }

		public string DataSource { get; set; }

		public string DisplayName { get; set; }

		public bool EnableDataGrid { get; set; }

		public int FieldArtifactId { get; set; }

		public int FieldCodeTypeID { get; set; }

		public bool FieldIsArtifactBaseField { get; set; }

		public Relativity.Import.Export.Services.FieldTypeHelperFieldType FieldType { get; set; }

		public string FormatString { get; set; }

		public bool IsLinked { get; set; }

		public bool IsUnicodeEnabled { get; set; }

		public bool IsVirtualAssociativeArtifactType { get; set; }

		public int ParentFileFieldArtifactID { get; set; }

		public string ParentFileFieldDisplayName { get; set; }

		public Relativity.Import.Export.Services.ParentReflectionType ParentReflectionType { get; set; }

		public string ReflectedConnectorIdentifierColumnName { get; set; }

		public string ReflectedFieldArtifactTypeTableName { get; set; }

		public string ReflectedFieldConnectorFieldName { get; set; }

		public string ReflectedFieldIdentifierColumnName { get; set; }

		public string RelationalTableColumnName { get; set; }

		public string RelationalTableColumnName2 { get; set; }

		public string RelationalTableName { get; set; }

		public int SourceFieldArtifactID { get; set; }

		public int SourceFieldArtifactTypeID { get; set; }

		public string SourceFieldArtifactTypeTableName { get; set; }

		public string SourceFieldName { get; set; }
	}
}