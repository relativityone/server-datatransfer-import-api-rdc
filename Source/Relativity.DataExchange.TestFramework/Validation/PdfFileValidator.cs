// ----------------------------------------------------------------------------
// <copyright file="PdfFileValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	public class PdfFileValidator : IFileValidator
	{
		private readonly int numberOfDifferentBytesThreshold;

		public PdfFileValidator(int numberOfDifferentBytesThreshold)
		{
			this.numberOfDifferentBytesThreshold = numberOfDifferentBytesThreshold;
		}

		public async Task<bool> IsValidAsync(string actualFilePath)
		{
			var expectedFilePath = TestData.SamplePdfFiles.SingleOrDefault(a => a.IndexOf(Path.GetFileNameWithoutExtension(actualFilePath), StringComparison.OrdinalIgnoreCase) != -1);
			if (expectedFilePath != null)
			{
				byte[] expectedFile = await ReadAllBytesAsync(expectedFilePath).ConfigureAwait(false);
				byte[] actualFile = await ReadAllBytesAsync(actualFilePath).ConfigureAwait(false);

				return this.IsEqual(expectedFile, actualFile);
			}

			return false;
		}

		private static async Task<byte[]> ReadAllBytesAsync(string path)
		{
			using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
			{
				byte[] buff = new byte[file.Length];
				await file.ReadAsync(buff, 0, (int)file.Length).ConfigureAwait(false);
				return buff;
			}
		}

		private bool IsEqual(byte[] expectedFile, byte[] actualFile)
		{
			if (expectedFile.Length != actualFile.Length)
			{
				return false;
			}

			int numberOfDifferentBytes = expectedFile.Zip(actualFile, (first, second) => first == second).Count(x => false);
			return numberOfDifferentBytes <= this.numberOfDifferentBytesThreshold;
		}
	}
}