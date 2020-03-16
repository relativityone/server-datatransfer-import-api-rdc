// // ----------------------------------------------------------------------------
// <copyright file="FileNameValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;
	using System.IO;

	[Serializable]
	public class FileNameValueSource : IFieldValueSourceWithPrefix
	{
		public FileNameValueSource(string fileExtension)
			: this("IAPI-test", fileExtension)
		{
		}

		public FileNameValueSource(string fileNamePrefix, string fileExtension)
		{
			this.FileNamePrefix = fileNamePrefix;
			this.FileExtension = fileExtension;
		}

		public string FileNamePrefix { get; }

		public string FileExtension { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			for (int i = 0; ; i++)
			{
				string fileName = $"{this.FileNamePrefix}_{i}";
				yield return Path.ChangeExtension(fileName, this.FileExtension);
			}
		}

		public IFieldValueSourceWithPrefix CreateFieldValueSourceWithPrefix(string prefix)
		{
			return new FileNameValueSource(prefix, this.FileExtension);
		}
	}
}