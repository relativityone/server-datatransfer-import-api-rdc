﻿//------------------------------------------------------------------------------
// <auto-generated>
// </auto-generated>
//------------------------------------------------------------------------------
namespace Relativity.Import.Export.Services
{
	using System;

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

		public Relativity.Import.Export.Services.FieldType FieldType { get; set; }

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

		public bool IsCodeOrMulticodeField
		{
			get
			{
				return this.FieldType == FieldType.Code || this.FieldType == FieldType.MultiCode;
			}
		}

		public bool IsMultiValueField
		{
			get
			{
				return this.FieldType == FieldType.Objects || this.Category == FieldCategory.MultiReflected;
			}
		}

		public ViewFieldInfo(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException(nameof(row));
			}

			FieldArtifactId = System.Convert.ToInt32(row["FieldArtifactID"]);
			AvfId = System.Convert.ToInt32(row["AvfID"]);
			Category = (FieldCategory)row["FieldCategoryID"];
			DisplayName = row["DisplayName"].ToString();
			AvfColumnName = row["AvfColumnName"].ToString();
			AvfHeaderName = row["AvfHeaderName"].ToString();
			AllowFieldName = row["AllowFieldName"].ToString();
			ColumnSource = (ColumnSourceType)System.Enum.Parse(typeof(ColumnSourceType), row["ColumnSource"].ToString());
			DataSource = row["DataSource"].ToString();
			SourceFieldName = row["SourceFieldDisplayName"].ToString();
			SourceFieldArtifactTypeID = System.Convert.ToInt32(row["SourceFieldArtifactTypeID"]);
			ConnectorFieldArtifactID = System.Convert.ToInt32(row["ConnectorFieldArtifactID"]);
			SourceFieldArtifactTypeTableName = row["SourceFieldArtifactTypeTableName"].ToString();
			ConnectorFieldName = row["ConnectorFieldName"].ToString();
			FieldType = (FieldType)row["FieldTypeID"];
			ConnectorFieldCategory = (FieldCategory)row["ConnectorFieldCategoryID"];
			IsLinked = System.Convert.ToBoolean(row["IsLinked"]);
			FieldCodeTypeID = System.Convert.ToInt32(row["FieldCodeTypeID"]);
			ArtifactTypeID = System.Convert.ToInt32(row["ArtifactTypeID"]);
			ArtifactTypeTableName = row["ArtifactTypeTableName"].ToString();
			FieldIsArtifactBaseField = System.Convert.ToBoolean(row["FieldIsArtifactBaseField"]);
			FormatString = System.Convert.ToString(row["FormatString"]);
			IsUnicodeEnabled = System.Convert.ToBoolean(row["IsUnicodeEnabled"]);
			AllowHtml = System.Convert.ToBoolean(row["AllowHtml"]);
			ParentFileFieldArtifactID = System.Convert.ToInt32(row["ParentFileFieldArtifactID"]);
			ParentFileFieldDisplayName = System.Convert.ToString(row["ParentFileFieldDisplayName"]);
			AssociativeArtifactTypeID = System.Convert.ToInt32(row["AssociativeArtifactTypeID"]);
			RelationalTableName = System.Convert.ToString(row["RelationalTableName"]);
			RelationalTableColumnName = System.Convert.ToString(row["RelationalTableColumnName"]);
			RelationalTableColumnName2 = System.Convert.ToString(row["RelationalTableColumnName2"]);
			SourceFieldArtifactID = System.Convert.ToInt32(row["SourceFieldArtifactID"]);

			if (row.Table.Columns.Contains("ParentReflectionType"))
			{
				ParentReflectionType = (ParentReflectionType)row["ParentReflectionType"];
				ReflectedFieldArtifactTypeTableName = System.Convert.ToString(row["ReflectedFieldArtifactTypeTableName"]);
				ReflectedFieldIdentifierColumnName = System.Convert.ToString(row["ReflectedFieldArtifactTypeIdentifierColumnName"]);
				ReflectedFieldConnectorFieldName = System.Convert.ToString(row["ReflectedFieldArtifactTypeConnectorFieldName"]);
				ReflectedConnectorIdentifierColumnName = System.Convert.ToString(row["ReflectedConnectorArtifactTypeIdentifierColumnName"]);
			}
			else
			{
				ParentReflectionType = ParentReflectionType.Empty;
				ReflectedFieldArtifactTypeTableName = string.Empty;
				ReflectedFieldIdentifierColumnName = string.Empty;
				ReflectedFieldConnectorFieldName = string.Empty;
				ReflectedConnectorIdentifierColumnName = string.Empty;
			}

			EnableDataGrid = System.Convert.ToBoolean(row["EnableDataGrid"]);

			bool columnExists = row.Table.Columns.Contains("IsVirtualAssociativeArtifactType");
			if (columnExists)
				IsVirtualAssociativeArtifactType = System.Convert.ToBoolean(row["IsVirtualAssociativeArtifactType"]);
		}

		public ViewFieldInfo(ViewFieldInfo vfi)
		{
			this.CopyFromViewFieldInfo(vfi);
		}

		private void CopyFromViewFieldInfo(ViewFieldInfo vfi)
		{
			FieldArtifactId = vfi.FieldArtifactId;
			AvfId = vfi.AvfId;
			Category = vfi.Category;
			DisplayName = vfi.DisplayName;
			AvfColumnName = vfi.AvfColumnName;
			AvfHeaderName = vfi.AvfHeaderName;
			AllowFieldName = vfi.AllowFieldName;
			ColumnSource = vfi.ColumnSource;
			DataSource = vfi.DataSource;
			SourceFieldName = vfi.SourceFieldName;
			SourceFieldArtifactTypeID = vfi.SourceFieldArtifactTypeID;
			ConnectorFieldArtifactID = vfi.ConnectorFieldArtifactID;
			SourceFieldArtifactTypeTableName = vfi.SourceFieldArtifactTypeTableName;
			ConnectorFieldName = vfi.ConnectorFieldName;
			FieldType = vfi.FieldType;
			ConnectorFieldCategory = vfi.ConnectorFieldCategory;
			IsLinked = vfi.IsLinked;
			FieldCodeTypeID = vfi.FieldCodeTypeID;
			ArtifactTypeID = vfi.ArtifactTypeID;
			ArtifactTypeTableName = vfi.ArtifactTypeTableName;
			FieldIsArtifactBaseField = vfi.FieldIsArtifactBaseField;
			FormatString = vfi.FormatString;
			IsUnicodeEnabled = vfi.IsUnicodeEnabled;
			AllowHtml = vfi.AllowHtml;
			ParentFileFieldArtifactID = vfi.ParentFileFieldArtifactID;
			ParentFileFieldDisplayName = vfi.ParentFileFieldDisplayName;
			AssociativeArtifactTypeID = vfi.AssociativeArtifactTypeID;
			RelationalTableName = vfi.RelationalTableName;
			RelationalTableColumnName = vfi.RelationalTableColumnName;
			RelationalTableColumnName2 = vfi.RelationalTableColumnName2;
			SourceFieldArtifactID = vfi.SourceFieldArtifactID;

			// Reflected Object fields
			ParentReflectionType = vfi.ParentReflectionType;
			ReflectedFieldArtifactTypeTableName = vfi.ReflectedFieldArtifactTypeTableName;
			ReflectedFieldIdentifierColumnName = vfi.ReflectedFieldIdentifierColumnName;
			ReflectedFieldConnectorFieldName = vfi.ReflectedFieldConnectorFieldName;
			ReflectedConnectorIdentifierColumnName = vfi.ReflectedConnectorIdentifierColumnName;

			EnableDataGrid = vfi.EnableDataGrid;
			IsVirtualAssociativeArtifactType = vfi.IsVirtualAssociativeArtifactType;
		}
	}
}