// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeParameters2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file upload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	/// <summary>
	/// Represents the generic parameters to setup a Transfer API bridge for upload. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	public sealed class UploadTapiBridgeParameters2 : TapiBridgeParameters2
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridgeParameters2"/> class.
		/// </summary>
		public UploadTapiBridgeParameters2()
		{
			this.BcpFileTransfer = false;
			this.MaxFilesPerFolder = 1000;
			this.SortIntoVolumes = true;

			// Those settings are set the same in the base class for safety to preserve possible unforeseen usages
			this.FileNotFoundErrorsRetry = AppSettingsConstants.TapiFileNotFoundErrorsRetryDefaultValue;
			this.FileNotFoundErrorsDisabled = AppSettingsConstants.TapiFileNotFoundErrorsDisabledDefaultValue;
			this.PermissionErrorsRetry = AppSettingsConstants.PermissionErrorsRetryDefaultValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridgeParameters2"/> class.
		/// </summary>
		/// <param name="copy">
		/// The parameters to copy.
		/// </param>
		public UploadTapiBridgeParameters2(UploadTapiBridgeParameters2 copy)
			: base(copy)
		{
			this.BcpFileTransfer = copy.BcpFileTransfer;
			this.MaxFilesPerFolder = copy.MaxFilesPerFolder;
			this.SortIntoVolumes = copy.SortIntoVolumes;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridgeParameters2"/> class.
		/// </summary>
		/// <param name="copy">
		/// The parameters to copy.
		/// </param>
		public UploadTapiBridgeParameters2(TapiBridgeParameters2 copy)
			: base(copy)
		{
			this.BcpFileTransfer = false;
			this.MaxFilesPerFolder = 1000;
			this.SortIntoVolumes = true;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the file transfer is BCP based.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the file transfer is BCP based; otherwise, <see langword="false" />.
		/// </value>
		public bool BcpFileTransfer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum files per folder setting.
		/// </summary>
		/// <value>
		/// The maximum files per folder.
		/// </value>
		public int MaxFilesPerFolder
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to sort all transfers into a volumes folder. This is a native file specific parameter.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to sort all transfers into a volumes folder; otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// This is always <see langword="true" /> unless transferring BCP files.
		/// </remarks>
		public bool SortIntoVolumes
		{
			get;
			set;
		}

		/// <summary>
		/// Performs a shallow copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="TapiBridgeParameters2"/> instance.
		/// </returns>
		public UploadTapiBridgeParameters2 ShallowCopy()
		{
			return new UploadTapiBridgeParameters2(this);
		}
	}
}