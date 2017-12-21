using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class EncodingFileSize
	{
		/// <summary>
		///     This calculation is not precise, but it will always return value bigger than real file size
		/// </summary>
		/// <param name="sizeInUnicode"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static long CalculateLongTextFileSize(long sizeInUnicode, Encoding encoding)
		{
			long maxCharactersEncoded = CalculateMaxCharactersCountInEncoding(Encoding.Unicode, sizeInUnicode);
			long maxBytesForCharacters = CalculateMaxBytesForCharactersCountInEncoding(encoding, maxCharactersEncoded);
			return maxBytesForCharacters;
		}

		/// <summary>
		///     Encoding doesn't contain GetMaxCharCount method for Int64 type
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		private static long CalculateMaxCharactersCountInEncoding(Encoding encoding, long bytes)
		{
			const int bytesSample = 1024;
			int maxCharactersForSample = encoding.GetMaxCharCount(bytesSample);
			long maxCharactersForBytes = (bytes / bytesSample + 1) * maxCharactersForSample;
			return maxCharactersForBytes;
		}


		/// <summary>
		///     Encoding doesn't contain GetMaxByteCount method for Int64 type
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="characters"></param>
		/// <returns></returns>
		private static long CalculateMaxBytesForCharactersCountInEncoding(Encoding encoding, long characters)
		{
			const int charactersSample = 1024;
			int maxBytesForSample = encoding.GetMaxByteCount(charactersSample);
			long maxBytesForCharacters = (characters / charactersSample + 1) * maxBytesForSample;
			return maxBytesForCharacters;
		}
	}
}