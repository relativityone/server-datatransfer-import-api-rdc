﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="ViewFieldInfoMockFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using Relativity.DataExchange.Service;

	internal class ViewFieldInfoMockFactory
	{
		private DataRow _dataRow;

		/// <summary>
		/// It creates ViewFiledInfo array
		/// </summary>
		/// <param name="fieldsInfo">
		/// it hold info about AvfId and display name of teh field.
		/// </param>
		/// <returns>
		/// The <see cref="kCura.WinEDDS.ViewFieldInfo"/> instances.
		/// </returns>
		public static kCura.WinEDDS.ViewFieldInfo[] CreateMockedViewFieldInfoArray(List<Tuple<int, string>> fieldsInfo)
		{
			var viewFieldInfo = new List<kCura.WinEDDS.ViewFieldInfo>();
			DataTable dataTable = CreateMock();

			foreach (var fieldInfo in fieldsInfo)
			{
				var row = dataTable.NewRow();
				row["AvfId"] = fieldInfo.Item1;
				row["DisplayName"] = fieldInfo.Item2;
				viewFieldInfo.Add(new kCura.WinEDDS.ViewFieldInfo(row));
			}

			return viewFieldInfo.ToArray();
		}

		public ViewFieldInfoMockFactory Build()
		{
			this._dataRow = CreateMock().NewRow();
			return this;
		}

		public ViewFieldInfoMockFactory WithAvfId(int id)
		{
			this._dataRow["AvfID"] = id;
			return this;
		}

		public ViewFieldInfoMockFactory WithDisplayName(string name)
		{
			this._dataRow["DisplayName"] = name;
			return this;
		}

		public ViewFieldInfoMockFactory WithAvfName(string avfName)
		{
			this._dataRow["AvfColumnName"] = avfName;
			return this;
		}

		public ViewFieldInfoMockFactory WithFieldType(FieldType fieldType)
		{
			this._dataRow["FieldTypeID"] = fieldType;
			return this;
		}

		public ViewFieldInfoMockFactory WithFormatString(string formatString)
		{
			this._dataRow["FormatString"] = formatString;
			return this;
		}

		public kCura.WinEDDS.ViewFieldInfo Create()
		{
			return new kCura.WinEDDS.ViewFieldInfo(this._dataRow);
		}

		private static DataTable CreateMock()
		{
			var dataTable = new DataTable();
			dataTable.Columns.Add("FieldArtifactID", typeof(int));
			dataTable.Columns["FieldArtifactID"].DefaultValue = 1;
			dataTable.Columns.Add("AvfID", typeof(int));
			dataTable.Columns["AvfID"].DefaultValue = 1;
			dataTable.Columns.Add("FieldCategoryID", typeof(int));
			dataTable.Columns["FieldCategoryID"].DefaultValue = FieldCategory.Identifier;
			dataTable.Columns.Add("ColumnSource", typeof(string));
			dataTable.Columns["ColumnSource"].DefaultValue = "Computed";
			dataTable.Columns.Add("SourceFieldArtifactTypeID", typeof(int));
			dataTable.Columns["SourceFieldArtifactTypeID"].DefaultValue = 1;
			dataTable.Columns.Add("ConnectorFieldArtifactID", typeof(int));
			dataTable.Columns["ConnectorFieldArtifactID"].DefaultValue = 1;
			dataTable.Columns.Add("FieldTypeID", typeof(int));
			dataTable.Columns["FieldTypeID"].DefaultValue = FieldType.Empty;
			dataTable.Columns.Add("ConnectorFieldCategoryID", typeof(int));
			dataTable.Columns["ConnectorFieldCategoryID"].DefaultValue = 1;
			dataTable.Columns.Add("IsLinked", typeof(bool));
			dataTable.Columns["IsLinked"].DefaultValue = false;
			dataTable.Columns.Add("FieldCodeTypeID", typeof(int));
			dataTable.Columns["FieldCodeTypeID"].DefaultValue = 1;
			dataTable.Columns.Add("ArtifactTypeID", typeof(int));
			dataTable.Columns["ArtifactTypeID"].DefaultValue = 1;
			dataTable.Columns.Add("FieldIsArtifactBaseField", typeof(bool));
			dataTable.Columns["FieldIsArtifactBaseField"].DefaultValue = false;
			dataTable.Columns.Add("IsUnicodeEnabled", typeof(bool));
			dataTable.Columns["IsUnicodeEnabled"].DefaultValue = false;
			dataTable.Columns.Add("ParentFileFieldArtifactID", typeof(int));
			dataTable.Columns["ParentFileFieldArtifactID"].DefaultValue = 1;
			dataTable.Columns.Add("ParentFileFieldDisplayName", typeof(string));
			dataTable.Columns["ParentFileFieldDisplayName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("AssociativeArtifactTypeID", typeof(int));
			dataTable.Columns["AssociativeArtifactTypeID"].DefaultValue = 1;
			dataTable.Columns.Add("RelationalTableName", typeof(string));
			dataTable.Columns["RelationalTableName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("RelationalTableColumnName", typeof(string));
			dataTable.Columns["RelationalTableColumnName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("RelationalTableColumnName2", typeof(string));
			dataTable.Columns["RelationalTableColumnName2"].DefaultValue = string.Empty;
			dataTable.Columns.Add("SourceFieldArtifactID", typeof(int));
			dataTable.Columns["SourceFieldArtifactID"].DefaultValue = 1;
			dataTable.Columns.Add("EnableDataGrid", typeof(bool));
			dataTable.Columns["EnableDataGrid"].DefaultValue = true;
			dataTable.Columns.Add("DisplayName", typeof(string));
			dataTable.Columns["DisplayName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("AvfColumnName", typeof(string));
			dataTable.Columns["AvfColumnName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("AvfHeaderName", typeof(string));
			dataTable.Columns["AvfHeaderName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("AllowFieldName", typeof(string));
			dataTable.Columns["AllowFieldName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("SourceFieldArtifactTypeTableName", typeof(string));
			dataTable.Columns["SourceFieldArtifactTypeTableName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("ConnectorFieldName", typeof(string));
			dataTable.Columns["ConnectorFieldName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("ArtifactTypeTableName", typeof(string));
			dataTable.Columns["ArtifactTypeTableName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("FormatString", typeof(string));
			dataTable.Columns["FormatString"].DefaultValue = string.Empty;
			dataTable.Columns.Add("DataSource", typeof(string));
			dataTable.Columns["DataSource"].DefaultValue = string.Empty;
			dataTable.Columns.Add("SourceFieldDisplayName", typeof(string));
			dataTable.Columns["SourceFieldDisplayName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("AllowHtml", typeof(bool));
			dataTable.Columns["AllowHtml"].DefaultValue = false;
			dataTable.Columns.Add("ParentReflectionType", typeof(int));
			dataTable.Columns["ParentReflectionType"].DefaultValue = -1;
			dataTable.Columns.Add("ReflectedFieldArtifactTypeTableName", typeof(string));
			dataTable.Columns["ReflectedFieldArtifactTypeTableName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("ReflectedFieldArtifactTypeIdentifierColumnName", typeof(string));
			dataTable.Columns["ReflectedFieldArtifactTypeIdentifierColumnName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("ReflectedFieldArtifactTypeConnectorFieldName", typeof(string));
			dataTable.Columns["ReflectedFieldArtifactTypeConnectorFieldName"].DefaultValue = string.Empty;
			dataTable.Columns.Add("ReflectedConnectorArtifactTypeIdentifierColumnName", typeof(string));
			dataTable.Columns["ReflectedConnectorArtifactTypeIdentifierColumnName"].DefaultValue = string.Empty;
			return dataTable;
		}
	}
}