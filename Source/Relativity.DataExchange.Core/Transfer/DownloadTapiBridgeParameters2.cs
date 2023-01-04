// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeParameters2.cs" company="Relativity ODA LLC">
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
	public sealed class DownloadTapiBridgeParameters2 : TapiBridgeParameters2
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridgeParameters2"/> class.
		/// </summary>
		public DownloadTapiBridgeParameters2()
		{
			this.FileNotFoundErrorsRetry = AppSettingsConstants.TapiExportFileNotFoundErrorsRetryDefaultValue;
			this.FileNotFoundErrorsDisabled = AppSettingsConstants.TapiExportFileNotFoundErrorsDisabledDefaultValue;
			this.PermissionErrorsRetry = AppSettingsConstants.ExportPermissionErrorsRetryDefaultValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridgeParameters2"/> class.
		/// </summary>
		/// <param name="copy">
		/// The parameters to copy.
		/// </param>
		public DownloadTapiBridgeParameters2(TapiBridgeParameters2 copy)
			: base(copy)
		{
		}

		/// <summary>
		/// Performs a shallow copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="TapiBridgeParameters2"/> instance.
		/// </returns>
		public DownloadTapiBridgeParameters2 ShallowCopy()
		{
			return new DownloadTapiBridgeParameters2(this);
		}
	}
}