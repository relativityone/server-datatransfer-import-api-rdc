﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteSize.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a byte size value.
// </summary>
// <auto-generated/>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Diagnostics;
	using System.Globalization;

	/// <summary>
	/// Represents a byte size value.
	/// </summary>
	/// <remarks>
	/// This class was ripped from <see href="https://github.com/omar/ByteSize"/>.
	/// This is intentionally excluded from code coverage.
	/// </remarks>
	[DebuggerNonUserCode]
	internal struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize>
	{
		public const long BitsInByte = 8;
		public const long BytesInKiloByte = 1024;
		public const long BytesInMegaByte = 1048576;
		public const long BytesInGigaByte = 1073741824;
		public const long BytesInTeraByte = 1099511627776;
		public const long BytesInPetaByte = 1125899906842624;

		public const string BitSymbol = "b";
		public const string ByteSymbol = "B";
		public const string KiloByteSymbol = "KB";
		public const string MegaByteSymbol = "MB";
		public const string GigaByteSymbol = "GB";
		public const string TeraByteSymbol = "TB";
		public const string PetaByteSymbol = "PB";

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteSize"/> struct.
		/// </summary>
		/// <param name="byteSize">
		/// Size of the byte.
		/// </param>
		public ByteSize(double byteSize)
			: this()
		{
			// Get ceiling because bis are whole units
			this.Bits = (long)Math.Ceiling(byteSize * BitsInByte);
			this.Bytes = byteSize;
			this.KiloBytes = byteSize / BytesInKiloByte;
			this.MegaBytes = byteSize / BytesInMegaByte;
			this.GigaBytes = byteSize / BytesInGigaByte;
			this.TeraBytes = byteSize / BytesInTeraByte;
			this.PetaBytes = byteSize / BytesInPetaByte;
		}

		public long Bits
		{
			get;
		}

		public double Bytes
		{
			get;
		}

		public double KiloBytes
		{
			get;
		}

		public double MegaBytes
		{
			get;
		}

		public double GigaBytes
		{
			get;
		}

		public double TeraBytes
		{
			get;
		}

		public double PetaBytes
		{
			get;
		}

		public string LargestWholeNumberSymbol
		{
			get
			{
				// Absolute value is used to deal with negative values
				if (Math.Abs(this.PetaBytes) >= 1)
				{
					return PetaByteSymbol;
				}

				if (Math.Abs(this.TeraBytes) >= 1)
				{
					return TeraByteSymbol;
				}

				if (Math.Abs(this.GigaBytes) >= 1)
				{
					return GigaByteSymbol;
				}

				if (Math.Abs(this.MegaBytes) >= 1)
				{
					return MegaByteSymbol;
				}

				if (Math.Abs(this.KiloBytes) >= 1)
				{
					return KiloByteSymbol;
				}

				if (Math.Abs(this.Bytes) >= 1)
				{
					return ByteSymbol;
				}

				return BitSymbol;
			}
		}

		public double LargestWholeNumberValue
		{
			get
			{
				// Absolute value is used to deal with negative values
				if (Math.Abs(this.PetaBytes) >= 1)
				{
					return this.PetaBytes;
				}

				if (Math.Abs(this.TeraBytes) >= 1)
				{
					return this.TeraBytes;
				}

				if (Math.Abs(this.GigaBytes) >= 1)
				{
					return this.GigaBytes;
				}

				if (Math.Abs(this.MegaBytes) >= 1)
				{
					return this.MegaBytes;
				}

				if (Math.Abs(this.KiloBytes) >= 1)
				{
					return this.KiloBytes;
				}

				if (Math.Abs(this.Bytes) >= 1)
				{
					return this.Bytes;
				}

				return this.Bits;
			}
		}

		public static ByteSize FromBits(long value)
		{
			return new ByteSize(value / (double)BitsInByte);
		}

		public static ByteSize FromBytes(double value)
		{
			return new ByteSize(value);
		}

		public static ByteSize FromKiloBytes(double value)
		{
			return new ByteSize(value * BytesInKiloByte);
		}

		public static ByteSize FromMegaBytes(double value)
		{
			return new ByteSize(value * BytesInMegaByte);
		}

		public static ByteSize FromGigaBytes(double value)
		{
			return new ByteSize(value * BytesInGigaByte);
		}

		public static ByteSize FromTeraBytes(double value)
		{
			return new ByteSize(value * BytesInTeraByte);
		}

		public static ByteSize FromPetaBytes(double value)
		{
			return new ByteSize(value * BytesInPetaByte);
		}

		public static ByteSize operator +(ByteSize b1, ByteSize b2)
		{
			return new ByteSize(b1.Bytes + b2.Bytes);
		}

		public static ByteSize operator ++(ByteSize b)
		{
			return new ByteSize(b.Bytes + 1);
		}

		public static ByteSize operator -(ByteSize b)
		{
			return new ByteSize(-b.Bytes);
		}

		public static ByteSize operator --(ByteSize b)
		{
			return new ByteSize(b.Bytes - 1);
		}

		public static bool operator ==(ByteSize b1, ByteSize b2)
		{
			return b1.Bits == b2.Bits;
		}

		public static bool operator !=(ByteSize b1, ByteSize b2)
		{
			return b1.Bits != b2.Bits;
		}

		public static bool operator <(ByteSize b1, ByteSize b2)
		{
			return b1.Bits < b2.Bits;
		}

		public static bool operator <=(ByteSize b1, ByteSize b2)
		{
			return b1.Bits <= b2.Bits;
		}

		public static bool operator >(ByteSize b1, ByteSize b2)
		{
			return b1.Bits > b2.Bits;
		}

		public static bool operator >=(ByteSize b1, ByteSize b2)
		{
			return b1.Bits >= b2.Bits;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Maintainability",
			"CA1502:AvoidExcessiveComplexity",
			Justification = "Preserve existing code from Github repo.")]
		public static bool TryParse(string s, out ByteSize result)
		{
			// Arg checking
			if (string.IsNullOrWhiteSpace(s))
			{
				throw new ArgumentNullException(nameof(s), "String is null or whitespace");
			}

			// Setup the result
			result = new ByteSize();

			// Get the index of the first non-digit character
			s = s.TrimStart(); // Protect against leading spaces

			int num;
			var found = false;

			// Pick first non-digit number
			for (num = 0; num < s.Length; num++)
			{
				if (!(char.IsDigit(s[num]) || s[num] == '.'))
				{
					found = true;
					break;
				}
			}

			if (found == false)
			{
				return false;
			}

			int lastNumber = num;

			// Cut the input string in half
			var numberPart = s.Substring(0, lastNumber).Trim();
			var sizePart = s.Substring(lastNumber, s.Length - lastNumber).Trim();

			// Get the numeric part
			double number;
			if (
				!double.TryParse(
					numberPart,
					NumberStyles.Float | NumberStyles.AllowThousands,
					NumberFormatInfo.InvariantInfo,
					out number))
			{
				return false;
			}

			// Get the magnitude part
			switch (sizePart)
			{
				case "b":
					// Can't have partial bits
					if (number % 1 != 0)
					{
						return false;
					}

					result = FromBits((long)number);
					break;

				case "B":
					result = FromBytes(number);
					break;

				case "KB":
				case "kB":
				case "kb":
					result = FromKiloBytes(number);
					break;

				case "MB":
				case "mB":
				case "mb":
					result = FromMegaBytes(number);
					break;

				case "GB":
				case "gB":
				case "gb":
					result = FromGigaBytes(number);
					break;

				case "TB":
				case "tB":
				case "tb":
					result = FromTeraBytes(number);
					break;

				case "PB":
				case "pB":
				case "pb":
					result = FromPetaBytes(number);
					break;
			}

			return true;
		}

		public static ByteSize Parse(string s)
		{
			ByteSize result;
			if (TryParse(s, out result))
			{
				return result;
			}

			throw new FormatException("Value is not in the correct format");
		}

		/// <summary>
		/// Converts the value of the current ByteSize object to a string.
		/// The metric prefix symbol (bit, byte, kilo, mega, giga, tera) used is
		/// the largest metric prefix such that the corresponding value is greater
		/// than or equal to one.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.LargestWholeNumberValue, this.LargestWholeNumberSymbol);
		}

		public string ToString(string format)
		{
			if (!format.Contains("#") && !format.Contains("0"))
			{
				format = "#.## " + format;
			}

			Func<string, bool> has = s => format.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) != -1;
			Func<double, string> output = n => n.ToString(format, CultureInfo.InvariantCulture);
			if (has("PB"))
			{
				return output(this.PetaBytes);
			}

			if (has("TB"))
			{
				return output(this.TeraBytes);
			}

			if (has("GB"))
			{
				return output(this.GigaBytes);
			}

			if (has("MB"))
			{
				return output(this.MegaBytes);
			}

			if (has("KB"))
			{
				return output(this.KiloBytes);
			}

			// Byte and Bit symbol look must be case-sensitive
			if (format.IndexOf(ByteSymbol, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return output(this.Bytes);
			}

			if (format.IndexOf(BitSymbol, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return output(this.Bits);
			}

			return $"{this.LargestWholeNumberValue.ToString(format, CultureInfo.InvariantCulture)} {this.LargestWholeNumberSymbol}";
		}

		public override bool Equals(object value)
		{
			if (value == null)
			{
				return false;
			}

			ByteSize other;
			if (value is ByteSize)
			{
				other = (ByteSize)value;
			}
			else
			{
				return false;
			}

			return this.Equals(other);
		}

		public bool Equals(ByteSize value)
		{
			return this.Bits == value.Bits;
		}

		public override int GetHashCode()
		{
			return this.Bits.GetHashCode();
		}

		public int CompareTo(ByteSize other)
		{
			return this.Bits.CompareTo(other.Bits);
		}
	}
}