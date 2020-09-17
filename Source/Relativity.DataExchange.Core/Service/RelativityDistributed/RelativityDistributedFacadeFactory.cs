// <copyright file="RelativityDistributedFacadeFactory.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using System;
	using System.Net;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	/// <summary>
	/// This type provides a method which returns an instance of <see cref="IRelativityDistributedFacade"/>.
	/// </summary>
	internal class RelativityDistributedFacadeFactory
	{
		private readonly IReLoginService reLoginService;

		private readonly ILog logger;

		private readonly IAppSettings settings;

		private readonly IFile fileHelper;

		private readonly Func<string> authenticationTokenProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityDistributedFacadeFactory"/> class.
		/// </summary>
		/// <param name="logger">logger.</param>
		/// <param name="settings">Application settings.</param>
		/// <param name="reLoginService">ReLogin service.</param>
		/// <param name="fileHelper">fileHelper.</param>
		/// <param name="authenticationTokenProvider">Returns authentication token for Relativity.Distributed.</param>
		public RelativityDistributedFacadeFactory(
			ILog logger,
			IAppSettings settings,
			IReLoginService reLoginService,
			IFile fileHelper,
			Func<string> authenticationTokenProvider)
		{
			this.reLoginService = reLoginService;
			this.logger = logger;
			this.settings = settings;
			this.fileHelper = fileHelper;
			this.authenticationTokenProvider = authenticationTokenProvider;
		}

		/// <summary>
		/// Creates an instance of <see cref="IRelativityDistributedFacade"/>.
		/// </summary>
		/// <param name="downloadHandlerUrl">Relativity.Distributed URL.</param>
		/// <param name="credentials">credentials.</param>
		/// <param name="cookieContainer">cookieContainer.</param>
		/// <returns>D.</returns>
		public IRelativityDistributedFacade Create(
			string downloadHandlerUrl,
			NetworkCredential credentials,
			CookieContainer cookieContainer)
		{
			IRelativityDistributedFacade facade = new RelativityDistributedFacade(
				this.logger,
				this.settings,
				this.fileHelper,
				downloadHandlerUrl,
				credentials,
				cookieContainer,
				this.authenticationTokenProvider);

			facade = new RelativityDistributedFacadeAuthenticationDecorator(
				this.logger,
				this.settings,
				this.reLoginService,
				facade);

			facade = new RelativityDistributedFacadeRetriesDecorator(
				this.logger,
				this.settings,
				facade);

			return facade;
		}
	}
}
