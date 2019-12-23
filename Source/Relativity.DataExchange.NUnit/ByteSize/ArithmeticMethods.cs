// <copyright file="ArithmeticMethods.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange;

	public class ArithmeticMethods
    {
	    [Test]
        public void IncrementOperator()
        {
            var size = ByteSize.FromBytes(2);
            size++;

            Assert.AreEqual(24, size.Bits);
            Assert.AreEqual(3, size.Bytes);
        }

        [Test]
        public void MinusOperatorUnary()
        {
            var size = ByteSize.FromBytes(2);

            size = -size;

            Assert.AreEqual(-16, size.Bits);
            Assert.AreEqual(-2, size.Bytes);
        }

        [Test]
        public void DecrementOperator()
        {
            var size = ByteSize.FromBytes(2);
            size--;

            Assert.AreEqual(8, size.Bits);
            Assert.AreEqual(1, size.Bytes);
        }
    }
}
