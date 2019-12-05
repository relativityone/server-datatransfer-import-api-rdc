// ----------------------------------------------------------------------------
// <copyright file="DocumentWithObjectsImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class DocumentWithObjectsImportDto
	{
		public DocumentWithObjectsImportDto(string controlNumber, string originatingImagingDocumentError, string domainsEmailTo, string domainsEmailFrom)
		{
			this.ControlNumber = controlNumber;
			this.OriginatingImagingDocumentError = originatingImagingDocumentError;
			this.DomainsEmailTo = domainsEmailTo;
			this.DomainsEmailFrom = domainsEmailFrom;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName(WellKnownFields.OriginatingImagingDocumentError)]
		public string OriginatingImagingDocumentError { get; }

		[DisplayName(WellKnownFields.DomainsEmailTo)]
		public string DomainsEmailTo { get; }

		[DisplayName(WellKnownFields.DomainsEmailFrom)]
		public string DomainsEmailFrom { get; }
	}
}