// // ----------------------------------------------------------------------------
// <copyright file="ByteArrayConverter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	using System;

	/// <summary>
	/// Provides methods for byte array conversion.
	/// </summary>
	internal class ByteArrayConverter : IByteArrayConverter
	{
		/// <inheritdoc />
		public long ToInt64(byte[] byteArray, int startIndex, int length, ByteOrdering byteOrdering)
		{
			byteArray.ThrowIfNull(nameof(byteArray));

			if (length <= 0 || length >= 8)
			{
				throw new ArgumentOutOfRangeException(nameof(length));
			}

			if (startIndex < 0 || startIndex + length > byteArray.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(startIndex));
			}

			long value = 0;
			switch (byteOrdering)
			{
				case ByteOrdering.LittleEndian:
					for (int i = startIndex + length - 1; i >= startIndex; --i)
					{
						value = (value << 8) | byteArray[i];
					}

					break;

				case ByteOrdering.BigEndian:
					for (int i = startIndex; i < startIndex + length; ++i)
					{
						value = (value << 8) | byteArray[i];
					}

					break;
			}

			return value;
		}
	}
}