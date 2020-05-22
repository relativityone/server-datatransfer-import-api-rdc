// ----------------------------------------------------------------------------
// <copyright file="OverlayIdentifierTestCases.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;

	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
	public static class OverlayIdentifierTestCases
	{
		private static readonly DocumentWithKeyFieldDto[] InitialData =
			{
				new DocumentWithKeyFieldDto("100010", "AAA"),
				new DocumentWithKeyFieldDto("100011", "BBB"),
				new DocumentWithKeyFieldDto("100012", "CCC"),
				new DocumentWithKeyFieldDto("100013", "DDD"),
				new DocumentWithKeyFieldDto("100014", "EEE"),
			};

		public static IEnumerable<TestCaseData> ShouldOverlayIdentifierTestCaseData
		{
			get
			{
				yield return new TestCaseData(
					"Overlay object type identifier in overlay mode",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					});

				yield return new TestCaseData(
					"Overlay object type identifier in append/overlay mode",
					OverwriteModeEnum.AppendOverlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("200014", "FFF"),
						new DocumentWithKeyFieldDto("200015", "GGG"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
						new DocumentWithKeyFieldDto("200014", "FFF"),
						new DocumentWithKeyFieldDto("200015", "GGG"),
					});

				yield return new TestCaseData(
					"Overlay object type identifier with doubled identifier values",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200012", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200012", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					});

				yield return new TestCaseData(
					"Overlay object type identifier with switched identifier values",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("100012", "BBB"),
						new DocumentWithKeyFieldDto("100011", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("100012", "BBB"),
						new DocumentWithKeyFieldDto("100011", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					});

				yield return new TestCaseData(
					"Overlay object type identifier with already existing identifier values",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("100014", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("100014", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					});
			}
		}

		public static IEnumerable<TestCaseData> ShouldOverlayIdentifierWithErrorTestCaseData
		{
			get
			{
				yield return new TestCaseData(
					"Overlay object type identifier with null and empty key field values",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", string.Empty),
						new DocumentWithKeyFieldDto("200013", null),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("100012", "CCC"),
						new DocumentWithKeyFieldDto("100013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					},
					"Identity value not set");

				yield return new TestCaseData(
					"Overlay object type identifier with doubled key field values",
					OverwriteModeEnum.Overlay,
					InitialData,
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "BBB"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("100012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "EEE"),
					},
					"Document '(BBB)' has been previously processed in this file on line 2.");

				yield return new TestCaseData(
					"Overlay object type identifier with initially doubled key field values",
					OverwriteModeEnum.Overlay,
					new[]
					{
						new DocumentWithKeyFieldDto("100010", "AAA"),
						new DocumentWithKeyFieldDto("100011", "BBB"),
						new DocumentWithKeyFieldDto("100012", "CCC"),
						new DocumentWithKeyFieldDto("100013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("200013", "DDD"),
					},
					new[]
					{
						new DocumentWithKeyFieldDto("200010", "AAA"),
						new DocumentWithKeyFieldDto("200011", "BBB"),
						new DocumentWithKeyFieldDto("200012", "CCC"),
						new DocumentWithKeyFieldDto("100013", "DDD"),
						new DocumentWithKeyFieldDto("100014", "DDD"),
					},
					" - This record's Overlay Identifier is shared by multiple documents in the case, and cannot be imported");
			}
		}
	}
}