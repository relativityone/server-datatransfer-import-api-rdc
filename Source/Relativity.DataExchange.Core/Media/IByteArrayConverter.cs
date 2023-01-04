// // ----------------------------------------------------------------------------
// <copyright file="IByteArrayConverter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	/// <summary>
	/// Represents an abstract object to handle byte array conversion.
	/// </summary>
	internal interface IByteArrayConverter
	{
		/// <summary>
		/// Converts byte array to long value.
		/// </summary>
		/// <param name="byteArray">Byte array to convert.</param>
		/// <param name="startIndex">The starting position.</param>
		/// <param name="length">Number of bytes to convert.</param>
		/// <param name="byteOrdering">Order of bytes in byte array.</param>
		/// <returns>Converted value as long.</returns>
		long ToInt64(byte[] byteArray, int startIndex, int length, ByteOrdering byteOrdering);
	}
}