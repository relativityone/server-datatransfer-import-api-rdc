// ----------------------------------------------------------------------------
// <copyright file="BatchCleanUpTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;

    using Moq;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class BatchCleanUpTests
	{
		private BatchCleanUp _instance;

		private IList<Mock<IClearable>> _repositoryMocks;

		[SetUp]
		public void SetUp()
		{
			_repositoryMocks = new List<Mock<IClearable>>
			{
				new Mock<IClearable>(),
				new Mock<IClearable>(),
				new Mock<IClearable>()
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