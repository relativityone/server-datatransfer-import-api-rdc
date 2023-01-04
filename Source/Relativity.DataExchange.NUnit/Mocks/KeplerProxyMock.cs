// <copyright file="KeplerProxyMock.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Mocks
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using Polly;

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

		public Task<T> ExecuteAsyncWithoutRetries<T>(Func<IServiceProxyFactory, Task<T>> func)
		{
			throw new NotImplementedException();
		}

		public Task<T> ExecuteAsync<T>(Func<IServiceProxyFactory, Task<T>> func)
		{
			return this.ExecuteAsync(func, 360);
		}

		public Task<T> ExecuteAsync<T>(Func<IServiceProxyFactory, Task<T>> func, int waitTimeSeconds)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			return func(this.serviceProxyFactory);
		}

		public Task<T> ExecuteAsync<T>(Context context, CancellationToken cancellationToken, Func<Context, CancellationToken, IServiceProxyFactory, Task<T>> func)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteAsync(Func<IServiceProxyFactory, Task> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			return func(this.serviceProxyFactory);
		}

		public Task<string> ExecutePostAsync(string endpointAddress, string body)
		{
			return this.serviceProxyFactory.ExecutePostAsync(endpointAddress, body);
		}
	}
}
