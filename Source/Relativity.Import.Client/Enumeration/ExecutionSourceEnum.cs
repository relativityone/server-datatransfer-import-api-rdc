// This is a duplicate of the enum in Relativity because we cannot access it from the IAPI due to
// how we distribute IAPI for client consumption
namespace kCura.Relativity.ImportAPI.Enumeration
{
	/// <summary>
	/// Specifies where the document is being imported from.
	/// </summary>
	public enum ExecutionSourceEnum
	{
		/// <summary>
		/// Used when we don't know the import origin or it has not been specified
		/// </summary>
		Unknown,
		/// <summary>
		/// We are importing through the RDC
		/// </summary>
		RDC,
		/// <summary>
		///We are importing through the Import API 
		/// </summary>
		ImportAPI,
		/// <summary>
		///We are importing through Relativity Integration Points (RIP)
		/// </summary>
		RIP,
		/// <summary>
		///We are importing through Processing
		/// </summary>
		Processing
	}
}
