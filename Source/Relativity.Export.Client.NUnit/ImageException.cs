// ----------------------------------------------------------------------------
// <copyright file="ImageException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.Utility;

    public class ImageException : Image.ImageRollupException
	{
		public ImageException() : base("", null)
		{
		}
	}
}