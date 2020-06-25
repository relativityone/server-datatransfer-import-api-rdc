using System.Collections.Generic;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Model;

namespace SQLDataComparer.UnitTests.UseCaseModels
{
	public static class DocumentUseCaseModels
	{
		private struct OneSide
		{
			public string DocArtifactId;
			public string DocControlNumber;
			public string DocValue;
		}

		public static void MockDocumentHasTheSameValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static void MockDocumentHasDifferentValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val2"
			};

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}
		
		public static void MockDifferentDocumentsWithTheSameValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc2",
				DocValue = "Val1"
			};

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static void MockDifferentDocumentsWithDifferentValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc2",
				DocValue = "Val2"
			};


			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static void MockDocumentIsOnlyOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightTables = GetDocumentTable(rightSide);
			rightTables[0].Rows.RemoveAt(0);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(rightTables
				);
		}

		public static void MockDocumentIsOnlyOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var leftTables = GetDocumentTable(leftSide);
			leftTables[0].Rows.RemoveAt(0);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(leftTables
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static void MockDocumentHasValueOnlyOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = ""
			};

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static void MockDocumentHasValueOnlyOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = ""
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1"
			};

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide)
				);

			dataLoader.Setup(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide)
				);
		}

		public static CompareConfig GetModel()
		{
			return new CompareConfig
			{
				TablesConfig = new[]
				{
					new TableConfig
					{
						Name = "EDDSDBO.Document",
						RowId = "ControlNumber",
						IgnoreConfig = new[]
						{
							new IgnoreConfig
							{
								Name = "ArtifactID"
							}
						}
					}
				}
			};
		}

		private static List<Table> GetDocumentTable(OneSide side)
		{
			var documentTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"ControlNumber", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.Document",
				RowId = "ControlNumber"
			};

			documentTable.Rows = new List<Row>
			{
				new Row(documentTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocArtifactId,
						side.DocControlNumber,
						side.DocValue
					}
				}
			};

			return new List<Table>
			{
				documentTable
			};
		}
	}
}
