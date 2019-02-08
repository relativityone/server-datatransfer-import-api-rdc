// ----------------------------------------------------------------------------
// <copyright file="LongTextTapiBridgePoolTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Threading;

    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;

    using Moq;

    using global::NUnit.Framework;

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
			_factory = new Mock<ILongTextTapiBridgeFactory>();
			_bridge = new Mock<IDownloadTapiBridge>();

			_factory.Setup(x => x.Create(CancellationToken.None)).Returns(_bridge.Object);

			_instance = new LongTextTapiBridgePool(_factory.Object, new NullLogger());
		}

		[Test]
		public void ItShouldDisposeBridges()
		{
			// ACT
			_instance.Release(_bridge.Object);

			// ASSERT
			_bridge.Verify(x => x.Disconnect(), Times.Never);
			_bridge.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ItShouldCreateBridgeForEveryRequest()
		{
			// ACT
			_instance.Request(CancellationToken.None);
			_instance.Request(CancellationToken.None);

			// ASSERT
			_factory.Verify(x => x.Create(CancellationToken.None), Times.Exactly(2));
		}

		[Test]
		public void ItShouldDisposeBridge()
		{
			// ACT
			_instance.Request(CancellationToken.None);
			_instance.Dispose();

			// ASSERT
			_bridge.Verify(x => x.Dispose(), Times.Once);
		}
	}
}