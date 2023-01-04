// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurePropertyValue.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Mark any property value as sensitive.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	using System;

	/// <summary>
	/// Mark any property value as sensitive.
	/// </summary>
	/// <typeparam name="T"> Property value type.</typeparam>
	public class SecurePropertyValue<T> : SecurePropertyValueBase
	{
		private readonly object value;

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurePropertyValue{T}"/> class.
		/// </summary>
		/// <param name="value"> Object to secure.</param>
		public SecurePropertyValue(T value)
		{
			this.value = value;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.value?.ToString() ?? string.Empty;
		}
	}
}
