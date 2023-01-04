// <copyright file="IWebApiVsKeplerFactory.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System;
	using System.Net;

	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;

	/// <summary>
	/// Factory for <see cref="IWebApiVsKepler"/>.
	/// </summary>
	internal interface IWebApiVsKeplerFactory
	{
		/// <summary>
		/// Creates instance of <see cref="IWebApiVsKepler"/>.
		/// </summary>
		/// <param name="webApiUrl">WebAPI URL.</param>
		/// <param name="credentials">Credentials.</param>
		/// <param name="getCorrelationId">Function returning CorrelationId.</param>
		/// <returns><see cref="IWebApiVsKepler"/> instance.</returns>
		IWebApiVsKepler Create(Uri webApiUrl, NetworkCredential credentials, Func<string> getCorrelationId);
	}
}
