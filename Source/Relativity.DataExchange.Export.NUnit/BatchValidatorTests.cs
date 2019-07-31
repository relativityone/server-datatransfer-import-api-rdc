// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.Logging;

	[TestFixture]
	public class BatchValidatorTests
	{
		private BatchValidator _instance;
		private List<Mock<IBatchValidator>> _validatorMocks;

		[SetUp]
		public void SetUp()
		{
			this._validatorMocks = new List<Mock<IBatchValidator>>
			{
				new Mock<IBatchValidator>(),
				new Mock<IBatchValidator>(),
				new Mock<IBatchValidator>()
			};
			this._instance = new BatchValidator(this._validatorMocks.Select(x => x.Object).ToList(), new NullLogger());
		}

		[Test]
		public void ItShouldRunAllValidators()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this._validatorMocks.ForEach(x => x.Verify(v => v.ValidateExportedBatch(artifacts, CancellationToken.None)));
		}

		[Test]
		public void ItShouldNotFailSilentlyWhenValidating()
		{
			ObjectExportInfo[] artifacts = null;

			this._validatorMocks[1].Setup(x => x.ValidateExportedBatch(artifacts, CancellationToken.None)).Throws<Exception>();

			// ACT & ASSERT
			Assert.Throws<Exception>(() => this._instance.ValidateExportedBatch(artifacts, CancellationToken.None));

			// ASSERT
			this._validatorMocks[2].Verify(x => x.ValidateExportedBatch(artifacts, CancellationToken.None), Times.Never);
		}
	}
}