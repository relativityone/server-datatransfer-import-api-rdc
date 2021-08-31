// <copyright file="FieldsRandomHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;

	public static class FieldsRandomHelper
	{
		private static readonly Random Random = new Random();

		private static readonly Dictionary<FieldType, Func<FieldInfo, object>> FieldValueProvider =
			new Dictionary<FieldType, Func<FieldInfo, object>>
			{
				{ FieldType.Varchar, (fieldInfo) => NextString(10, fieldInfo.TextLength) },
				{ FieldType.Boolean, (_) => NextBoolean() ? "1" : "0" },
			};

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			category: "Microsoft.Reliability",
			checkId: "CA2000:Dispose objects before losing scope",
			Justification = "Dispozed in test")]
		public static DataTable GetFieldValues(IEnumerable<FieldInfo> fields, int numberOfArtifactsToCreate)
		{
			DataTable fieldValues = new DataTable() { Locale = CultureInfo.CurrentCulture };
			DataColumn[] columns = fields.Select(field => new DataColumn(field.DisplayName.Replace(" ", string.Empty))).ToArray();

			fieldValues.Columns.AddRange(columns);

			for (int i = 0; i < numberOfArtifactsToCreate; i++)
			{
				object[] values = fields.Select(field => FieldValueProvider[field.Type].Invoke(field)).ToArray();
				fieldValues.Rows.Add(values);
			}

			return fieldValues;
		}

		private static string NextString(int minLength, int maxLength)
		{
			const string availableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			int length = Random.Next(minLength, maxLength);

			return new string(Enumerable.Repeat(availableChars, length)
				.Select(s => s[Random.Next(s.Length)]).ToArray());
		}

		private static bool NextBoolean()
		{
			return Random.NextDouble() > 0.5;
		}
	}
}