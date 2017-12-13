// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeParameters.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file upload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// Represents the generic parameters to setup a Transfer API bridge for upload.
	/// </summary>
	public class UploadTapiBridgeParameters : TapiBridgeParameters
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridgeParameters"/> class.
		/// </summary>
		public UploadTapiBridgeParameters()
		{
			this.BcpFileTransfer = false;
			this.AsperaBcpRootFolder = "BCPPath";
			this.MaxFilesPerFolder = 1000;
			this.SortIntoVolumes = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridgeParameters"/> class.
		/// </summary>
		/// <param name="copy">
		/// The parameters to copy.
		/// </param>
		public UploadTapiBridgeParameters(UploadTapiBridgeParameters copy) : base(copy)
		{
			this.AsperaBcpRootFolder = copy.AsperaBcpRootFolder;
			this.BcpFileTransfer = copy.BcpFileTransfer;
			this.MaxFilesPerFolder = copy.MaxFilesPerFolder;
			this.SortIntoVolumes = copy.SortIntoVolumes;
		}

		/// <summary>
		/// Gets or sets the Aspera BCP root folder.
		/// </summary>
		/// <value>
		/// The folder.
		/// </value>
		public string AsperaBcpRootFolder
		{
			get;
			set;
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
		/// The <see cref="TapiBridgeParameters"/> instance.
		/// </returns>
		public UploadTapiBridgeParameters ShallowCopy()
		{
			return new UploadTapiBridgeParameters(this);
		}
	}
}