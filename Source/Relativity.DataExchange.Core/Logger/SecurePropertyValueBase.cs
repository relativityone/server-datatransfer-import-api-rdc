// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurePropertyValueBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Mark any property value as sensitive.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	/// <summary>
	/// Mark any property value as sensitive.
	/// </summary>
	public abstract class SecurePropertyValueBase
	{
		/// <summary>
		/// Call ToString method of marked property value.
		/// </summary>
		/// <returns>Return a string that represents marked property value.</returns>
		public abstract override string ToString();
	}
}
