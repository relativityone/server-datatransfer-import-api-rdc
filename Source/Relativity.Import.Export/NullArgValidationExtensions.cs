// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullArgValidationExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Extension to check null pointer arguments on the function call
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Extension method to check method argument reference is not null.
	/// </summary>
	public static class NullArgValidationExtensions
	{
		/// <summary>
		/// It throws ArgumentNullException when passed argument is null.
		/// </summary>
		/// <typeparam name="TObject">class.</typeparam>
		/// <param name="obj">object to be validated.</param>
		/// <param name="paramName">name of the parameter passed to the method.</param>
		/// <returns>obj instance when the reference is not null.</returns>
		public static TObject ThrowIfNull<TObject>(this TObject obj, string paramName)
			where TObject : class
		{
			if (obj == null)
			{
				throw new ArgumentNullException(paramName);
			}

			return obj;
		}
	}
}
