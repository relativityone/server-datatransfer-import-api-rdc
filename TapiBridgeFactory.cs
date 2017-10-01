// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeFactory.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create <see cref="TransferClientBridge"/> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Represents a class to create <see cref="TapiBridge"/> instances.
    /// </summary>
    public static class TapiBridgeFactory
    {
        /// <summary>
        /// Creates a <see cref="TapiBridge"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="TapiBridge"/> instance.
        /// </returns>
        public static TapiBridge CreateUploadBridge(TapiBridgeParameters parameters, CancellationToken token)
        {
            var transferLog = GetTransferLog(parameters);
            return new TapiBridge(parameters, TransferDirection.Upload, transferLog, token);
        }

        /// <summary>
        /// Gets the transfer log instance.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters
        /// </param>
        /// <returns>
        /// The <see cref="ITransferLog"/> instance.
        /// </returns>
        private static ITransferLog GetTransferLog(TapiBridgeParameters parameters)
        {
            try
            {
#if DEBUG
                LogSettings.Instance.MinimumLogLevel = LoggingLevel.Debug;
#else
                LogSettings.Instance.MinimumLogLevel = LoggingLevel.Information;
#endif
                if (!parameters.LogEnabled)
                {
                    return new NullTransferLog();
                }

                var options = Relativity.Logging.Factory.LogFactory.GetOptionsFromAppDomain() ?? new LoggerOptions();
                if (string.IsNullOrEmpty(options.Application))
                {
                    options.Application = "597C83DB-41C5-497D-B58E-7490479AD27E";
                }

                if (string.IsNullOrEmpty(options.ConfigurationFileLocation))
                {
                    options.ConfigurationFileLocation = GetLoggerConfigFilePath(parameters.LogConfigFile);
                }

                if (string.IsNullOrEmpty(options.System))
                {
                    options.System = "Import";
                }

                if (string.IsNullOrEmpty(options.SubSystem))
                {
                    options.SubSystem = "TransferAPI";
                }

                return new RelativityTransferLog(Relativity.Logging.Factory.LogFactory.GetLogger(options), false);
            }
            catch (Exception e)
            {
                try
                {
                    Relativity.Logging.Tools.InternalLogger.WriteTokCuraEventLog(
                        "Failed to setup WinEDDS logging. Exception: " + e,
                        "WinEDDS");
                }
                catch (Exception)
                {
                    // Being overly cautious to ensure no fatal errors occur due to logging.
                }

                return new NullTransferLog();
            }
        }

        /// <summary>
        /// Gets the logger configuration file path.
        /// </summary>
        /// <param name="logConfigFile">
        /// The Relativity Logging configuration file.
        /// </param>
        /// <returns>
        /// The full path.
        /// </returns>
        private static string GetLoggerConfigFilePath(string logConfigFile)
        {
            try
            {
                var path = logConfigFile;
                if (string.IsNullOrEmpty(path) || (Path.IsPathRooted(path) && !File.Exists(path)))
                {
                    return string.Empty;
                }

                if (Path.IsPathRooted(path) && File.Exists(path))
                {
                    return path;
                }

                // Be careful - unit tests result can return null.
                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                var directory = Directory.GetParent(assembly.Location).FullName;
                var file = Path.Combine(directory, "LoggerConfig.xml");
                if (!File.Exists(file))
                {
                    file = Path.Combine(directory, "LogConfig.xml");
                }

                return file;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}