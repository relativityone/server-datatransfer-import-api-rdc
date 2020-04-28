using System.Collections.Generic;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Model;

namespace SQLDataComparer.UnitTests.UseCaseModels
{
	public static class SingleObjectUseCaseModels
	{
		private struct OneSide
		{
			public string DocArtifactId;
			public string DocControlNumber;
			public string DocValue;

			public string FirstObjectArtifactId;
			public string FirstObjectName;
			public string FirstObjectValue;
			public string SecondObjectArtifactId;
			public string SecondObjectName;
			public string SecondObjectValue;
			public string SelectedObjectArtifactId;
		}

		public static void MockDocumentHasSingleObjTheSameModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "11",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "12",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "11"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "21",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "22",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "21"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasSingleObjDifferentSameValueModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "11",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "12",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "11"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "21",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "22",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "22"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasSingleObjTheSameWithDiffValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "11",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "12",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "11"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "21",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val21",
				SecondObjectArtifactId = "22",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "21"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasSingleObjOnlyOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "11",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "12",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "11"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "21",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "22",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = ""
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasSingleObjOnlyOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "11",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "12",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = ""
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectArtifactId = "21",
				FirstObjectName = "Object1",
				FirstObjectValue = "Val11",
				SecondObjectArtifactId = "22",
				SecondObjectName = "Object2",
				SecondObjectValue = "Val11",
				SelectedObjectArtifactId = "21"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide));

		}
		
		public static CompareConfig GetModel()
		{
			return new CompareConfig
			{
				TablesConfig =
				{
					new TableConfig
					{
						Name = "EDDSDBO.Document",
						RowId = "ControlNumber",
						IgnoreConfig =
						{
							new SingleIgnoreConfig
							{
								Name = "ArtifactID"
							},
							new SingleIgnoreConfig
							{
								Name = "SingleObj"
							}
						},
						MappingsConfig =
						{
							new MappingConfig
							{
								Name = "SingleObj",
								Type = MappingType.SingleObject,
								TargetTable = "EDDSDBO.SingleObj"
							}
						}
					},
					new TableConfig
					{
						Name = "EDDSDBO.SingleObj",
						RowId = "Name",
						IgnoreConfig =
						{
							new SingleIgnoreConfig
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
					{"Value", 2},
					{"SingleObj", 3}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID", "SingleObj"
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
						side.DocValue,
						side.SelectedObjectArtifactId
					}
				}
			};
			
			return new List<Table>
			{
				documentTable
			};
		}

		private static List<Table> GetSingleObjTable(OneSide side)
		{
			var firstSingleObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0},
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.SingleObj",
				RowId = "Name"
			};

			firstSingleObjTable.Rows = new List<Row>
			{
				new Row(firstSingleObjTable)
				{
					Id = side.FirstObjectName,
					Values = new List<string>
					{
						side.FirstObjectArtifactId,
						side.FirstObjectName,
						side.FirstObjectValue
					}
				}
			};

			var secondSingleObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0},
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.SingleObj",
				RowId = "Name"
			};

			secondSingleObjTable.Rows = new List<Row>
			{
				new Row(secondSingleObjTable)
				{
					Id = side.SecondObjectName,
					Values = new List<string>
					{
						side.SecondObjectArtifactId,
						side.SecondObjectName,
						side.SecondObjectValue
					}
				}
			};

			return new List<Table>
			{
				firstSingleObjTable,
				secondSingleObjTable
			};
		}
		private static List<Table> GetSingleObjectMappingTable(OneSide side)
		{
			var fTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ControlNumber", 0},
					{"ArtifactID", 1},
					{"SingleObj", 2}
				},
				Name = "EDDSDBO.Document",
				RowId = "ControlNumber"
			};

			fTable.Rows = new List<Row>
			{
				new Row(fTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocControlNumber,
						side.DocArtifactId,
						side.SelectedObjectArtifactId
					}
				}
			};

			return new List<Table>
			{
				fTable
			};
		}
	}
}
