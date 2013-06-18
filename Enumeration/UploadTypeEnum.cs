using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Enumeration
{
	/// <summary>
	/// Specifies the file upload mode used by the Import API.
	/// </summary>
	public enum UploadTypeEnum
	{
		/// <summary>
		/// Uploads files through the web server. This is the standard mode.
		/// </summary>
		Web,
		/// <summary>
		/// Uploads files directly.
		/// </summary>
		/// <remarks>
		/// This mode is significantly faster than Web mode.  It requires specific Windows group permissions and a connection to the network hosting the data.
		/// If an upload in this mode fails, the system reverts to Web mode.
		/// </remarks>
		Direct
	}
}
