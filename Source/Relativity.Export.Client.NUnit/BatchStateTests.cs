﻿// ----------------------------------------------------------------------------
// <copyright file="BatchStateTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using kCura.Vendor.Castle.Core.Internal;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

    using Moq;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class BatchStateTests
	{
		private BatchState _instance;
		private IList<Mock<IStateful>> _statefulComponentMocks;

		[SetUp]
		public void SetUp()
		{
			_statefulComponentMocks = new List<Mock<IStateful>>
			{
				new Mock<IStateful>(),
				new Mock<IStateful>(),
				new Mock<IStateful>()
			};
			_instance = new BatchState(_statefulComponentMocks.Select(x => x.Object).ToList(), new NullLogger());
		}

		[Test]
		public void ItShouldSaveStateForEveryStatefulComponent()
		{
			//ACT
			_instance.SaveState();

			//ASSERT
			_statefulComponentMocks.ForEach(x => x.Verify(sc => sc.SaveState()));
		}

		[Test]
		public void ItShouldRestoreStateInEveryStatefulComponent()
		{
			//ACT
			_instance.RestoreState();

			//ASSERT
			_statefulComponentMocks.ForEach(x => x.Verify(sc => sc.RestoreLastState()));
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenSavingState()
		{
			_statefulComponentMocks[1].Setup(x => x.SaveState()).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.SaveState());

			_statefulComponentMocks[0].Verify(x => x.SaveState());
			_statefulComponentMocks[2].Verify(x => x.SaveState(), Times.Never);
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenRestoringState()
		{
			_statefulComponentMocks[1].Setup(x => x.RestoreLastState()).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.RestoreState());

			_statefulComponentMocks[0].Verify(x => x.RestoreLastState());
			_statefulComponentMocks[2].Verify(x => x.RestoreLastState(), Times.Never);
		}
	}
}