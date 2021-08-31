// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportNegativeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerBulkImportNegativeTests : KeplerServiceTestBase
	{
		private const int DocumentObjectTypeId = (int)ArtifactType.Document;

		public KeplerBulkImportNegativeTests(bool useKepler)
			: base(useKepler)
		{
		}

		[IdentifiedTest("3C85B80F-C3B7-40B7-842E-319E312CE4AE")]
		public void ShouldThrowWhenInvalidWorkspaceForHasErrors()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.NativeRunHasErrors(NonExistingWorkspaceId, Guid.NewGuid().ToString()),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("AE7787BB-03F8-40BE-88F2-FBF80CC08A5F")]
		public void ShouldThrowWhenInvalidRunIdForHasErrors()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.NativeRunHasErrors(this.TestParameters.WorkspaceId, Guid.NewGuid().ToString()),
					this.GetExpectedException(
						"NativeRunHasErrorsAsync",
						"SQL Statement Failed",
						"kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));
			}
		}

		[IdentifiedTest("CD931BE8-8E60-4CAA-84A9-644FD3BC969A")]
		public void ShouldThrowWhenInvalidWorkspaceForDisposeTempTables()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.DisposeTempTables(NonExistingWorkspaceId, Guid.NewGuid().ToString()),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("C0B58951-3213-4AEA-930C-368627348428")]
		public void ShouldDisposeTempTablesForNonExistingRunId()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var result = sut.DisposeTempTables(this.TestParameters.WorkspaceId, Guid.NewGuid().ToString());

				Assert.That(result, Is.Null);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204", Justification = "RunId is used in excepton")]
		[IdentifiedTest("B799DE0E-025A-48DE-8C0D-36532E09F820")]
		public void ShouldThrowWhenInvalidRunIdForDisposeTempTables()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.DisposeTempTables(this.TestParameters.WorkspaceId, "FakeRunId"),
					this.GetExpectedException("DisposeTempTablesAsync", "Invalid RunId", typeof(Exception).ToString()));
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204", Justification = "RunId is used in excepton")]
		[IdentifiedTest("D0FA7CDE-209C-48DD-9341-65C17F6B366B")]
		public void ShouldThrowWhenInvalidRunIdForGenerateNonImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, "FakeRunId", DocumentObjectTypeId, false, WellKnownFields.ControlNumberId),
					this.GetExpectedException("GenerateNonImageErrorFilesAsync", "Invalid RunId", typeof(Exception).ToString()));
			}
		}

		[IdentifiedTest("014B7CE4-16F5-48FE-8207-2BA7AF6CF3DB")]
		public void ShouldThrowWhenInvalidWorkspaceIdForGenerateNonImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateNonImageErrorFiles(NonExistingWorkspaceId, Guid.NewGuid().ToString(), DocumentObjectTypeId, false, WellKnownFields.ControlNumberId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("C8C14472-260B-46D8-A481-21B9DCEFD5AF")]
		public void ShouldThrowWhenInvalidArtifactTypeIdForGenerateNonImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, Guid.NewGuid().ToString(), NonExistingArtifactTypeId, false, WellKnownFields.ControlNumberId),
					this.GetExpectedException("GenerateNonImageErrorFilesAsync", "SQL Statement Failed", "kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));
			}
		}

		[IdentifiedTest("A2CEE868-37EF-4D19-AB1E-A0A8D01C1716")]
		public void ShouldThrowWhenInvalidKeyFieldIdForGenerateNonImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, Guid.NewGuid().ToString(), DocumentObjectTypeId, false, NonExistingFieldId),
					this.GetExpectedException("GenerateNonImageErrorFilesAsync", "Object reference not set to an instance of an object.", typeof(NullReferenceException).ToString()));
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204", Justification = "runID is used in excepton")]
		[IdentifiedTest("B3790FF0-6D5E-448D-A9E6-CB55ECE98E88")]
		public void ShouldThrowWhenInvalidRunIdForGenerateImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateImageErrorFiles(this.TestParameters.WorkspaceId, "FakeRunId", false, WellKnownFields.ControlNumberId),
					this.GetExpectedException("GenerateImageErrorFilesAsync", "Invalid runID", typeof(Exception).ToString()));
			}
		}

		[IdentifiedTest("4F906D51-0163-4DB7-BCE1-9482A9C94CD0")]
		public void ShouldThrowWhenInvalidWorkspaceIdForGenerateImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateImageErrorFiles(NonExistingWorkspaceId, Guid.NewGuid().ToString(), false, WellKnownFields.ControlNumberId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("7811B11F-8FE9-43E2-B039-BB03766FC1FD")]
		public void ShouldThrowWhenInvalidKeyFieldIdForGenerateImageErrorFiles()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GenerateImageErrorFiles(this.TestParameters.WorkspaceId, Guid.NewGuid().ToString(), false, NonExistingFieldId),
					this.GetExpectedException("GenerateImageErrorFilesAsync", "SQL Statement Failed", "kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));
			}
		}

		private EqualConstraint GetExpectedException(string method, string message, string exceptionType)
		{
			if (this.UseKepler)
			{
				message =
					$"Error during call {method}. InnerExceptionType: {exceptionType}, InnerExceptionMessage: {message}";
			}

			return Throws.Exception.InstanceOf<SoapException>().With.Message.EqualTo(message);
		}
	}
}