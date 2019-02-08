// ----------------------------------------------------------------------------
// <copyright file="ImageException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export.VolumeManagerV2.ImagesRollup
{
    using kCura.Utility;

    public class ImageException : Image.ImageRollupException
	{
		public ImageException() : base("", null)
		{
		}
	}
}