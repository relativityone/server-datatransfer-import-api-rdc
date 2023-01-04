// -----------------------------------------------------------------------------------------------------
// <copyright file="ArtifactFieldCollectionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CaseInfo"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Api;

	using Relativity.DataExchange.Service;

	[TestFixture]
	public class ArtifactFieldCollectionTests : SerializationTestsBase
	{
		private static IReadOnlyList<ArtifactField> Artifacts =>
			new List<ArtifactField>
				{
					new ArtifactField("displayName", 100, FieldType.Varchar, FieldCategory.Generic, 100, 100, 4, true),
					new ArtifactField("displayName2", 101, FieldType.LayoutText, FieldCategory.Generic, 101, 100, 4, true),
					new ArtifactField("displayName3", 102, FieldType.Boolean, FieldCategory.Generic, 102, 100, 4, true),
					new ArtifactField("displayName4", 103, FieldType.Code, FieldCategory.Generic, 103, 100, 4, true),
					new ArtifactField("displayName5", 104, FieldType.Currency, FieldCategory.Generic, 104, 100, 4, true),
					new ArtifactField("displayName6", 105, FieldType.Date, FieldCategory.Generic, 105, 100, 4, true),
					new ArtifactField("displayName7", 106, FieldType.Decimal, FieldCategory.Generic, 106, 100, 4, true),
					new ArtifactField("displayName8", 107, FieldType.Varchar, FieldCategory.Generic, 107, 100, 4, true),
					new ArtifactField("displayName9", 108, FieldType.Varchar, FieldCategory.Generic, 108, 100, 4, true),
					new ArtifactField("displayName10", 109, FieldType.Varchar, FieldCategory.Generic, 109, 100, 4, true)
				};

		[Test]
		public void ArtifactsShouldBeFindableAfterInsert()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			var toInsert = Artifacts.First();
			collection.Add1(toInsert);

			// Assert
			Assert.True(collection[toInsert.ArtifactID].Equals(toInsert));
			Assert.True(collection[toInsert.DisplayName].Equals(toInsert));
			Assert.True(collection.get_FieldList(FieldType.Varchar)[0].Equals(toInsert));
			Assert.True(collection.get_FieldList(FieldType.LayoutText).Length == 0);
		}

		[Test]
		public void ArtifactsShouldNotBeFindableIfNotInserted()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			var toInsert = Artifacts.Skip(1).First();
			collection.Add1(toInsert);

			// Assert
			Assert.IsNull(collection[Artifacts.First().ArtifactID]);
			Assert.IsNull(collection[Artifacts.First().DisplayName]);
			Assert.True(collection.get_FieldList(FieldType.LayoutText).Length == 1);
		}

		[Test]
		public void ArtifactsShouldNotThrowWhenAdded()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			var toInsert = Artifacts;
			collection.AddRange(toInsert);

			// Assert
			Assert.IsNotNull(collection[Artifacts.First().ArtifactID]);
			Assert.IsNotNull(collection[Artifacts.First().DisplayName]);
			Assert.True(collection.get_FieldList(FieldType.Varchar).Length == 4);
		}

		[Test]
		public void ArtifactsShouldDisappearWhenClear()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			var toInsert = Artifacts;
			collection.AddRange(toInsert);
			collection.Clear();

			// Assert
			Assert.IsNull(collection[Artifacts.First().ArtifactID]);
			Assert.IsNull(collection[Artifacts.First().DisplayName]);
			Assert.True(collection.get_FieldList(FieldType.Varchar).Length == 0);
		}

		[Test]
		public void ArtifactsShouldBeAddedToCollection()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			var toInsert = Artifacts;
			collection.AddRange(toInsert);

			// Assert
			Assert.IsEmpty(toInsert.Except(collection.ToList()));
		}

		[Test]
		public void ArtifactsShouldThrowDuplicateKeyWhenInsertingDuplicateKey()
		{
			// Arrange
			ArtifactFieldCollection collection = new ArtifactFieldCollection();

			// Act
			collection.Add1(Artifacts.First());

			// Assert
			Assert.Throws<ArgumentException>(() => collection.Add1(Artifacts.First()));
		}
	}
}