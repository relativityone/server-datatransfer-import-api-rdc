// ----------------------------------------------------------------------------
// <copyright file="IFileValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Validation
{
	using System.Threading.Tasks;

	public interface IFileValidator
	{
		Task<bool> IsValidAsync(string actualFilePath);
	}
}