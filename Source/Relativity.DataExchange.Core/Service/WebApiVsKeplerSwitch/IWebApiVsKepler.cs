// ----------------------------------------------------------------------------
// <copyright file="IWebApiVsKepler.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	/// <summary>
	/// Determines if Kepler service should be used instead of WebApi service.
	/// </summary>
	public interface IWebApiVsKepler
	{
		/// <summary>
		/// Determines if Kepler service should be used instead of WebApi service.
		/// </summary>
		/// <returns>Boolean flag indicating if Kepler service should be used.</returns>
		bool UseKepler();
	}
}