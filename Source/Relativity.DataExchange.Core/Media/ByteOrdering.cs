// ----------------------------------------------------------------------------
// <copyright file="ByteOrdering.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	/// <summary>
	/// Represents the well-known byte ordering values.
	/// </summary>
	internal enum ByteOrdering
	{
		/// <summary>
		/// The byte order is always from the least significant byte to the most significant byte and used by Intel.
		/// </summary>
		LittleEndian,

		/// <summary>
		/// The byte order is always from the most significant byte to the least significant byte and used by Motorola.
		/// </summary>
		BigEndian,
	}
}