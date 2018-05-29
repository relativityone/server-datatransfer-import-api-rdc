//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("kCura.WinEDDS.TApi.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to A serious error has occurred attempting to transfer files over HTTP..
        /// </summary>
        internal static string HttpFallbackExceptionMessage {
            get {
                return ResourceManager.GetString("HttpFallbackExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switching to the HTTP client due to a serious error within the &apos;{0}&apos; transfer client. Error: {1}.
        /// </summary>
        internal static string HttpFallbackWarningMessage {
            get {
                return ResourceManager.GetString("HttpFallbackWarningMessage", resourceCulture);
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
        ///   Looks up a localized string similar to Error when accessing load file - Retrying in {0} seconds. Error: {1}.
        /// </summary>
        internal static string IoReporterWarningMessageWithException {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error when accessing load file - Retrying in {0} seconds - {1} tries left. Error: {2}.
        /// </summary>
        internal static string IoReporterWarningMessageWithExceptionAndRetryInfo {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithExceptionAndRetryInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error when accessing load file - Retrying in {0} seconds. Error details are not available..
        /// </summary>
        internal static string IoReporterWarningMessageWithoutException {
            get {
                return ResourceManager.GetString("IoReporterWarningMessageWithoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error when accessing load file - Retrying in {0} seconds - {1} tries left. Error details are not available..
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
        ///   Looks up a localized string similar to This opration cannot be performed because this instance has already been disposed..
        /// </summary>
        internal static string ObjectDisposedExceptionMessage {
            get {
                return ResourceManager.GetString("ObjectDisposedExceptionMessage", resourceCulture);
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
        ///   Looks up a localized string similar to The workspace artifact must be specified..
        /// </summary>
        internal static string WorkspaceExceptionMessage {
            get {
                return ResourceManager.GetString("WorkspaceExceptionMessage", resourceCulture);
            }
        }
    }
}