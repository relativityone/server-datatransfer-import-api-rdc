// <copyright file="ToStringMethod.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange;

	public class ToStringMethod
    {
        [Test]
        public void ReturnsLargestMetricSuffix()
        {
            // Arrange
            var b = ByteSize.FromKiloBytes(10.5);

            // Act
            var result = b.ToString();

            // Assert
            Assert.AreEqual(10.5.ToString("0.0 KB"), result);
        }

        [Test]
        public void ReturnsDefaultNumberFormat()
        {
            // Arrange
            var b = ByteSize.FromKiloBytes(10.5);

            // Act
            var result = b.ToString("KB");

            // Assert
            Assert.AreEqual(10.5.ToString("0.0 KB"), result);
        }

        [Test]
        public void ReturnsProvidedNumberFormat()
        {
            // Arrange
            var b = ByteSize.FromKiloBytes(10.1234);

            // Act
            var result = b.ToString("#.#### KB");

            // Assert
            Assert.AreEqual(10.1234.ToString("0.0000 KB"), result);
        }

        [Test]
        public void ReturnsBytes()
        {
            // Arrange
            var b = ByteSize.FromBytes(10);

            // Act
            var result = b.ToString("##.#### B");

            // Assert
            Assert.AreEqual("10 B", result);
        }

        [Test]
        public void ReturnsKiloBytes()
        {
            // Arrange
            var b = ByteSize.FromKiloBytes(10);

            // Act
            var result = b.ToString("##.#### KB");

            // Assert
            Assert.AreEqual("10 KB", result);
        }

        [Test]
        public void ReturnsMegaBytes()
        {
            // Arrange
            var b = ByteSize.FromMegaBytes(10);

            // Act
            var result = b.ToString("##.#### MB");

            // Assert
            Assert.AreEqual("10 MB", result);
        }

        [Test]
        public void ReturnsGigaBytes()
        {
            // Arrange
            var b = ByteSize.FromGigaBytes(10);

            // Act
            var result = b.ToString("##.#### GB");

            // Assert
            Assert.AreEqual("10 GB", result);
        }

        [Test]
        public void ReturnsTeraBytes()
        {
            // Arrange
            var b = ByteSize.FromTeraBytes(10);

            // Act
            var result = b.ToString("##.#### TB");

            // Assert
            Assert.AreEqual("10 TB", result);
        }

        [Test]
        public void ReturnsPetaBytes()
        {
            // Arrange
            var b = ByteSize.FromPetaBytes(10);

            // Act
            var result = b.ToString("##.#### PB");

            // Assert
            Assert.AreEqual("10 PB", result);
        }

        [Test]
        public void ReturnsSelectedFormat()
        {
            // Arrange
            var b = ByteSize.FromTeraBytes(10);

            // Act
            var result = b.ToString("0.0 TB");

            // Assert
            Assert.AreEqual(10.ToString("0.0 TB"), result);
        }

        [Test]
        public void ReturnsLargestMetricPrefixLargerThanZero()
        {
            // Arrange
            var b = ByteSize.FromMegaBytes(.5);

            // Act
            var result = b.ToString("#.#");

            // Assert
            Assert.AreEqual("512 KB", result);
        }

        [Test]
        public void ReturnsLargestMetricPrefixLargerThanZeroForNegativeValues()
        {
            // Arrange
            var b = ByteSize.FromMegaBytes(-.5);

            // Act
            var result = b.ToString("#.#");

            // Assert
            Assert.AreEqual("-512 KB", result);
        }

        [Test]
		public void ReturnsZeroBits()
		{
			// Arrange
			var b = ByteSize.FromBits(0);

			// Act
			var result = b.ToString();

			// Assert
			Assert.AreEqual("0 b", result);
		}
	}
}
