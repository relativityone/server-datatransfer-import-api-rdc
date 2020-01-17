// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceNotification.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to provide notification updates when performing service-related operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	/// <summary>
	/// Represents an abstract object to provide notification updates when performing service-related operations.
	/// </summary>
	internal interface IServiceNotification
	{
		/// <summary>
		/// Provides an error notification message during a service-related operation.
		/// </summary>
		/// <param name="message">
		/// The notification message.
		/// </param>
		void NotifyError(string message);

		/// <summary>
		/// Provides a status notification message during service-related operation.
		/// </summary>
		/// <param name="message">
		/// The notification message.
		/// </param>
		void NotifyStatus(string message);

		/// <summary>
		/// Provides a warning notification message during service-related operation.
		/// </summary>
		/// <param name="message">
		/// The notification message.
		/// </param>
		void NotifyWarning(string message);
	}
}