using System.Collections.Generic;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Model;

namespace SQLDataComparer.UnitTests.UseCaseModels
{
	public static class SingleAndMultiObjectUseCaseModels
	{
		private struct OneSide
		{
			public string DocArtifactId;
			public string DocControlNumber;
			public string DocValue;

			public string FirstMultiObjectName;
			public string FirstMultiObjectArtifactId;
			public string FirstMultiObjectValue;
			public string SecondMultiObjectName;
			public string SecondMultiObjectArtifactId;
			public string SecondMultiObjectValue;
			public string ThirdMultiObjectName;
			public string ThirdMultiObjectArtifactId;
			public string ThirdMultiObjectValue;
			public string FourthMultiObjectName;
			public string FourthMultiObjectArtifactId;
			public string FourthMultiObjectValue;
			public string FirstSelectedMultiObjectValueArtifactId;
			public string SecondSelectedMultiObjectValueArtifactId;
			
			public string FirstSingleObjectArtifactId;
			public string FirstSingleObjectName;
			public string FirstSingleObjectValue;
			public string SecondSingleObjectArtifactId;
			public string SecondSingleObjectName;
			public string SecondSingleObjectValue;
			public string SelectedSingleObjectArtifactId;
		}

		public static void MockDocumentHasSingleAndMultiObjectSetTheSameWithTheSameValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstMultiObjectName = "Obj11",
				FirstMultiObjectArtifactId = "111",
				FirstMultiObjectValue = "Val111",
				SecondMultiObjectName = "Obj12",
				SecondMultiObjectArtifactId = "112",
				SecondMultiObjectValue = "Val112",
				ThirdMultiObjectName = "Obj13",
				ThirdMultiObjectArtifactId = "113",
				ThirdMultiObjectValue = "Val113",
				FourthMultiObjectName = "Obj14",
				FourthMultiObjectArtifactId = "114",
				FourthMultiObjectValue = "Val114",
				FirstSelectedMultiObjectValueArtifactId = "111",
				SecondSelectedMultiObjectValueArtifactId = "112",
				FirstSingleObjectArtifactId = "11",
				FirstSingleObjectName = "Object1",
				FirstSingleObjectValue = "Val11",
				SecondSingleObjectArtifactId = "12",
				SecondSingleObjectName = "Object2",
				SecondSingleObjectValue = "Val11",
				SelectedSingleObjectArtifactId = "11"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstMultiObjectName = "Obj11",
				FirstMultiObjectArtifactId = "211",
				FirstMultiObjectValue = "Val111",
				SecondMultiObjectName = "Obj12",
				SecondMultiObjectArtifactId = "212",
				SecondMultiObjectValue = "Val112",
				ThirdMultiObjectName = "Obj13",
				ThirdMultiObjectArtifactId = "213",
				ThirdMultiObjectValue = "Val113",
				FourthMultiObjectName = "Obj14",
				FourthMultiObjectArtifactId = "214",
				FourthMultiObjectValue = "Val114",
				FirstSelectedMultiObjectValueArtifactId = "211",
				SecondSelectedMultiObjectValueArtifactId = "212",
				FirstSingleObjectArtifactId = "21",
				FirstSingleObjectName = "Object1",
				FirstSingleObjectValue = "Val11",
				SecondSingleObjectArtifactId = "22",
				SecondSingleObjectName = "Object2",
				SecondSingleObjectValue = "Val11",
				SelectedSingleObjectArtifactId = "21"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetSingleObjTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.SetupSequence(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetSingleObjectMappingTable(leftSide))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetSingleObjTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.SetupSequence(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetSingleObjectMappingTable(rightSide))
				.Returns(GetMultiObjectMappingTable(rightSide));
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
								Name = "MultiObj",
								Type = MappingType.MultiObject,
								TargetTable = "EDDSDBO.MultiObj"
							},
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
					},
					new TableConfig
					{
						Name = "EDDSDBO.MultiObj",
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
						side.SelectedSingleObjectArtifactId
					}
				}
			};

			return new List<Table>
			{
				documentTable
			};
		}

		private static List<Table> GetMutliObjTable(OneSide side)
		{
			var firstMultiObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.MultiObj",
				RowId = "Name"
			};

			firstMultiObjTable.Rows = new List<Row>
			{
				new Row(firstMultiObjTable)
				{
					Id = side.FirstMultiObjectName,
					Values = new List<string>
					{
						side.FirstMultiObjectArtifactId,
						side.FirstMultiObjectName,
						side.FirstMultiObjectValue
					}
				}
			};

			var secondMultiObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.MultiObj",
				RowId = "Name"
			};

			secondMultiObjTable.Rows = new List<Row>
			{
				new Row(secondMultiObjTable)
				{
					Id = side.SecondMultiObjectName,
					Values = new List<string>
					{
						side.SecondMultiObjectArtifactId,
						side.SecondMultiObjectName,
						side.SecondMultiObjectValue
					}
				}
			};

			var thirdMultiObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.MultiObj",
				RowId = "Name"
			};

			thirdMultiObjTable.Rows = new List<Row>
			{
				new Row(thirdMultiObjTable)
				{
					Id = side.ThirdMultiObjectName,
					Values = new List<string>
					{
						side.ThirdMultiObjectArtifactId,
						side.ThirdMultiObjectName,
						side.ThirdMultiObjectValue
					}
				}
			};

			var fourthMultiObjTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1},
					{"Value", 2}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.MultiObj",
				RowId = "Name"
			};

			fourthMultiObjTable.Rows = new List<Row>
			{
				new Row(fourthMultiObjTable)
				{
					Id = side.FourthMultiObjectValue,
					Values = new List<string>
					{
						side.FourthMultiObjectArtifactId,
						side.FourthMultiObjectName,
						side.FourthMultiObjectValue
					}
				}
			};

			return new List<Table>
			{
				firstMultiObjTable,
				secondMultiObjTable,
				thirdMultiObjTable,
				fourthMultiObjTable
			};
		}

		private static List<Table> GetMultiObjectMappingTable(OneSide side)
		{
			var fTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ControlNumber", 0},
					{"f{docArtifactID}ArtifactID", 1},
					{"f{objectArtifactID}ArtifactID", 2}
				},
				Name = "EDDSDBO.f{docArtifactID}f{objectArtifactID}",
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
						side.FirstSelectedMultiObjectValueArtifactId
					}
				},
				new Row(fTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocControlNumber,
						side.DocArtifactId,
						side.SecondSelectedMultiObjectValueArtifactId
					}
				}
			};

			return new List<Table>
			{
				fTable
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
					Id = side.FirstSingleObjectName,
					Values = new List<string>
					{
						side.FirstSingleObjectArtifactId,
						side.FirstSingleObjectName,
						side.FirstSingleObjectValue
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
					Id = side.SecondSingleObjectName,
					Values = new List<string>
					{
						side.SecondSingleObjectArtifactId,
						side.SecondSingleObjectName,
						side.SecondSingleObjectValue
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
						side.SelectedSingleObjectArtifactId
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
