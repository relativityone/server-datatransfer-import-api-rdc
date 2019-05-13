// ----------------------------------------------------------------------------
// <copyright file="OutsideInFileTypeIdentifierService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;
	using System.Globalization;
	using System.Reflection;

	using OutsideIn;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Represents a file identification service class object using Outside In technology.
	/// </summary>
	internal class OutsideInFileTypeIdentifierService : IFileTypeIdentifier
	{
		/// <summary>
		/// The default idle timeout value in seconds.
		/// </summary>
		public const int DefaultIdleTimeout = 10;

		/// <summary>
		/// The file identification configuration.
		/// </summary>
		private readonly FileTypeConfiguration configuration = new FileTypeConfiguration();

		/// <summary>
		/// The optional timeout in seconds.
		/// </summary>
		private readonly int timeout;

		/// <summary>
		/// The OI exporter instance.
		/// </summary>
		private Exporter exporter;

		/// <summary>
		/// The disposed backing field.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="OutsideInFileTypeIdentifierService"/> class.
		/// </summary>
		public OutsideInFileTypeIdentifierService()
			: this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OutsideInFileTypeIdentifierService"/> class.
		/// </summary>
		/// <param name="timeout">
		/// The optional timeout in seconds.
		/// </param>
		public OutsideInFileTypeIdentifierService(int? timeout)
		{
			this.timeout = timeout ?? DefaultIdleTimeout;
			this.disposed = false;
			this.exporter = null;
		}

		/// <inheritdoc />
		public IFileTypeConfiguration Configuration
		{
			get
			{
				this.Initialize();
				return this.configuration;
			}
		}

		/// <summary>
		/// Gets the OI installation path.
		/// </summary>
		/// <returns>
		/// The full path.
		/// </returns>
		public static string GetInstallPath()
		{
			string assemblyPath = new System.Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assemblyPath), "oi");
			if (!string.IsNullOrEmpty(path))
			{
				path = path.Replace("%20", " ");
			}

			return path;
		}

		/// <summary>
		/// Shutdown the OI link monitor.
		/// </summary>
		public static void Shutdown()
		{
			OutsideIn.Shutdown();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public IFileTypeInfo Identify(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			this.Initialize();

			try
			{
				// OI fixed the lock file/file not found error code defect.
				System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
				FileFormat fileFormat = this.exporter.Identify(fileInfo);
				return new FileTypeInfo(fileFormat.GetId(), fileFormat.GetDescription());
			}
			catch (System.IO.FileNotFoundException)
			{
				throw;
			}
			catch (OutsideInException e) when (e.ErrorCode == OutsideInConstants.FileNotFoundErrorCode)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.OutsideInFileNotFoundError, file);
				throw new System.IO.FileNotFoundException(message, file);
			}
			catch (OutsideInException e)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.OutsideInFileIdError, file, e.ErrorCode);
				throw new FileTypeIdentifyException(message, e, GetFileIdError(e.ErrorCode));
			}
			catch (Exception e)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.OutsideInFileIdUnexpectedError, file);
				throw new FileTypeIdentifyException(message, e);
			}
		}

		/// <inheritdoc />
		public void Reinitialize()
		{
			if (this.exporter != null)
			{
				this.exporter.Dispose();
				this.exporter = null;
			}
		}

		/// <summary>
		/// Retrieves the file identification enumeration value from the native error code.
		/// </summary>
		/// <param name="error">
		/// The Outside In error code.
		/// </param>
		/// <returns>
		/// The <see cref="FileTypeIdentifyError"/> value.
		/// </returns>
		private static FileTypeIdentifyError GetFileIdError(int error)
		{
			switch (error)
			{
				case OutsideInConstants.FileNotFoundErrorCode:
					return FileTypeIdentifyError.FileNotFound;

				case OutsideInConstants.FilePermissionErrorCode:
					return FileTypeIdentifyError.Permissions;

				default:
					// When all else fails, assume an I/O error.
					return FileTypeIdentifyError.Io;
			}
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

			if (disposing && this.exporter != null)
			{
				this.exporter.Dispose();
				this.exporter = null;
			}

			this.disposed = true;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Initialize()
		{
			if (this.exporter != null)
			{
				return;
			}

			string path = GetInstallPath();

			try
			{
				this.configuration.HasError = false;
				this.configuration.Exception = null;
				var oiConfiguration = new OutsideInConfig
					                      {
						                      InstallLocation = new System.IO.DirectoryInfo(path),
						                      IdleWorkerTimeout = Convert.ToUInt32(
							                      TimeSpan.FromSeconds(this.timeout).TotalMilliseconds),
					                      };
				OutsideIn.SetConfiguration(oiConfiguration);
				this.configuration.Timeout = Convert.ToInt32(TimeSpan.FromMilliseconds(oiConfiguration.IdleWorkerTimeout).TotalSeconds);
				this.configuration.InstallDirectory = oiConfiguration.InstallLocation.ToString();
				this.ApplyVersion(path);
			}
			catch (OutsideInException e) when (e.ErrorCode == OutsideInConstants.NoErrorCode)
			{
				// OI is already configured.
				this.configuration.Timeout = this.timeout;
				this.configuration.InstallDirectory = path;
				this.ApplyVersion(path);
			}
			catch (OutsideInException e)
			{
				this.configuration.HasError = true;
				this.configuration.Exception = e;
			}
			catch (Exception e)
			{
				this.configuration.HasError = true;
				this.configuration.Exception = e;
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.OutsideInConfigurationError,
					path);
				throw new FileTypeIdentifyException(message, e);
			}

			// Allow this to potentially throw.
			this.exporter = OutsideIn.NewLocalExporter();
		}

		/// <summary>
		/// Applies the OI version to the configuration object.
		/// </summary>
		/// <param name="path">
		/// The OI configuration directory.
		/// </param>
		private void ApplyVersion(string path)
		{
			OutsideInVersion version = OutsideIn.GetCoreVersion();
			if (version == null)
			{
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.OutsideInNotAvailableError,
					path);
				throw new InvalidOperationException(message);
			}

			this.configuration.Version = version.GetVersion();
		}
	}
}