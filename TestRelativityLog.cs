// ----------------------------------------------------------------------------
// <copyright file="TestRelativityLog.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    using System;

    using global::Relativity.Logging;

    using Serilog;
    using Serilog.Context;
    using Serilog.Enrichers;

    /// <summary>
    /// Represents a thread-safe class object to write debug, information, warning, and error logs using Serilog.
    /// </summary>
    /// <remarks>
    /// This is an alternative implementation of Relativity Logging <see cref="ITransferLog"/> and can be used in client-side scenarios.
    /// </remarks>
    public class TestRelativityLog : ILog
    {
        /// <summary>
        /// The Serilog logger backing.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The flag that dictates whether the logger should be disposed.
        /// </summary>
        private readonly bool disposeLogger;

        /// <summary>
        /// The disposed backing.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRelativityLog"/> class.
        /// </summary>
        public TestRelativityLog()
        {
            this.logger = CreateLogger();
            this.disposeLogger = true;
            this.IsEnabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRelativityLog"/> class.
        /// </summary>
        /// <param name="logger">
        /// The existing logger instance.
        /// </param>
        public TestRelativityLog(ILogger logger)
        {
            this.logger = logger;
            this.disposeLogger = false;
            this.IsEnabled = true;
        }

        /// <inheritdoc />
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string Application
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string SubSystem
        {
            get;
            set;
        }

        /// <inheritdoc />
        public string System
        {
            get;
            set;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void LogInformation(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Information(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Information(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogDebug(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Debug(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Debug(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogWarning(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Warning(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Warning(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogError(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Error(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Error(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogVerbose(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Verbose(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Verbose(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogFatal(string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Fatal(messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.logger.Fatal(exception, messageTemplate, propertyValues);
        }

        /// <inheritdoc />
        public ILog ForContext<T>()
        {
            var newLogger = this.logger.ForContext<T>();
            return new TestRelativityLog(newLogger);
        }

        /// <inheritdoc />
        public ILog ForContext(Type forContext)
        {
            var newLogger = this.logger.ForContext(forContext);
            return new TestRelativityLog(newLogger);
        }

        /// <inheritdoc />
        public ILog ForContext(string propertyName, object value, bool destructureObjects)
        {
            var newLogger = this.logger.ForContext(propertyName, value, destructureObjects);
            return new TestRelativityLog(newLogger);
        }

        /// <inheritdoc />
        public IDisposable LogContextPushProperty(string propertyName, object obj)
        {
            return LogContext.PushProperty(propertyName, obj);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <returns>
        /// The <see cref="ILogger"/> instance.
        /// </returns>
        private static ILogger CreateLogger()
        {
            var configuration = new LoggerConfiguration().MinimumLevel.Debug()
                .Enrich.WithProperty("App", "Integration-Test")
                .Enrich.With(
                    new MachineNameEnricher(),
                    new ProcessIdEnricher(),
                    new ThreadIdEnricher())
                .MinimumLevel.Debug();
            configuration = configuration.WriteTo.Console();
            return configuration.CreateLogger();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing && this.disposeLogger)
            {
                var disposableLogger = this.logger as IDisposable;
                disposableLogger?.Dispose();
            }

            this.disposed = true;
        }
    }
}