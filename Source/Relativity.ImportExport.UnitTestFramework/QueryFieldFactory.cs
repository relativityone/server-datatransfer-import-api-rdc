// ----------------------------------------------------------------------------
// <copyright file="QueryFieldFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.ImportExport.UnitTestFramework
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    public class QueryFieldFactory
    {
        private DataTable table;

        private DataRow GenerateRow()
        {
            if (table == null)
            {
                table = new DataTable("Test-Data") {  Locale = CultureInfo.InvariantCulture };
                table.Columns.Add("FieldArtifactID", typeof(int));
                table.Columns.Add("AvfId", typeof(int));
                table.Columns.Add("FieldCategoryID", typeof(int));
                table.Columns.Add("FieldTypeID", typeof(int));
                table.Columns.Add("ArtifactTypeID", typeof(int));
                table.Columns.Add("ArtifactTypeTableName", typeof(string));
                table.Columns.Add("FieldCodeTypeID", typeof(int));
                table.Columns.Add("FieldIsArtifactBaseField", typeof(bool));
                table.Columns.Add("DisplayName", typeof(string));
                table.Columns.Add("AvfColumnName", typeof(string));
                table.Columns.Add("AvfHeaderName", typeof(string));
                table.Columns.Add("FormatString", typeof(string));
                table.Columns.Add("AssociativeArtifactTypeID", typeof(int));
                table.Columns.Add("IsUnicodeEnabled", typeof(bool));
                table.Columns.Add("AllowHtml", typeof(bool));
                table.Columns.Add("AllowFieldName", typeof(string));
                table.Columns.Add("SourceFieldArtifactID", typeof(int));
                table.Columns.Add("SourceFieldArtifactTypeTableName", typeof(string));
                table.Columns.Add("SourceFieldDisplayName", typeof(string));
                table.Columns.Add("SourceFieldArtifactTypeID", typeof(int));
                table.Columns.Add("ConnectorFieldArtifactID", typeof(int));
                table.Columns.Add("ConnectorFieldName", typeof(string));
                table.Columns.Add("ConnectorFieldCategoryID", typeof(int));
                table.Columns.Add("ParentFileFieldArtifactID", typeof(int));
                table.Columns.Add("ParentFileFieldDisplayName", typeof(string));
                table.Columns.Add("IsLinked", typeof(bool));
                table.Columns.Add("ColumnSource", typeof(string));
                table.Columns.Add("DataSource", typeof(string));
                table.Columns.Add("RelationalTableName", typeof(string));
                table.Columns.Add("RelationalTableColumnName", typeof(string));
                table.Columns.Add("RelationalTableColumnName2", typeof(string));
                table.Columns.Add("ParentReflectionType", typeof(int));
                table.Columns.Add("ReflectedFieldArtifactTypeTableName", typeof(string));
                table.Columns.Add("ReflectedFieldArtifactTypeIdentifierColumnName", typeof(string));
                table.Columns.Add("ReflectedFieldArtifactTypeConnectorFieldName", typeof(string));
                table.Columns.Add("ReflectedConnectorArtifactTypeIdentifierColumnName", typeof(string));
                table.Columns.Add("EnableDataGrid", typeof(bool));
            }

            return table.NewRow();
        }

        public kCura.WinEDDS.ViewFieldInfo GenerateQueryField(
            int fieldArtifactID,
            int avfId,
            FieldCategory fieldCategory,
            FieldTypeHelper.FieldType fieldType,
            int artifactTypeId,
            string artifactTypeTableName,
            int fieldCodeTypeID,
            bool fieldIsArtifactBaseField,
            string displayName,
            string avfColumnName,
            string avfHeaderName,
            string formatString,
            int associativeArtifactTypeID,
            bool isUnicodeEnabled,
            bool allowHtml,
            string allowFieldName,
            int sourceFieldArtifactID,
            string sourceFieldArtifactTypeTableName,
            string sourceFieldDisplayName,
            int sourceFieldArtifactTypeID,
            int connectorFieldArtifactID,
            string connectorFieldName,
            int connectorFieldCategoryId,
            int parentFileFieldArtifactID,
            string parentFileFieldDisplayName,
            bool isLinked,
            string columnSource,
            string dataSource,
            string relationalTableName,
            string relationalTableColumnName,
            string relationalTableColumnName2,
            ParentReflectionType parentReflectionType,
            string reflectedFieldArtifactTypeTableName,
            string reflectedFieldIdentifierColumnName,
            string reflectedFieldConnectorFieldName,
            string reflectedConnectorIdentifierColumnName,
            bool enableDataGrid)
        {
            var row = GenerateRow();
            row["FieldArtifactId"] = fieldArtifactID;
            row["AvfId"] = avfId;
            row["FieldCategoryID"] = (int) fieldCategory;
            row["FieldTypeID"] = (int) fieldType;
            row["ArtifactTypeID"] = artifactTypeId;
            row["ArtifactTypeTableName"] = artifactTypeTableName;
            row["FieldCodeTypeID"] = fieldCodeTypeID;
            row["FieldIsArtifactBaseField"] = fieldIsArtifactBaseField;
            row["DisplayName"] = displayName;
            row["AvfColumnName"] = avfColumnName;
            row["AvfHeaderName"] = avfHeaderName;
            row["FormatString"] = formatString;
            row["AssociativeArtifactTypeID"] = associativeArtifactTypeID;
            row["IsUnicodeEnabled"] = isUnicodeEnabled;
            row["AllowHtml"] = allowHtml;
            row["AllowFieldName"] = allowFieldName;
            row["SourceFieldArtifactID"] = sourceFieldArtifactID;
            row["SourceFieldArtifactTypeTableName"] = sourceFieldArtifactTypeTableName;
            row["SourceFieldDisplayName"] = sourceFieldDisplayName;
            row["SourceFieldArtifactTypeID"] = sourceFieldArtifactTypeID;
            row["ConnectorFieldArtifactID"] = connectorFieldArtifactID;
            row["ConnectorFieldName"] = connectorFieldName;
            row["ConnectorFieldCategoryID"] = connectorFieldCategoryId;
            row["ParentFileFieldArtifactID"] = parentFileFieldArtifactID;
            row["ParentFileFieldDisplayName"] = parentFileFieldDisplayName;
            row["IsLinked"] = isLinked;
            row["ColumnSource"] = columnSource;
            row["DataSource"] = dataSource;
            row["RelationalTableName"] = relationalTableName;
            row["RelationalTableColumnName"] = relationalTableColumnName;
            row["RelationalTableColumnName2"] = relationalTableColumnName2;
            row["ParentReflectionType"] = (int) parentReflectionType;
            row["ReflectedFieldArtifactTypeTableName"] = reflectedFieldArtifactTypeTableName;
            row["ReflectedFieldArtifactTypeIdentifierColumnName"] = reflectedFieldIdentifierColumnName;
            row["ReflectedFieldArtifactTypeConnectorFieldName"] = reflectedFieldConnectorFieldName;
            row["ReflectedConnectorArtifactTypeIdentifierColumnName"] = reflectedConnectorIdentifierColumnName;
            row["EnableDataGrid"] = enableDataGrid;
            return new kCura.WinEDDS.ViewFieldInfo(row);
        }

        public kCura.WinEDDS.ViewFieldInfo GetIdentifierQueryField()
        {
            return this.GenerateQueryField(5, 1000186, FieldCategory.Identifier, FieldTypeHelper.FieldType.Varchar, 10,
                "Document", -1, false, "Identifier", "Identifier", "Identifier", "", -1, true, false, "", -1, "",
                "Identifier", -1, -1, "", -1, -1, "", true, "MainTable", "Document", "", "", "",
                ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetExtractedTextField()
        {
            return this.GenerateQueryField(6, 1000187, FieldCategory.FullText, FieldTypeHelper.FieldType.Text, 10,
                "Document", -1, false, "Extracted Text", "ExtractedText", "Extracted Text", "", -1, true, false, "", -1,
                "", "Extracted Text", -1, -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "",
                ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericLongTextField()
        {
            return this.GenerateQueryField(6, 1001440, FieldCategory.FullText, FieldTypeHelper.FieldType.Text, 10,
                "Document", -1,
                false, "Long Text", "LongText", "Long Text", "", -1, true, false, "", -1, "", "Long Text", -1, -1, "",
                -1, -1, "", false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "", "",
                false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetRenamedGenericLongTextField()
        {
            return this.GenerateQueryField(6, 1001440, FieldCategory.FullText, FieldTypeHelper.FieldType.Text, 10,
                "Document", -1, false, "OtherLong Text", "OtherLongText", "OtherLong Text", "", -1, true, false, "", -1,
                "", "OtherLong Text", -1, -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "",
                ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericBooleanField()
        {
            return this.GenerateQueryField(14497, 1000357, FieldCategory.Generic, FieldTypeHelper.FieldType.Boolean, 10,
                "Document", -1, false, "Bool", "Bool", "Bool", "", -1, false, false, "", -1, "", "Bool", -1, -1, "", -1,
                -1, "", false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericSingleCodeField()
        {
            return this.GenerateQueryField(391085, 1001118, FieldCategory.Generic, FieldTypeHelper.FieldType.Code, 10,
                "Document", 1000097, false, "Single Choice", "SingleChoice", "Single Choice", "", -1, true, false, "",
                -1, "", "Single Choice", -1, -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "",
                ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericMultiCodeField()
        {
            return this.GenerateQueryField(391081, 1001117, FieldCategory.Generic, FieldTypeHelper.FieldType.MultiCode, 10,
                "Document", 1000096, false, "MCode", "MCode", "MCode", "", -1, false, false, "", -1, "", "MCode", -1,
                -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "",
                "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetArtifactIdField()
        {
            return this.GenerateQueryField(13090, 1000248, FieldCategory.Generic, FieldTypeHelper.FieldType.Integer, 10,
                "Document", -1, true, "Artifact ID", "ArtifactID", "ArtifactID", "", -1, false, false, "", -1, "",
                "ArtifactID", -1, -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "",
                ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericNumericField(FieldTypeHelper.FieldType type)
        {
            return this.GenerateQueryField(13090, 1000248, FieldCategory.Generic, type, 10, "Document", -1, false, "Decimal",
                "Decimal", "Decimal", "", -1, false, false, "", -1, "", "Decimal", -1, -1, "", -1, -1, "", false,
                "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericUserField()
        {
            return this.GenerateQueryField(14490, 1000353, FieldCategory.Generic, FieldTypeHelper.FieldType.User, 10,
                "Document",
                -1, false, "User Field", "UserField", "User Field", "", -1, false, false, "", -1, "", "User Field", -1,
                -1, "", -1, -1, "", false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "",
                "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericObjectField()
        {
            return this.GenerateQueryField(504869, 1001267, FieldCategory.Generic, FieldTypeHelper.FieldType.Object, 10,
                "Document",
                -1, false, "WSP", "WSP", "WSP", "", 1000047, false, false, "", -1, "WithSpace", "Name", -1, -1, "", -1,
                -1, "", false, "MainTable", "o1000047_f504869", "", "", "", ParentReflectionType.Empty, "", "", "", "",
                false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericObjectsField()
        {
            return this.GenerateQueryField(505158, 1001278, FieldCategory.Generic, FieldTypeHelper.FieldType.Objects, 10,
                "Document", -1, false, "DcMon", "DcMon", "DcMon", "", 1000045, false, false, "", -1, "Cmon", "Name", -1,
                -1, "Name", -1, -1, "", false, "MainTable", "Document", "f505158f505168", "f505158ArtifactID",
                "f505168ArtifactID", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GenerateMultiReflectedUserField()
        {
            return this.GenerateQueryField(505162, 1001282, FieldCategory.MultiReflected, FieldTypeHelper.FieldType.User, 10,
                "Document", -1, false, "DcMon::User", "DcMonUser", "DcMon::User", "", 1000045, false, false, "", 504867,
                "Cmon", "User", 1000045, 505158, "Name", 0, -1, "", false, "MainTable", "Cmon", "f505158f505168",
                "f505158ArtifactID", "f505168ArtifactID", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericDateField()
        {
            return this.GenerateQueryField(504869, 1001267, FieldCategory.Generic, FieldTypeHelper.FieldType.Date, 10,
                "Document",
                -1, false, "Date", "Date", "Date", "d", -1, false, false, "", -1, "", "Date", -1, -1, "", -1, -1, "",
                false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetGenericDateTimeField()
        {
            return this.GenerateQueryField(504869, 1001267, FieldCategory.Generic, FieldTypeHelper.FieldType.Date, 10,
                "Document",
                -1, false, "Date", "Date", "Date", "g", -1, false, false, "", -1, "", "Date", -1, -1, "", -1, -1, "",
                false, "MainTable", "Document", "", "", "", ParentReflectionType.Empty, "", "", "", "", false);
        }

        public kCura.WinEDDS.ViewFieldInfo GetDocumentFolderNameField()
        {
            return this.GenerateQueryField(15, 1000196, FieldCategory.FolderName, FieldTypeHelper.FieldType.Varchar, 10,
                "Document", -1, false, "Folder Name", "FolderName", "Folder Name", "", -1, false, false, "", -1, "",
                "Name", -1, -1, "", -1, -1, "", false, "Artifact", "Folder", "", "", "", ParentReflectionType.Empty, "",
                "", "", "", false);
        }

        public IEnumerable<kCura.WinEDDS.ViewFieldInfo> GetAllDocumentFields()
        {
            return new List<kCura.WinEDDS.ViewFieldInfo>
            {
                this.GetIdentifierQueryField(),
                this.GetExtractedTextField(),
                this.GetGenericBooleanField(),
                this.GetGenericSingleCodeField(),
                this.GetGenericMultiCodeField(),
                this.GetArtifactIdField(),
                this.GetGenericNumericField(FieldTypeHelper.FieldType.Currency),
                this.GetGenericNumericField(FieldTypeHelper.FieldType.Decimal),
                this.GetGenericNumericField(FieldTypeHelper.FieldType.Integer),
                this.GetGenericUserField(),
                this.GetGenericObjectField(),
                this.GetGenericObjectsField(),
                this.GenerateMultiReflectedUserField(),
                this.GetGenericDateField(),
                this.GetGenericDateTimeField(),
                this.GetDocumentFolderNameField()
            };
        }
    }
}