// -----------------------------------------------------------------------------------------------------
// <copyright file="FieldStub.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Data;

    using Relativity.DataExchange.Service;

	public class FieldStub : kCura.WinEDDS.ViewFieldInfo
	{
		public FieldStub(DataRow row)
			: base(row)
		{
		}

		public FieldStub(Relativity.DataExchange.Service.ViewFieldInfo vfi)
			: base(vfi)
		{
		}

		public void SetType(FieldType fieldType)
		{
			FieldType = fieldType;
		}

		public void SetFieldArtifactId(int fieldArtifactId)
		{
			FieldArtifactId = fieldArtifactId;
		}

		public void SetIsUnicodeEnabled(bool isUnicodeEnabled)
		{
			IsUnicodeEnabled = isUnicodeEnabled;
		}

		public void SetAvfColumnName(string avfColumnName)
		{
			AvfColumnName = avfColumnName;
		}
	}
}