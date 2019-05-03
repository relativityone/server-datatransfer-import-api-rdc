// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using Relativity.Import.Export;
    using Relativity.Import.Export.Media;

    public class ImageException : ImageRollupException
	{
		public ImageException()
			: base(string.Empty, null)
		{
		}
	}
}