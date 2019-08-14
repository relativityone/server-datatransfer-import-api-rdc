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
    internal class ExportStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExportStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Relativity.DataExchange.Resources.ExportStrings", typeof(ExportStrings).Assembly);
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
        ///   Looks up a localized string similar to The export request cannot be created for artifact {0} because the target path is null or empty..
        /// </summary>
        internal static string ExportRequestTargetPathExceptionMessage {
            get {
                return ResourceManager.GetString("ExportRequestTargetPathExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The export configuration settings must supply a valid credential..
        /// </summary>
        internal static string ExportSettingsNullCredentialExceptionMessage {
            get {
                return ResourceManager.GetString("ExportSettingsNullCredentialExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The export configuration settings must supply a valid workspace..
        /// </summary>
        internal static string ExportSettingsNullWorkspaceExceptionMessage {
            get {
                return ResourceManager.GetString("ExportSettingsNullWorkspaceExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrieved workspace file share details..
        /// </summary>
        internal static string FileStorageCompletedStatusMessage {
            get {
                return ResourceManager.GetString("FileStorageCompletedStatusMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A non-fatal error occurred attempting to retrieve file share details associated with workspace {0}. All exported artifacts can only be transferred by direct or web mode. Error: {1}.
        /// </summary>
        internal static string FileStorageExceptionWarningMessage {
            get {
                return ResourceManager.GetString("FileStorageExceptionWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The default file share {0} for workspace {1} contains invalid cloud transfer configuration info. Exported artifacts from this file share can only be transferred by direct or web mode. Error: &apos;{2}&apos;.
        /// </summary>
        internal static string FileStorageInvalidDefaultFileShareWarningMessage {
            get {
                return ResourceManager.GetString("FileStorageInvalidDefaultFileShareWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The non-default file share {0} associated with workspace {1} contains invalid cloud transfer configuration info. Exported artifacts from this file share can only be transferred by direct or web mode. Error: &apos;{2}&apos;.
        /// </summary>
        internal static string FileStorageInvalidNonDefaultFileShareWarningMessage {
            get {
                return ResourceManager.GetString("FileStorageInvalidNonDefaultFileShareWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrieving workspace file share details....
        /// </summary>
        internal static string FileStorageStartedStatusMessage {
            get {
                return ResourceManager.GetString("FileStorageStartedStatusMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All of the file shares associated with workspace {0} contain invalid cloud transfer configuration info. All exported artifacts can only be transferred by direct or web mode..
        /// </summary>
        internal static string FileStorageZeroValidFileSharesWarningMessage {
            get {
                return ResourceManager.GetString("FileStorageZeroValidFileSharesWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remote source path is empty and indicates a back-end database issue..
        /// </summary>
        internal static string FileValidationEmptyRemoteSourcePath {
            get {
                return ResourceManager.GetString("FileValidationEmptyRemoteSourcePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File missing..
        /// </summary>
        internal static string FileValidationFileMissing {
            get {
                return ResourceManager.GetString("FileValidationFileMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zero byte file..
        /// </summary>
        internal static string FileValidationZeroByteFile {
            get {
                return ResourceManager.GetString("FileValidationZeroByteFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to release all transfer pool resources..
        /// </summary>
        internal static string TransferPoolDisposeExceptionMessage {
            get {
                return ResourceManager.GetString("TransferPoolDisposeExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation cannot be performed because the workspace &apos;{0}&apos; default file share isn&apos;t defined. Retry or contact your system administrator for assistance if this problem persists..
        /// </summary>
        internal static string WorkspaceDefaultFileshareNullExceptionMessage {
            get {
                return ResourceManager.GetString("WorkspaceDefaultFileshareNullExceptionMessage", resourceCulture);
            }
        }
    }
}
