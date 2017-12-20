using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class BatchValidatorTests
	{
		private BatchValidator _instance;
		private IList<Mock<IBatchValidator>> _validatorMocks;

		[SetUp]
		public void SetUp()
		{
			_validatorMocks = new List<Mock<IBatchValidator>>
			{
				new Mock<IBatchValidator>(),
				new Mock<IBatchValidator>(),
				new Mock<IBatchValidator>()
			};
			_instance = new BatchValidator(_validatorMocks.Select(x => x.Object).ToList(), new NullLogger());
		}

		[Test]
		public void ItShouldRunAllValidators()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] predictions = new VolumePredictions[1];

			//ACT
			_instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None);

			//ASSERT
			_validatorMocks.ForEach(x => x.Verify(v => v.ValidateExportedBatch(artifacts, predictions, CancellationToken.None)));
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenValidating()
		{
			ObjectExportInfo[] artifacts = null;
			VolumePredictions[] predictions = null;

			_validatorMocks[1].Setup(x => x.ValidateExportedBatch(artifacts, predictions, CancellationToken.None)).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.ValidateExportedBatch(artifacts, predictions, CancellationToken.None));

			//ASSERT
			_validatorMocks[2].Verify(x => x.ValidateExportedBatch(artifacts, predictions, CancellationToken.None), Times.Never);
		}
	}
}