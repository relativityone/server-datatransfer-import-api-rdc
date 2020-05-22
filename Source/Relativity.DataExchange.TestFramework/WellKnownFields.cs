// ----------------------------------------------------------------------------
// <copyright file="WellKnownFields.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	/// <summary>
	/// Defines well-known field constants.
	/// </summary>
	public static class WellKnownFields
	{
		/// <summary>
		/// The artifact identifier field name.
		/// </summary>
		public const string ArtifactId = "artifact id";

		/// <summary>
		/// The bates number field name.
		/// </summary>
		public const string BatesNumber = "bates number";

		/// <summary>
		/// The control number field name.
		/// </summary>
		public const string ControlNumber = "Control Number";

		/// <summary>
		/// The control number field identifier.
		/// </summary>
		public const int ControlNumberId = 1003667;

		/// <summary>
		/// The extracted text field.
		/// </summary>
		public const string ExtractedText = "Extracted Text";

		/// <summary>
		/// The file location field name.
		/// </summary>
		public const string FileLocation = "file location";

		/// <summary>
		/// The file name field name.
		/// </summary>
		public const string FileName = "file name";

		/// <summary>
		/// The file path field.
		/// </summary>
		public const string FilePath = "file path";

		/// <summary>
		/// The folder name field.
		/// </summary>
		public const string FolderName = "folder name";

		/// <summary>
		/// The 'Has Images' field.
		/// </summary>
		public const string HasImages = "has images";

		/// <summary>
		/// The 'Has Native' field.
		/// </summary>
		public const string HasNative = "has native";

		/// <summary>
		/// The native file size field.
		/// </summary>
		public const string NativeFileSize = "NativeFileSize";

		/// <summary>
		/// The Outside In file identifier field.
		/// </summary>
		public const string OutsideInFileId = "OutsideInFileId";

		/// <summary>
		/// The Outside In file type name field.
		/// </summary>
		public const string OutsideInFileType = "OutsideInFileType";

		/// <summary>
		/// The relativity image count.
		/// </summary>
		public const string RelativityImageCount = "relativity image count";

		/// <summary>
		/// The confidential designation.
		/// </summary>
		public const string ConfidentialDesignation = "Confidential Designation";

		/// <summary>
		/// The privilege designation.
		/// </summary>
		public const string PrivilegeDesignation = "Privilege Designation";

		/// <summary>
		/// The domains (email to).
		/// </summary>
		public const string DomainsEmailTo = "Domains (Email To)";

		/// <summary>
		/// The domains (email from).
		/// </summary>
		public const string DomainsEmailFrom = "Domains (Email From)";

		/// <summary>
		/// The originating imaging document error.
		/// </summary>
		public const string OriginatingImagingDocumentError = "Originating Imaging Document Error";

		/// <summary>
		/// The document identifier.
		/// </summary>
		public const string DocumentIdentifier = "Document Identifier";

		/// <summary>
		/// Maximum number of errors.
		/// </summary>
		public const int MaximumErrorCount = 1000;

		/// <summary>
		/// Default name of an RDO identifier field.
		/// </summary>
		public const string RdoIdentifier = "Name";

		/// <summary>
		/// Text field used as key field when overlaying.
		/// </summary>
		public const string KeyFieldName = "Key Field";

		/// <summary>
		/// Simple text field.
		/// </summary>
		public const string TextFieldName = "Text Field";
	}
}