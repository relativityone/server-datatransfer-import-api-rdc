// -----------------------------------------------------------------------------------------------------
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
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LongTextTapiBridgePoolTests
	{
		private LongTextTapiBridgePool _instance;

		private Mock<ILongTextDownloadTapiBridgeFactory> _factory;
		private Mock<IDownloadTapiBridge> _bridge;

		[SetUp]
		public void SetUp()
		{
			this._factory = new Mock<ILongTextDownloadTapiBridgeFactory>();
			this._bridge = new Mock<IDownloadTapiBridge>();

			this._factory.Setup(x => x.Create(CancellationToken.None)).Returns(this._bridge.Object);

			this._instance = new LongTextTapiBridgePool(this._factory.Object, new TestNullLogger());
		}

		[Test]
		public void ItShouldDisposeBridges()
		{
			// ACT
			this._instance.Release(this._bridge.Object);

			// ASSERT
			this._bridge.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ItShouldNotThrowWhenTheBridgeIsNotRegistered()
		{
			// ARRANGE
			Mock<IDownloadTapiBridge> otherBridge = new Mock<IDownloadTapiBridge>();

			// ACT
			this._instance.Request(CancellationToken.None);
			this._instance.Release(otherBridge.Object);
			this._instance.Release(this._bridge.Object);

			// ASSERT
			this._bridge.Verify(x => x.Dispose(), Times.Once);
			otherBridge.Verify(x => x.Dispose(), Times.Once);
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