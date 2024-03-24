using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Relativity.DataExchange.Export.NUnit
{
    [TestFixture]
    public class WriterStubTests
    {
        [Test]
        public void WriteEntry_Adds_Entry_To_Text()
        {
            // Arrange
            var stub = new WriterStub();
            var entry = "TestEntry";

            // Act
            stub.WriteEntry(entry, CancellationToken.None);

            // Assert
            Assert.AreEqual(entry, stub.Text);
        }

        [Test]
        public void WriteChunk_Adds_Chunk_To_Text()
        {
            // Arrange
            var stub = new WriterStub();
            var chunk = "TestChunk";

            // Act
            stub.WriteChunk(chunk, CancellationToken.None);

            // Assert
            Assert.AreEqual(chunk, stub.Text);
        }

        [Test]
        public void FlushChunks_Does_Not_Affect_Text()
        {
            // Arrange
            var stub = new WriterStub();
            var initialText = stub.Text;

            // Act
            stub.FlushChunks(CancellationToken.None);

            // Assert
            Assert.AreEqual(initialText, stub.Text);
        }
        [Test]
        public void InitializeFile_Does_Not_Throw_Exception()
        {
            // Arrange
            var stub = new WriterStub();

            // Act
            TestDelegate act = () => stub.InitializeFile(CancellationToken.None);

            // Assert
            Assert.DoesNotThrow(act);
        }

        [Test]
        public void RestoreLastState_Does_Not_Throw_Exception()
        {
            // Arrange
            var stub = new WriterStub();

            // Act
            TestDelegate act = () => stub.RestoreLastState();

            // Assert
            Assert.DoesNotThrow(act);
        }

        [Test]
        public void SaveState_Does_Not_Throw_Exception()
        {
            // Arrange
            var stub = new WriterStub();

            // Act
            TestDelegate act = () => stub.SaveState();

            // Assert
            Assert.DoesNotThrow(act);
        }

        [Test]
        public void Dispose_Does_Not_Throw_Exception()
        {
            // Arrange
            var stub = new WriterStub();

            // Act
            TestDelegate act = () => stub.Dispose();

            // Assert
            Assert.DoesNotThrow(act);
        }

    }
}