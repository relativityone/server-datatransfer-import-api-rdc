// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchCleanUpTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class BatchCleanUpTests
	{
		private BatchCleanUp _instance;

		private IList<Mock<IClearable>> _repositoryMocks;

		[SetUp]
		public void SetUp()
		{
			this._repositoryMocks = new List<Mock<IClearable>>
			{
				new Mock<IClearable>(),
				new Mock<IClearable>(),
				new Mock<IClearable>()
			};

			this._instance = new BatchCleanUp(this._repositoryMocks.Select(x => x.Object).ToList(), new TestNullLogger());
		}

		[Test]
		public void ItShouldExecuteCleanUpsIndependently()
		{
			this._repositoryMocks[1].Setup(x => x.Clear()).Throws<Exception>();

			// ACT
			this._instance.CleanUp();

			// ASSERT
			foreach (var repositoryMock in this._repositoryMocks)
			{
				repositoryMock.Verify(x => x.Clear());
			}
		}
	}
}