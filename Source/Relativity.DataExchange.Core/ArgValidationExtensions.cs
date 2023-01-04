// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgValidationExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Extensions for method arguments validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange
{
	using System;

	/// <summary>
	/// Extensions for method arguments validation.
	/// </summary>
	internal static class ArgValidationExtensions
	{
		/// <summary>
		/// It throws ArgumentNullException when passed argument is null.
		/// </summary>
		/// <typeparam name="TObject">class.</typeparam>
		/// <param name="obj">object to be validated.</param>
		/// <param name="paramName">name of the parameter passed to the method.</param>
		/// <returns>obj instance when the reference is not null.</returns>
		public static TObject ThrowIfNull<TObject>([ValidatedNotNull]this TObject obj, string paramName)
			where TObject : class
		{
			if (obj == null)
			{
				throw new ArgumentNullException(paramName);
			}

			return obj;
		}

		/// <summary>
		/// It throws ArgumentException when passed argument of type string is null or empty.
		/// </summary>
		/// <param name="obj">object to be validated.</param>
		/// <param name="paramName">name of the parameter passed to the method.</param>
		/// <returns>obj instance when the reference is not null or empty.</returns>
		public static string ThrowIfNullOrEmpty([ValidatedNotNull]this string obj, string paramName)
		{
			if (string.IsNullOrEmpty(obj))
			{
				throw new ArgumentException("Parameter should not be null or empty", paramName);
			}

			return obj;
		}
	}
}
