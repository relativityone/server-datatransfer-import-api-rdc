// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchStateTests.cs" company="Relativity ODA LLC">
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
	using Relativity.Logging;

	[TestFixture]
	public class BatchStateTests
	{
		private BatchState _instance;
		private List<Mock<IStateful>> _statefulComponentMocks;

		[SetUp]
		public void SetUp()
		{
			this._statefulComponentMocks = new List<Mock<IStateful>>
			{
				new Mock<IStateful>(),
				new Mock<IStateful>(),
				new Mock<IStateful>()
			};
			this._instance = new BatchState(this._statefulComponentMocks.Select(x => x.Object).ToList(), new NullLogger());
		}

		[Test]
		public void ItShouldSaveStateForEveryStatefulComponent()
		{
			// ACT
			this._instance.SaveState();

			// ASSERT
			this._statefulComponentMocks.ForEach(x => x.Verify(sc => sc.SaveState()));
		}

		[Test]
		public void ItShouldRestoreStateInEveryStatefulComponent()
		{
			// ACT
			this._instance.RestoreState();

			// ASSERT
			this._statefulComponentMocks.ForEach(x => x.Verify(sc => sc.RestoreLastState()));
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenSavingState()
		{
			this._statefulComponentMocks[1].Setup(x => x.SaveState()).Throws<Exception>();

			// ACT & ASSERT
			Assert.Throws<Exception>(() => this._instance.SaveState());

			this._statefulComponentMocks[0].Verify(x => x.SaveState());
			this._statefulComponentMocks[2].Verify(x => x.SaveState(), Times.Never);
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenRestoringState()
		{
			this._statefulComponentMocks[1].Setup(x => x.RestoreLastState()).Throws<Exception>();

			// ACT & ASSERT
			Assert.Throws<Exception>(() => this._instance.RestoreState());

			this._statefulComponentMocks[0].Verify(x => x.RestoreLastState());
			this._statefulComponentMocks[2].Verify(x => x.RestoreLastState(), Times.Never);
		}
	}
}