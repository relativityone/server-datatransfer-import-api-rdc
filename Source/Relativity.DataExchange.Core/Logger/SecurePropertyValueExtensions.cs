// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurePropertyValueExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Extensions method for secure property value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	/// <summary>
	/// Extensions method for secure property value class.
	/// </summary>
	public static class SecurePropertyValueExtensions
	{
		/// <summary>
		/// Create secure object.
		/// </summary>
		/// <param name="value">Object to secure.</param>
		/// <typeparam name="T"> Property value type.</typeparam>
		/// <returns>Object decorated with SecurePropertyValue class.</returns>
		public static SecurePropertyValue<T> Secure<T>(this T value)
		{
			return new SecurePropertyValue<T>(value);
		}
	}
}
