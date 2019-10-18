// ----------------------------------------------------------------------------
// <copyright file="PaddingDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration.Dto
{
	public class PaddingDto
	{
		public PaddingDto(int subdirectoryDigitPadding, int volumeDigitPadding)
		{
			this.SubdirectoryDigitPadding = subdirectoryDigitPadding;
			this.VolumeDigitPadding = volumeDigitPadding;
		}

		public int SubdirectoryDigitPadding { get; }

		public int VolumeDigitPadding { get; }

		public override string ToString()
		{
			return $"({this.SubdirectoryDigitPadding}, {this.VolumeDigitPadding})";
		}
	}
}
