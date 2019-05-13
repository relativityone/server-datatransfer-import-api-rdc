// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelimitedMode.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents the supported file delimiter modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Data
{
	/// <summary>
	/// Represents the supported file delimiter modes.
	/// </summary>
	public enum DelimitedMode
	{
		/// <summary>
		/// The delimited line contains no bounds.
		/// </summary>
		Simple,

		/// <summary>
		/// The delimited line contains maximum cell and line lengths.
		/// </summary>
		Bounded,
	}
}