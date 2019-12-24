// <copyright file="CreatingMethods.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange;

	public class CreatingMethods
    {
        [Test]
        public void Constructor()
        {
            // Arrange
            double byteSize = 1125899906842624;

            // Act
            var result = new ByteSize(byteSize);

            // Assert
            Assert.AreEqual(byteSize * 8, result.Bits);
            Assert.AreEqual(byteSize, result.Bytes);
            Assert.AreEqual(byteSize / 1024, result.KiloBytes);
            Assert.AreEqual(byteSize / 1024 / 1024, result.MegaBytes);
            Assert.AreEqual(byteSize / 1024 / 1024 / 1024, result.GigaBytes);
            Assert.AreEqual(byteSize / 1024 / 1024 / 1024 / 1024, result.TeraBytes);
            Assert.AreEqual(1, result.PetaBytes);
        }

        [Test]
        public void FromBitsMethod()
        {
            // Arrange
            long value = 8;

            // Act
            var result = ByteSize.FromBits(value);

            // Assert
            Assert.AreEqual(8, result.Bits);
            Assert.AreEqual(1, result.Bytes);
        }

        [Test]
        public void FromBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromBytes(value);

            // Assert
            Assert.AreEqual(12, result.Bits);
            Assert.AreEqual(1.5, result.Bytes);
        }

        [Test]
        public void FromKiloBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromKiloBytes(value);

            // Assert
            Assert.AreEqual(1536, result.Bytes);
            Assert.AreEqual(1.5, result.KiloBytes);
        }

        [Test]
        public void FromMegaBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromMegaBytes(value);

            // Assert
            Assert.AreEqual(1572864, result.Bytes);
            Assert.AreEqual(1.5, result.MegaBytes);
        }

        [Test]
        public void FromGigaBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromGigaBytes(value);

            // Assert
            Assert.AreEqual(1610612736, result.Bytes);
            Assert.AreEqual(1.5, result.GigaBytes);
        }

        [Test]
        public void FromTeraBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromTeraBytes(value);

            // Assert
            Assert.AreEqual(1649267441664, result.Bytes);
            Assert.AreEqual(1.5, result.TeraBytes);
        }

        [Test]
        public void FromPetaBytesMethod()
        {
            // Arrange
            double value = 1.5;

            // Act
            var result = ByteSize.FromPetaBytes(value);

            // Assert
            Assert.AreEqual(1688849860263936, result.Bytes);
            Assert.AreEqual(1.5, result.PetaBytes);
        }
    }
}
