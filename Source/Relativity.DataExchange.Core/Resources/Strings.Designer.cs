﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Relativity.DataExchange.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Relativity.DataExchange.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid boolean..
        /// </summary>
        internal static string BooleanImporterErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("BooleanImporterErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be completed because the error report data source is invalid..
        /// </summary>
        internal static string BuildErrorReportArgError {
            get {
                return ResourceManager.GetString("BuildErrorReportArgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error reading cell. Please check format..
        /// </summary>
        internal static string CellImporterErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("CellImporterErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Transfer API client identifier &apos;{0}&apos; does not exist. Check to make sure the value was entered correctly..
        /// </summary>
        internal static string ClientIdNotFoundExceptionMessage {
            get {
                return ResourceManager.GetString("ClientIdNotFoundExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing.
        /// </summary>
        internal static string ClientInitializing {
            get {
                return ResourceManager.GetString("ClientInitializing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error has occurred attempting to wait for the transfer job to complete..
        /// </summary>
        internal static string CompleteJobExceptionMessage {
            get {
                return ResourceManager.GetString("CompleteJobExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error has occurred processing file [{0}] (page {1} of {2}).
        /// </summary>
        internal static string ConvertToMultiPageTiffOrPdfError {
            get {
                return ResourceManager.GetString("ConvertToMultiPageTiffOrPdfError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid date..
        /// </summary>
        internal static string DateImporterErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("DateImporterErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid decimal..
        /// </summary>
        internal static string DecimalImporterErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("DecimalImporterErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This error is considered fatal and suggests either an authentication issue or a version incompatibility between the client and server..
        /// </summary>
        internal static string HttpBadRequestFatalMessage {
            get {
                return ResourceManager.GetString("HttpBadRequestFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; HTTP Web service could not be called because the supplied Import API credential type &apos;{1}&apos; doesn&apos;t contain an authorization header and is unsupported..
        /// </summary>
        internal static string HttpCredentialNotSupportedExceptionMessage {
            get {
                return ResourceManager.GetString("HttpCredentialNotSupportedExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; HTTP Web service &apos;{1}&apos; method failed with an HTTP {2} status code.
        ///
        ///Error: {3}
        ///
        ///Detail: {4}.
        /// </summary>
        internal static string HttpExceptionMessage {
            get {
                return ResourceManager.GetString("HttpExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This error is considered fatal and suggests the client is forbidden from making the HTTP Web service call and is likely a problem with expired credentials or authentication..
        /// </summary>
        internal static string HttpForbiddenFatalMessage {
            get {
                return ResourceManager.GetString("HttpForbiddenFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; HTTP &apos;{1}&apos; Web service method failed.
        ///
        ///Error: {2}
        ///
        ///Detail: {3}.
        /// </summary>
        internal static string HttpNoStatusExceptionMessage {
            get {
                return ResourceManager.GetString("HttpNoStatusExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This error is considered fatal and suggests the HTTP Web service endpoint is not found and a potential version incompatibility exists between the client and server..
        /// </summary>
        internal static string HttpNotFoundFatalMessage {
            get {
                return ResourceManager.GetString("HttpNotFoundFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; HTTP Web service &apos;{1}&apos; method failed because it exceeded the {2} second timeout.
        ///
        ///Error: {3}
        ///
        ///Detail: {4}.
        /// </summary>
        internal static string HttpTimeoutExceptionMessage {
            get {
                return ResourceManager.GetString("HttpTimeoutExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This error is considered fatal and suggests the client is unauthorized from making the HTTP Web service call and is likely a problem with expired credentials or authentication..
        /// </summary>
        internal static string HttpUnauthorizedFatalMessage {
            get {
                return ResourceManager.GetString("HttpUnauthorizedFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image conversion failed because a TIFF image codec doesn&apos;t exist within this Operating System..
        /// </summary>
        internal static string ImageConversionTiffCodecNotFoundMessage {
            get {
                return ResourceManager.GetString("ImageConversionTiffCodecNotFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image identification cannot be performed on &apos;{0}&apos; because the file cannot be found..
        /// </summary>
        internal static string ImageFileNotFoundError {
            get {
                return ResourceManager.GetString("ImageFileNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image file &apos;{0}&apos; is of type &apos;{1}&apos; and isn&apos;t a valid TIFF or JPEG..
        /// </summary>
        internal static string ImageFormatNotSupportedError {
            get {
                return ResourceManager.GetString("ImageFormatNotSupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image file &apos;{0}&apos; cannot be read due to possible file corruption..
        /// </summary>
        internal static string ImageReadError {
            get {
                return ResourceManager.GetString("ImageReadError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The image file &apos;{0}&apos; is zero bytes..
        /// </summary>
        internal static string ImageZeroBytesError {
            get {
                return ResourceManager.GetString("ImageZeroBytesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in line {0}, column &quot;{1}&quot;. {2}.
        /// </summary>
        internal static string ImporterExcelError {
            get {
                return ResourceManager.GetString("ImporterExcelError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in row {0}, column &quot;{1}&quot;. {2}.
        /// </summary>
        internal static string ImporterStandardError {
            get {
                return ResourceManager.GetString("ImporterStandardError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in row {0}, field &quot;{1}&quot;. {2}.
        /// </summary>
        internal static string ImporterStandardFieldError {
            get {
                return ResourceManager.GetString("ImporterStandardFieldError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; application can’t be used with this Relativity instance because the import/export web-services version is invalid. Contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string ImportExportWebApiVersionInvalidExceptionMessage {
            get {
                return ResourceManager.GetString("ImportExportWebApiVersionInvalidExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This application can’t be used with this Relativity instance because the import/export web-services version is invalid. Contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string ImportExportWebApiVersionInvalidNoAppNameExceptionMessage {
            get {
                return ResourceManager.GetString("ImportExportWebApiVersionInvalidNoAppNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; application can’t be used with this Relativity instance because the import/export web-services version isn&apos;t supported..
        /// </summary>
        internal static string ImportExportWebApiVersionNotSupportedExceptionMessage {
            get {
                return ResourceManager.GetString("ImportExportWebApiVersionNotSupportedExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This application can’t be used with this Relativity instance because the import/export web-services version isn&apos;t supported..
        /// </summary>
        internal static string ImportExportWebApiVersionNotSupportedNoAppNameExceptionMessage {
            get {
                return ResourceManager.GetString("ImportExportWebApiVersionNotSupportedNoAppNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file {0} cannot be imported because it contains illegal characters within the path..
        /// </summary>
        internal static string ImportInvalidPathCharactersExceptionMessage {
            get {
                return ResourceManager.GetString("ImportInvalidPathCharactersExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid integer..
        /// </summary>
        internal static string IntegerImporterErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("IntegerImporterErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import error - Retrying in {0} seconds. Error: {1}.
        /// </summary>
        internal static string IoReporterWarningMessageWithException {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import error - Retrying in {0} seconds - {1} tries left. Error: {2}.
        /// </summary>
        internal static string IoReporterWarningMessageWithExceptionAndRetryInfo {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithExceptionAndRetryInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import error - Retrying in {0} seconds. Error details are not available..
        /// </summary>
        internal static string IoReporterWarningMessageWithoutException {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import error - Retrying in {0} seconds - {1} tries left. Error details are not available..
        /// </summary>
        internal static string IoReporterWarningMessageWithoutExceptionAndRetryInfo {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithoutExceptionAndRetryInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of {0} must be non-negative..
        /// </summary>
        internal static string LineNumberOutOfRangeExceptionMessage {
            get {
                return ResourceManager.GetString("LineNumberOutOfRangeExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (no endpoint was provided).
        /// </summary>
        internal static string NoEndpointProvided {
            get {
                return ResourceManager.GetString("NoEndpointProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (no message was provided).
        /// </summary>
        internal static string NoMessageProvided {
            get {
                return ResourceManager.GetString("NoMessageProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because this instance has already been disposed..
        /// </summary>
        internal static string ObjectDisposedExceptionMessage {
            get {
                return ResourceManager.GetString("ObjectDisposedExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input length exceeds maximum set length of {0} for the associated object field {1}..
        /// </summary>
        internal static string ObjectNameImporterMaxLengthErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("ObjectNameImporterMaxLengthErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because the Outside In file identification library located at &apos;{0}&apos; failed to initialize and suggests a serious configuration or environmental issue..
        /// </summary>
        internal static string OutsideInConfigurationError {
            get {
                return ResourceManager.GetString("OutsideInConfigurationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source file &apos;{0}&apos; cannot be identified because an Outside In error &apos;{1}&apos; occurred..
        /// </summary>
        internal static string OutsideInFileIdError {
            get {
                return ResourceManager.GetString("OutsideInFileIdError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source file &apos;{0}&apos; cannot be identified because an expected error occurred..
        /// </summary>
        internal static string OutsideInFileIdUnexpectedError {
            get {
                return ResourceManager.GetString("OutsideInFileIdUnexpectedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source file &apos;{0}&apos; cannot be identified because it doesn&apos;t exist..
        /// </summary>
        internal static string OutsideInFileNotFoundError {
            get {
                return ResourceManager.GetString("OutsideInFileNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because the Outside In file identification library located at &apos;{0}&apos; isn&apos;t available and suggests a serious configuration or environmental issue..
        /// </summary>
        internal static string OutsideInNotAvailableError {
            get {
                return ResourceManager.GetString("OutsideInNotAvailableError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because re-initializing the delimited file reader requires a file stream..
        /// </summary>
        internal static string ReinitializeReaderNotFileStreamError {
            get {
                return ResourceManager.GetString("ReinitializeReaderNotFileStreamError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; application can’t be used with this Relativity instance because version {1} doesn&apos;t meet or exceed the minimum version {2}..
        /// </summary>
        internal static string RelativtyMinVersionExceptionMessage {
            get {
                return ResourceManager.GetString("RelativtyMinVersionExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; application can’t be used with this Relativity instance because the Relativity version is invalid. Contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string RelativtyMinVersionInvalidExceptionMessage {
            get {
                return ResourceManager.GetString("RelativtyMinVersionInvalidExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This application can’t be used with this Relativity instance because the Relativity version is invalid. Contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string RelativtyMinVersionInvalidNoAppNameExceptionMessage {
            get {
                return ResourceManager.GetString("RelativtyMinVersionInvalidNoAppNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This application can’t be used with this Relativity instance because version {0} doesn&apos;t meet or exceed the minimum version {1}..
        /// </summary>
        internal static string RelativtyMinVersionNoAppNameExceptionMessage {
            get {
                return ResourceManager.GetString("RelativtyMinVersionNoAppNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrying transfer job - attempt {0} of {1}..
        /// </summary>
        internal static string RetryJobMessage {
            get {
                return ResourceManager.GetString("RetryJobMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The input value from the {0} source field has a length of {1} character(s). This exceeds the limit for the {0} destination field, which is currently set to {2} character(s)..
        /// </summary>
        internal static string StringImporterMaxLengthExWithFieldErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("StringImporterMaxLengthExWithFieldErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input length exceeds maximum set length of {0} for the {1} field..
        /// </summary>
        internal static string StringImporterMaxLengthWithFieldErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("StringImporterMaxLengthWithFieldErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input length exceeds maximum set length of {0} for this VarChar field..
        /// </summary>
        internal static string StringImporterMaxLengthWithoutFieldErrorAdditionalInfo {
            get {
                return ResourceManager.GetString("StringImporterMaxLengthWithoutFieldErrorAdditionalInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The TIFF image file &apos;{0}&apos; is encoded with &apos;{1}&apos; but only &apos;{2}&apos; is supported..
        /// </summary>
        internal static string TiffEncodingNotSupportedError {
            get {
                return ResourceManager.GetString("TiffEncodingNotSupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The TIFF image file &apos;{0}&apos; is {1} bits. Only 1 bit TIFFs are supported..
        /// </summary>
        internal static string TiffImageNotOneBitError {
            get {
                return ResourceManager.GetString("TiffImageNotOneBitError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The TIFF image file &apos;{0}&apos; is an unsupported or malformed TIFF..
        /// </summary>
        internal static string TiffImageNotSupportedError {
            get {
                return ResourceManager.GetString("TiffImageNotSupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The TIFF image file &apos;{0}&apos; is an unsupported multi-page TIFF..
        /// </summary>
        internal static string TiffMultiPageNotSupportedError {
            get {
                return ResourceManager.GetString("TiffMultiPageNotSupportedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created transfer client &apos;{0}&apos; using a best-fit strategy..
        /// </summary>
        internal static string TransferClientChangedBestFitMessage {
            get {
                return ResourceManager.GetString("TransferClientChangedBestFitMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created transfer client &apos;{0}&apos; using a default strategy..
        /// </summary>
        internal static string TransferClientChangedDefaultMessage {
            get {
                return ResourceManager.GetString("TransferClientChangedDefaultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created transfer client &apos;{0}&apos; using a forced configuration setting strategy..
        /// </summary>
        internal static string TransferClientChangedForceConfigMessage {
            get {
                return ResourceManager.GetString("TransferClientChangedForceConfigMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created transfer client &apos;{0}&apos; using a fallback strategy due to excessive errors..
        /// </summary>
        internal static string TransferClientChangedHttpFallbackMessage {
            get {
                return ResourceManager.GetString("TransferClientChangedHttpFallbackMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transfer client changed - {0}.
        /// </summary>
        internal static string TransferClientChangedMessage {
            get {
                return ResourceManager.GetString("TransferClientChangedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error downloading file: {0}.
        /// </summary>
        internal static string TransferFileDownloadFatalMessage {
            get {
                return ResourceManager.GetString("TransferFileDownloadFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} download failed: {1} - Retrying in {2} seconds. {3} tries left..
        /// </summary>
        internal static string TransferFileDownloadWarningMessage {
            get {
                return ResourceManager.GetString("TransferFileDownloadWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} download failed: {1} - This job is configured to skip this file-level issue..
        /// </summary>
        internal static string TransferFileDownloadWarningNoRetryMessage {
            get {
                return ResourceManager.GetString("TransferFileDownloadWarningNoRetryMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error uploading file: {0}.
        /// </summary>
        internal static string TransferFileUploadFatalMessage {
            get {
                return ResourceManager.GetString("TransferFileUploadFatalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} upload failed: {1} - Retrying in {2} seconds. {3} tries left..
        /// </summary>
        internal static string TransferFileUploadWarningMessage {
            get {
                return ResourceManager.GetString("TransferFileUploadWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} upload failed: {1} - This job is configured to skip this file-level issue..
        /// </summary>
        internal static string TransferFileUploadWarningNoRetryMessage {
            get {
                return ResourceManager.GetString("TransferFileUploadWarningNoRetryMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File transfer job was canceled..
        /// </summary>
        internal static string TransferJobCanceledMessage {
            get {
                return ResourceManager.GetString("TransferJobCanceledMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} download job issue: {1} - Retrying in {2} seconds - {3} tries left..
        /// </summary>
        internal static string TransferJobDownloadWarningMessage {
            get {
                return ResourceManager.GetString("TransferJobDownloadWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} download job issue: {1} - This job-level issue cannot be retried..
        /// </summary>
        internal static string TransferJobDownloadWarningNoRetryMessage {
            get {
                return ResourceManager.GetString("TransferJobDownloadWarningNoRetryMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File transfer job exceeded the max number of retries..
        /// </summary>
        internal static string TransferJobEndedMaxRetryMessage {
            get {
                return ResourceManager.GetString("TransferJobEndedMaxRetryMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File transfer job ended..
        /// </summary>
        internal static string TransferJobEndedMessage {
            get {
                return ResourceManager.GetString("TransferJobEndedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A serious error occurred transferring the requested files..
        /// </summary>
        internal static string TransferJobExceptionMessage {
            get {
                return ResourceManager.GetString("TransferJobExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because the file transfer job hasn&apos;t been created..
        /// </summary>
        internal static string TransferJobNullExceptionMessage {
            get {
                return ResourceManager.GetString("TransferJobNullExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File transfer job started..
        /// </summary>
        internal static string TransferJobStartedMessage {
            get {
                return ResourceManager.GetString("TransferJobStartedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} upload job issue: {1} - Retrying in {2} seconds - {3} tries left..
        /// </summary>
        internal static string TransferJobUploadWarningMessage {
            get {
                return ResourceManager.GetString("TransferJobUploadWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} upload job issue: {1} - This job-level issue cannot be retried..
        /// </summary>
        internal static string TransferJobUploadWarningNoRetryMessage {
            get {
                return ResourceManager.GetString("TransferJobUploadWarningNoRetryMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; HTTP Web service &apos;{1}&apos; method failed with a web exception {2} status code.
        ///
        ///Error: {3}
        ///
        ///Detail: {4}.
        /// </summary>
        internal static string WebExceptionMessage {
            get {
                return ResourceManager.GetString("WebExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This error is considered fatal and suggests a server has not been properly setup with an SSL certificate or the SSL certificate is no longer valid..
        /// </summary>
        internal static string WebExceptionTrustFailureMessage {
            get {
                return ResourceManager.GetString("WebExceptionTrustFailureMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A fatal file transfer error occurred and an attempt has already been made to resolve the issue by switching to web mode. Check the logs for more details or contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string WebModeFallbackAlreadyWebModeFatalExceptionMessage {
            get {
                return ResourceManager.GetString("WebModeFallbackAlreadyWebModeFatalExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switching to web mode due to a serious error within the &apos;{0}&apos; transfer job..
        /// </summary>
        internal static string WebModeFallbackNoErrorWarningMessage {
            get {
                return ResourceManager.GetString("WebModeFallbackNoErrorWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A fatal file transfer error occurred due to insufficient permissions and cannot be resolved by switching to web mode. Error: {0}.
        /// </summary>
        internal static string WebModeFallbackPermissionsFatalExceptionMessage {
            get {
                return ResourceManager.GetString("WebModeFallbackPermissionsFatalExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switching to web mode due to a serious error within the &apos;{0}&apos; transfer job. Error: {1}.
        /// </summary>
        internal static string WebModeFallbackWarningMessage {
            get {
                return ResourceManager.GetString("WebModeFallbackWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The workspace artifact must be specified..
        /// </summary>
        internal static string WorkspaceExceptionMessage {
            get {
                return ResourceManager.GetString("WorkspaceExceptionMessage", resourceCulture);
            }
        }
    }
}
