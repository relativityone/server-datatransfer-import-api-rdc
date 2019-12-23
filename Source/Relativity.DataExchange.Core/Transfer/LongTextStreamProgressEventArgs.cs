// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamProgressEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents long text stream progress event data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	/// <summary>
	/// Represents long text stream progress event data.
	/// </summary>
	internal struct LongTextStreamProgressEventArgs
	{
		private readonly ByteSize size;

		/// <summary>
		/// Initializes a new instance of the <see cref="LongTextStreamProgressEventArgs"/> struct.
		/// </summary>
		/// <param name="request">
		/// The request that raised this progress event.
		/// </param>
		/// <param name="totalBytesWritten">
		/// The total number of bytes written to the target stream.
		/// </param>
		/// <param name="completed">
		/// The value that indicates whether the long text stream transfer is complete.
		/// </param>
		public LongTextStreamProgressEventArgs(
			LongTextStreamRequest request,
			long totalBytesWritten,
			bool completed)
		{
			this.Request = request.ThrowIfNull(nameof(request));
			this.size = ByteSize.FromBytes(totalBytesWritten);
			this.TotalBytesWritten = totalBytesWritten;
			this.Completed = completed;
		}

		/// <summary>
		/// Gets a value indicating whether the long text stream transfer is complete.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when the transfer is complete; otherwise, <see langword="false" />.
		/// </value>
		public bool Completed
		{
			get;
		}

		/// <summary>
		/// Gets the request that raised this progress event.
		/// </summary>
		/// <value>
		/// The <see cref="LongTextStreamRequest"/> instance.
		/// </value>
		public LongTextStreamRequest Request
		{
			get;
		}

		/// <summary>
		/// Gets the total number of bytes written to the target stream.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long TotalBytesWritten
		{
			get;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return
				$"Large file transfer progress: {System.IO.Path.GetFileName(this.Request.TargetFile)} : {this.size.ToString("#.##")}";
		}
	}
}