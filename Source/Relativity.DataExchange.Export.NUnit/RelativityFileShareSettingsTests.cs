using NUnit.Framework;
using Relativity.DataExchange.Export.VolumeManagerV2;
using Relativity.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    using Moq;

    [TestFixture]
    public class RelativityFileShareSettingsTests
    {
        [Test]
        public void Constructor_NullFileShare_ThrowsArgumentNullException()
        {
            // Arrange
            RelativityFileShare fileShare = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RelativityFileShareSettings(fileShare));
        }

        [Test]
        public void Equals_NullObject_ReturnsFalse()
        {
            // Arrange
            RelativityFileShareSettings settings = new RelativityFileShareSettings(new RelativityFileShare());

            // Act
            bool result = settings.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }


        [Test]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            RelativityFileShare fileShare = new RelativityFileShare();
            RelativityFileShareSettings settings = new RelativityFileShareSettings(fileShare);

            // Act
            bool result = settings.Equals(settings);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_ObjectOfDifferentType_ReturnsFalse()
        {
            // Arrange
            RelativityFileShare fileShare = new RelativityFileShare();
            RelativityFileShareSettings settings = new RelativityFileShareSettings(fileShare);
            object obj = new object(); // Create an object of a different type

            // Act
            bool result = settings.Equals(obj);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
