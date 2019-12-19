// <copyright file="IErrorArguments.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	/// <summary>
	/// This object can retrieve arguments for an error.
	/// </summary>
	public interface IErrorArguments
	{
		/// <summary>
		/// Returns the values for a record/line in an error file one by one.
		/// </summary>
		/// <returns>Values for the error file.</returns>
		string FormattedLineInFile();
	}
}
