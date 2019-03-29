// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using Relativity.Import.Export;

    public class ImageException : ImageRollupException
	{
		public ImageException()
			: base(string.Empty, null)
		{
		}
	}
}