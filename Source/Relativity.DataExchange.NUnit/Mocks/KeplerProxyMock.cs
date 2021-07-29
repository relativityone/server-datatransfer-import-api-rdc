// <copyright file="KeplerProxyMock.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Mocks
{
	using System;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Service;

	internal class KeplerProxyMock : IKeplerProxy
	{
		private readonly IServiceProxyFactory serviceProxyFactory;

		public KeplerProxyMock(IServiceProxyFactory serviceProxyFactory)
		{
			this.serviceProxyFactory = serviceProxyFactory;
		}

		public void Dispose()
		{
		}

		public Task<T> ExecuteAsync<T>(Func<IServiceProxyFactory, Task<T>> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			return func(this.serviceProxyFactory);
		}

		public Task ExecuteAsync(Func<IServiceProxyFactory, Task> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			return func(this.serviceProxyFactory);
		}
	}
}
