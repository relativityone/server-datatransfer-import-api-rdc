﻿//------------------------------------------------------------------------------
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
        ///   Looks up a localized string similar to Not Set.
        /// </summary>
        internal static string ClientNotSet {
            get {
                return ResourceManager.GetString("ClientNotSet", resourceCulture);
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
        ///   Looks up a localized string similar to Transferring {0}/{1} - {2:0.00}%.
        /// </summary>
        internal static string ProgressMessage {
            get {
                return ResourceManager.GetString("ProgressMessage", resourceCulture);
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
        ///   Looks up a localized string similar to {0} download failed: {1}..
        /// </summary>
        internal static string TransferFileDownloadIssueMessage {
            get {
                return ResourceManager.GetString("TransferFileDownloadIssueMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} upload failed: {1}..
        /// </summary>
        internal static string TransferFileUploadIssueMessage {
            get {
                return ResourceManager.GetString("TransferFileUploadIssueMessage", resourceCulture);
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
        ///   Looks up a localized string similar to The workspace artifact must be specified..
        /// </summary>
        internal static string WorkspaceExceptionMessage {
            get {
                return ResourceManager.GetString("WorkspaceExceptionMessage", resourceCulture);
            }
        }
    }
}
