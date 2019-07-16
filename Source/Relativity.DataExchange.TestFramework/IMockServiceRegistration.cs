// -----------------------------------------------------------------------------------------------------
// <copyright file="IMockServiceRegistration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the container used by the integration tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using Castle.Windsor;

	/// <summary>
	/// Represents an abstract object used to register mock objects with a Castle Windsor container.
	/// </summary>
	public interface IMockServiceRegistration
	{
		/// <summary>
		/// Register one or more mocks with the container.
		/// </summary>
		/// <param name="container">
		/// The container.
		/// </param>
		void Register(IWindsorContainer container);
	}
}