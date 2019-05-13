// -----------------------------------------------------------------------------------------------------
// <copyright file="FieldStub.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
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
			this.FieldType = fieldType;
		}

		public void SetFieldArtifactId(int fieldArtifactId)
		{
			this.FieldArtifactId = fieldArtifactId;
		}

		public void SetIsUnicodeEnabled(bool isUnicodeEnabled)
		{
			this.IsUnicodeEnabled = isUnicodeEnabled;
		}

		public void SetAvfColumnName(string avfColumnName)
		{
			this.AvfColumnName = avfColumnName;
		}
	}
}