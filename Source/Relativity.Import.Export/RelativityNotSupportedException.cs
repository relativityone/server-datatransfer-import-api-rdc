// ----------------------------------------------------------------------------
// <copyright file="RelativityNotSupportedException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when attempting to import or export with an unsupported Relativity instance.
	/// </summary>
	[Serializable]
	public class RelativityNotSupportedException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityNotSupportedException"/> class.
		/// </summary>
		public RelativityNotSupportedException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityNotSupportedException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public RelativityNotSupportedException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityNotSupportedException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		/// <param name="relativityVersion">
		/// The unsupported Relativity version.
		/// </param>
		public RelativityNotSupportedException(string message, Version relativityVersion)
			: base(message)
		{
			this.RelativityVersion = relativityVersion;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityNotSupportedException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public RelativityNotSupportedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityNotSupportedException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		protected RelativityNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			info.AddValue(nameof(this.RelativityVersion), this.RelativityVersion);
		}

		/// <summary>
		/// Gets the unsupported Relativity version.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		public Version RelativityVersion
		{
			get;
			private set;
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.RelativityVersion = info.GetValue(nameof(this.RelativityVersion), typeof(Version)) as Version;
			base.GetObjectData(info, context);
		}
	}
}