using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images.Lines
{
	[TestFixture]
	public class FullTextLineWriterTests
	{
		private FullTextLineWriter _instance;

		private const string _BATES_NUMBER = "batesNumber";

		[SetUp]
		public void SetUp()
		{
			_instance = new FullTextLineWriter();
		}

		[Test]
		[TestCaseSource(nameof(DataSetsNoPageOffset))]
		public void ItShouldWriteLineWithoutPageOffset(string textToWrite, string expectedResult)
		{
			WriterStub writer = new WriterStub();
			TextReader textReader = new StringReader(textToWrite);

			//ACT
			_instance.WriteLine(_BATES_NUMBER, long.MinValue, writer, textReader, CancellationToken.None);

			//ASSERT
			Assert.That(writer.Text, Is.EqualTo(expectedResult));
		}

		private static IEnumerable<object[]> DataSetsNoPageOffset()
		{
			yield return new object[] {"text to write", $"FT,{_BATES_NUMBER},1,1,text|0|0|0|0^to|0|0|0|0^write{Environment.NewLine}"};
			yield return new object[] {"text", $"FT,{_BATES_NUMBER},1,1,text{Environment.NewLine}"};
			yield return new object[] {"double  space", $"FT,{_BATES_NUMBER},1,1,double|0|0|0|0^|0|0|0|0^space{Environment.NewLine}"};
			yield return new object[] {"co,mma", $"FT,{_BATES_NUMBER},1,1,comma{Environment.NewLine}"};
			yield return new object[] {$"char{(char) 10}10", $"FT,{_BATES_NUMBER},1,1,char|0|0|0|0^10{Environment.NewLine}"};
		}

		[Test]
		[TestCaseSource(nameof(DataSetsPageOffset))]
		public void ItShouldWriteLineWithPageOffset(string textToWrite, long pageOffset, string expectedResult)
		{
			WriterStub writer = new WriterStub();
			TextReader textReader = new StringReader(textToWrite);

			//ACT
			while (textReader.Peek() != -1)
			{
				_instance.WriteLine(_BATES_NUMBER, pageOffset, writer, textReader, CancellationToken.None);
			}

			//ASSERT
			Assert.That(writer.Text, Is.EqualTo(expectedResult));
		}

		private static IEnumerable<object[]> DataSetsPageOffset()
		{
			yield return new object[] {"AAAABBBBCCCC", 4, CreateExpectedResult("AAAA", "BBBB", "CCCC")};
			yield return new object[] {"AABBC", 2, CreateExpectedResult("AA", "BB", "C")};
			yield return new object[] {"A B C", 2, CreateExpectedResult("A|0|0|0|0^", "B|0|0|0|0^", "C")};
			yield return new object[] {"AB", 99, CreateExpectedResult("AB")};
			yield return new object[] {"A,B,C", 2, CreateExpectedResult("A", "B", "C")};
		}

		private static string CreateExpectedResult(params string[] lines)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var line in lines)
			{
				builder.Append($"FT,{_BATES_NUMBER},1,1,{line}{Environment.NewLine}");
			}

			return builder.ToString();
		}
	}
}