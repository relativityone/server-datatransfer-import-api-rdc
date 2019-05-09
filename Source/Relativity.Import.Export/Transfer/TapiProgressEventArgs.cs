﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiProgressEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Transfer API progress event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using Relativity.Transfer;

	/// <summary>
	/// Represents Transfer API progress event arguments data. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiProgressEventArgs"/> class.
		/// </summary>
		/// <param name="fileName">
		/// The transferred filename.
		/// </param>
		/// <param name="didTransferSucceed">
		/// Specify whether the file is successfully transferred.
		/// </param>
		/// <param name="transferStatus">
		/// The transfer status.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		/// <param name="fileBytes">
		/// The file bytes.
		/// </param>
		/// <param name="startTime">
		/// The start transfer time.
		/// </param>
		/// <param name="endTime">
		/// The end transfer time.
		/// </param>
		public TapiProgressEventArgs(
			string fileName,
			bool didTransferSucceed,
			TransferPathStatus transferStatus,
			int lineNumber,
			long fileBytes,
			DateTime startTime,
			DateTime endTime)
		{
			this.EndTime = endTime;
			this.FileBytes = fileBytes;
			this.FileName = fileName;
			this.LineNumber = lineNumber;
			this.StartTime = startTime;
			this.DidTransferSucceed = didTransferSucceed;
			this.TransferStatus = transferStatus;
		}

		/// <summary>
		/// Gets the end transfer time.
		/// </summary>
		public DateTime EndTime
		{
			get;
		}

		/// <summary>
		/// Gets the total transferred bytes.
		/// </summary>
		public long FileBytes
		{
			get;
		}

		/// <summary>
		/// Gets the transferred file name.
		/// </summary>
		public string FileName
		{
			get;
		}

		/// <summary>
		/// Gets the line number.
		/// </summary>
		public int LineNumber
		{
			get;
		}

		/// <summary>
		/// Gets the start transfer time.
		/// </summary>
		public DateTime StartTime
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the file was successfully transferred.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if successfully transferred; otherwise, /// <see langword="false"/>.
		/// </value>
		public bool DidTransferSucceed
		{
			get;
		}

		/// <summary>
		/// Gets the transfer status.
		/// </summary>
		public TransferPathStatus TransferStatus
		{
			get;
		}
	}
}