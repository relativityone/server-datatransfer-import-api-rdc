// -----------------------------------------------------------------------------------------------------
// <copyright file="IWixSession.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Desktop.Client.CustomActions
{
	/// <summary>
	/// Represents an abstract WIX session.
	/// </summary>
	public interface IWixSession
	{
		/// <summary>
		/// Get the string property value for the specified name.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property.
		/// </param>
		/// <returns>
		/// The string value.
		/// </returns>
		string GetStringPropertyValue(string propertyName);

		/// <summary>
		/// Get property value for the specified name.
		/// </summary>
		/// <typeparam name="T">
		/// The type of object to get.
		/// </typeparam>
		/// <param name="propertyName">
		/// Name of the property.
		/// </param>
		/// <returns>
		/// The property value.
		/// </returns>
		T GetPropertyValue<T>(string propertyName);

		/// <summary>
		/// Logs the specified message.
		/// </summary>
		/// <param name="msg">
		/// The message.
		/// </param>
		void Log(string msg);

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property.
		/// </param>
		/// <param name="value">
		/// The string value to set.
		/// </param>
		void SetPropertyValue(string propertyName, string value);
	}
}