﻿using System.Data;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.DataSize
{
	public class FieldStub : ViewFieldInfo
	{
		public FieldStub(DataRow row) : base(row)
		{
		}

		public FieldStub(Relativity.ViewFieldInfo vfi) : base(vfi)
		{
		}

		public void SetType(FieldTypeHelper.FieldType fieldType)
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