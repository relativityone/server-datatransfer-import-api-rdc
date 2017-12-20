using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class BatchCleanUpTests
	{
		private BatchCleanUp _instance;

		private IList<Mock<IRepository>> _repositoryMocks;

		[SetUp]
		public void SetUp()
		{
			_repositoryMocks = new List<Mock<IRepository>>
			{
				new Mock<IRepository>(),
				new Mock<IRepository>(),
				new Mock<IRepository>()
			};

			_instance = new BatchCleanUp(_repositoryMocks.Select(x => x.Object).ToList(), new NullLogger());
		}

		[Test]
		public void ItShouldExecuteCleanUpsIndependently()
		{
			_repositoryMocks[1].Setup(x => x.Clear()).Throws<Exception>();

			//ACT
			_instance.CleanUp();

			//ASSERT
			foreach (var repositoryMock in _repositoryMocks)
			{
				repositoryMock.Verify(x => x.Clear());
			}
		}
	}
}