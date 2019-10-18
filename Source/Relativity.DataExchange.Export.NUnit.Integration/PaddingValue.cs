// ----------------------------------------------------------------------------
// <copyright file="PaddingValue.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	public class PaddingValue
	{
		public PaddingValue(int subdirectoryDigitPadding, int volumeDigitPadding)
		{
			this.SubdirectoryDigitPadding = subdirectoryDigitPadding;
			this.VolumeDigitPadding = volumeDigitPadding;
		}

		public int SubdirectoryDigitPadding { get; set; }

		public int VolumeDigitPadding { get; set; }

		public override string ToString()
		{
			return $"({this.SubdirectoryDigitPadding}, {this.VolumeDigitPadding})";
		}
	}
}
