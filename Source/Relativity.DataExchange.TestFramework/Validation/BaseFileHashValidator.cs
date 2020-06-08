// ----------------------------------------------------------------------------
// <copyright file="BaseFileHashValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Threading.Tasks;

	public abstract class BaseFileHashValidator : IFileValidator
	{
		protected abstract IEnumerable<string> Files { get; }

		public async Task<bool> IsValidAsync(string actualFilePath)
		{
			HashSet<string> validHashes = new HashSet<string>(await Task.WhenAll(this.Files.Select(CalculateFileHashAsync)).ConfigureAwait(false), StringComparer.OrdinalIgnoreCase);
			var actualHash = await CalculateFileHashAsync(actualFilePath).ConfigureAwait(false);
			return validHashes.Contains(actualHash);
		}

		private static async Task<string> CalculateFileHashAsync(string path)
		{
			using (var md5 = MD5.Create())
			{
				using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
				{
					byte[] buff = new byte[file.Length];
					await file.ReadAsync(buff, 0, (int)file.Length).ConfigureAwait(false);
					byte[] hash = md5.ComputeHash(buff);
					return BitConverter.ToString(hash).Replace("-", string.Empty);
				}
			}
		}
	}
}