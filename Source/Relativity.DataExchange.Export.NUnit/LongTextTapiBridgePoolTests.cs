﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextTapiBridgePoolTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextTapiBridgePoolTests
	{
		private LongTextTapiBridgePool _instance;

		private Mock<ILongTextTapiBridgeFactory> _factory;
		private Mock<IDownloadTapiBridge> _bridge;

		[SetUp]
		public void SetUp()
		{
			this._factory = new Mock<ILongTextTapiBridgeFactory>();
			this._bridge = new Mock<IDownloadTapiBridge>();

			this._factory.Setup(x => x.Create(CancellationToken.None)).Returns(this._bridge.Object);

			this._instance = new LongTextTapiBridgePool(this._factory.Object, new NullLogger());
		}

		[Test]
		public void ItShouldDisposeBridges()
		{
			// ACT
			this._instance.Release(this._bridge.Object);

			// ASSERT
			this._bridge.Verify(x => x.Disconnect(), Times.Never);
			this._bridge.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ItShouldCreateBridgeForEveryRequest()
		{
			// ACT
			this._instance.Request(CancellationToken.None);
			this._instance.Request(CancellationToken.None);

			// ASSERT
			this._factory.Verify(x => x.Create(CancellationToken.None), Times.Exactly(2));
		}

		[Test]
		public void ItShouldDisposeBridge()
		{
			// ACT
			this._instance.Request(CancellationToken.None);
			this._instance.Dispose();

			// ASSERT
			this._bridge.Verify(x => x.Dispose(), Times.Once);
		}
	}
}