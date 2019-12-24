// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamRequest.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to setup a long text stream request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System.Text;

	/// <summary>
	/// Represents a class object to setup a long text stream request.
	/// </summary>
	internal class LongTextStreamRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LongTextStreamRequest"/> class.
		/// </summary>
		public LongTextStreamRequest()
		{
			this.SourceEncoding = Encoding.Default;
			this.SourceFieldArtifactId = 0;
			this.SourceObjectArtifactId = 0;
			this.SourceTotalBytes = 0L;
			this.TargetEncoding = Encoding.Default;
			this.TargetFile = null;
			this.WorkspaceId = 0;
		}

		/// <summary>
		/// Gets or sets the source artifact identifier for the object that contains the long text field.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int SourceObjectArtifactId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the source artifact identifier for the long text field.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int SourceFieldArtifactId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the source encoding and defaults to <see cref="Encoding.Default"/>.
		/// </summary>
		/// <value>
		/// The <see cref="Encoding"/> instance.
		/// </value>
		public Encoding SourceEncoding
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the total number of bytes contained within the source stream.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long SourceTotalBytes
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target encoding and defaults to <see cref="Encoding.Default"/>.
		/// </summary>
		/// <value>
		/// The <see cref="Encoding"/> instance.
		/// </value>
		public Encoding TargetEncoding
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the full path to the target file to create.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string TargetFile
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the workspace artifact identifier.
		/// </summary>
		/// <value>
		/// The artifact identifier.
		/// </value>
		public int WorkspaceId
		{
			get;
			set;
		}
	}
}