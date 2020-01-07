// <copyright file="ServiceProxyFactory.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ObjectManagers
{
	using System;

	internal class ServiceProxyFactory
	{
		private readonly IntegrationTestParameters testParameters;

		public ServiceProxyFactory(IntegrationTestParameters testParameters)
		{
			this.testParameters = testParameters;
		}

		public T CreateServiceProxy<T>()
		    where T : class, IDisposable
		{
			return ServiceHelper.GetServiceProxy<T>(testParameters);
		}
	}
}