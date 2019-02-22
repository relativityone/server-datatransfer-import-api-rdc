// ----------------------------------------------------------------------------
// <copyright file="OutsideInFileIdService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Globalization;
	using System.Reflection;

	using OutsideIn;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents a file identification service class object using Outside In technology.
	/// </summary>
	public class OutsideInFileIdService : IFileIdService
	{
		/// <summary>
		/// The file identification configuration.
		/// </summary>
		private readonly FileIdConfiguration configuration = new FileIdConfiguration();

		/// <summary>
		/// The optional timeout.
		/// </summary>
		private readonly int? timeout;

		/// <summary>
		/// The OI exporter instance.
		/// </summary>
		private Exporter exporter;

		/// <summary>
		/// The disposed backing field.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="OutsideInFileIdService"/> class.
		/// </summary>
		public OutsideInFileIdService()
			: this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OutsideInFileIdService"/> class.
		/// </summary>
		/// <param name="timeout">
		/// The optional timeout.
		/// </param>
		public OutsideInFileIdService(int? timeout)
		{
			this.timeout = timeout;
			this.disposed = false;
			this.exporter = null;
		}

		/// <inheritdoc />
		public FileIdConfiguration Configuration
		{
			get
			{
				this.Initialize();
				return this.configuration;
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public FileIdInfo Identify(string file)
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
				return new FileIdInfo(file, fileFormat.GetId(), fileFormat.GetDescription());
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
				throw new FileIdException(message, e, GetFileIdError(e.ErrorCode));
			}
			catch (Exception e)
			{
				string message = string.Format(CultureInfo.CurrentCulture, Strings.OutsideInFileIdUnexpectedError, file);
				throw new FileIdException(message, e);
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
		/// The <see cref="FileIdError"/> value.
		/// </returns>
		private static FileIdError GetFileIdError(int error)
		{
			switch (error)
			{
				case OutsideInConstants.FileNotFoundErrorCode:
					return FileIdError.FileNotFound;

				case OutsideInConstants.FilePermissionErrorCode:
					return FileIdError.Permissions;

				default:
					// When all else fails, assume an I/O error.
					return FileIdError.Io;
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

			string assemblyPath = new System.Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
			string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assemblyPath), "oi");
			if (!string.IsNullOrEmpty(path))
			{
				path = path.Replace("%20", " ");
			}

			try
			{
				this.configuration.HasError = false;
				this.configuration.Exception = null;
				OutsideInVersion version = OutsideIn.GetCoreVersion();
				this.configuration.Version = version.GetVersion();
				OutsideInConfig oiConfiguration =
					new OutsideInConfig { InstallLocation = new System.IO.DirectoryInfo(path) };
				if (this.timeout != null)
				{
					oiConfiguration.IdleWorkerTimeout = Convert.ToUInt32(this.timeout);
				}

				this.configuration.Timeout = Convert.ToInt32(oiConfiguration.IdleWorkerTimeout);
				this.configuration.InstallDirectory = oiConfiguration.InstallLocation.ToString();
				OutsideIn.SetConfiguration(oiConfiguration);
			}
			catch (OutsideInException e) when (e.ErrorCode == OutsideInConstants.NoErrorCode)
			{
				// OI is already configured.
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
				throw new FileIdException(Strings.OutsideInConfigurationError, e);
			}

			// Allow this to potentially throw.
			this.exporter = OutsideIn.NewLocalExporter();
		}
	}
}