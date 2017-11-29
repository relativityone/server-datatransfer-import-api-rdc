using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.TApi
{

	/// <summary>
	/// Represent error that occures when there are illegal characters in the file path.
	/// </summary>
	[Serializable]
	public class FileInfoInvalidPathException : Exception
    {
		/// <summary>
		/// Constructor for <see cref="FileInfoInvalidPathException"/> class
		/// </summary>
		public FileInfoInvalidPathException()
	    {
	    }

	    /// <summary>
		/// Constructor for <see cref="FileInfoInvalidPathException"/> class
		/// </summary>
		public FileInfoInvalidPathException(string message) : base(message)
        {
        }

		/// <summary>
		/// Constructor for <see cref="FileInfoInvalidPathException"/> class
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public FileInfoInvalidPathException(string message, Exception exception) : base(message, exception)
	    {
	    }

		/// <summary>
		/// Constructor for <see cref="FileInfoInvalidPathException"/> class
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected FileInfoInvalidPathException(SerializationInfo info,
			StreamingContext context) : base(info, context)
	    {
	    }
	}
}
