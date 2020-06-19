// <copyright file="StringContentValidator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;

	using NUnit.Framework;

	public class StringContentValidator : IFileValidator
	{
		private readonly string _expectedContent;
		private readonly string _extension;
		private readonly Encoding _encoding;

		public StringContentValidator(string expectedContent, string extension = null, Encoding encoding = null)
		{
			_extension = extension;
			_encoding = encoding;
			_expectedContent = expectedContent.Trim();
		}

		public async Task<bool> IsValidAsync(string actualFilePath)
		{
			if (this._extension != null)
			{
				ValidateFileExtension(actualFilePath, _extension);
			}

			if (this._encoding != null)
			{
				ValidateFileEncoding(actualFilePath, _encoding);
			}

			using (var reader = File.OpenText(actualFilePath))
			{
				var fileText = await reader.ReadToEndAsync().ConfigureAwait(false);
				return _expectedContent.Equals(fileText.Trim(), StringComparison.CurrentCultureIgnoreCase);
			}
		}

		private static void ValidateFileEncoding(string filePath, Encoding expectedEncoding)
		{
			Encoding currentEncoding = GetFileEncoding(filePath);
			Assert.AreEqual(expectedEncoding, currentEncoding, $"File '{filePath}' encoding is '{currentEncoding}', should be '{expectedEncoding}'");
		}

		private static void ValidateFileExtension(string filePath, string expectedExtension)
		{
			string currentExtension = GetFileExtension(filePath);
			Assert.AreEqual(expectedExtension, currentExtension, $"File '{filePath}' extension is '{currentExtension}', should be '{expectedExtension}'");
		}

		private static string GetFileExtension(string filePath)
		{
			return Path.GetExtension(filePath).Replace(".", string.Empty);
		}

		private static Encoding GetFileEncoding(string filePath)
		{
			using (StreamReader streamReader = new StreamReader(filePath, Encoding.Default, true))
			{
				streamReader.Peek();
				return streamReader.CurrentEncoding;
			}
		}
	}
}
