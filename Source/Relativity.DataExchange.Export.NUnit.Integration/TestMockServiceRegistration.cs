// -----------------------------------------------------------------------------------------------------
// <copyright file="TestMockServiceRegistration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to supply the integration tests with mocks.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;

	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	/// <summary>
	/// Represents a class object to supply the integration tests with mocks.
	/// </summary>
	public class TestMockServiceRegistration : IMockServiceRegistration
	{
		/// <summary>
		/// Gets or sets the mock Transfer API object service.
		/// </summary>
		/// <value>
		/// The <see cref="ITapiObjectService"/> instance.
		/// </value>
		public ITapiObjectService MockTapiObjectService
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mock file share settings service.
		/// </summary>
		/// <value>
		/// The <see cref="IFileShareSettingsService"/> instance.
		/// </value>
		public IFileShareSettingsService MockFileShareSettingsService
		{
			get;
			set;
		}

		public void Register(IWindsorContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			// Note: all mocks use IsDefault() to handle the replacement.
			if (this.MockTapiObjectService != null)
			{
				container.Register(
					Component.For<ITapiObjectService>().UsingFactoryMethod(k => this.MockTapiObjectService)
						.IsDefault());
			}

			if (this.MockFileShareSettingsService != null)
			{
				container.Register(
					Component.For<IFileShareSettingsService>()
						.UsingFactoryMethod(k => this.MockFileShareSettingsService).IsDefault());
			}
		}
	}
}