using System.Collections.Generic;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Model;

namespace SQLDataComparer.UnitTests.UseCaseModels
{
	public static class MultiChoiceUseCaseModels
	{
		private struct OneSide
		{
			public string DocArtifactId;
			public string DocControlNumber;
			public string DocValue;

			public string FirstChoiceValueArtifactId;
			public string FirstChoiceValueName;
			public string SecondChoiceValueArtifactId;
			public string SecondChoiceValueName;
			public string ThirdChoiceValueArtifactId;
			public string ThirdChoiceValueName;
			public string FourthChoiceValueArtifactId;
			public string FourthChoiceValueName;

			public string FirstSelectedChoiceValueArtifactId;
			public string SecondSelectedChoiceValueArtifactId;
		}

		public static void MockDocumentHasMultiChoiceSetTheSameModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "222"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}

		public static void MockDocumentHasMultiChoiceSetTheSameDifferentOrderModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "222",
				SecondSelectedChoiceValueArtifactId = "221"
			};
			
			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}

		public static void MockDocumentHasMultiChoiceSetDifferentlyAllChoicesDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "223",
				SecondSelectedChoiceValueArtifactId = "224"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}

		public static void MockDocumentHasMultiChoiceSetDifferentlyFirstChoiceDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "223",
				SecondSelectedChoiceValueArtifactId = "222"
			};
			
			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}


		public static void MockDocumentHasMultiChoiceSetDifferentlySecondChoiceDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "223"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}

		public static void MockDocumentHasMultiChoiceSetDifferentlyMoreSetOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "222"
			};

			var rightTables = GetZCodeArtifactTable(rightSide);
			rightTables[0].Rows.RemoveAt(1);


			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(rightTables);
		}


		public static void MockDocumentHasMultiChoiceSetDifferentlyMoreSetOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "222"
			};

			var leftTables = GetZCodeArtifactTable(leftSide);
			leftTables[0].Rows.RemoveAt(1);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(leftTables);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
		}

		public static void MockDocumentHasMultiChoiceSetOnlyOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "222"
			};
			
			var rightTables = GetZCodeArtifactTable(rightSide);
			rightTables[0].Rows.RemoveAt(1);
			rightTables[0].Rows.RemoveAt(0);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetCodeTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetZCodeArtifactTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetCodeTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(rightTables);
		}

		public static void MockDocumentHasMultiChoiceSetOnlyOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "111",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "112",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "113",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "114",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "111",
				SecondSelectedChoiceValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstChoiceValueArtifactId = "221",
				FirstChoiceValueName = "Privileged",
				SecondChoiceValueArtifactId = "222",
				SecondChoiceValueName = "Not Privileged",
				ThirdChoiceValueArtifactId = "223",
				ThirdChoiceValueName = "Further Review",
				FourthChoiceValueArtifactId = "224",
				FourthChoiceValueName = "Attorney Only",
				FirstSelectedChoiceValueArtifactId = "221",
				SecondSelectedChoiceValueArtifactId = "222"
			};

			var leftTables = GetZCodeArtifactTable(leftSide);
			leftTables[0].Rows.RemoveAt(1);
			leftTables[0].Rows.RemoveAt(0);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetCodeTable(leftSide))
				.Returns(GetDocumentTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string,string>>(), SideEnum.Left))
				.Returns(leftTables);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetCodeTable(rightSide))
				.Returns(GetDocumentTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetZCodeArtifactTable(rightSide));
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
						},
						MappingsConfig = new[]
						{
							new MappingConfig
							{
								Name = "MultiChoice",
								Type = MappingType.MultiChoice,
								TargetTable = "EDDSDBO.Code"
							}
						}
					},
					new TableConfig
					{
						Name = "EDDSDBO.Code",
						RowId = "Name",
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

		private static List<Table> GetCodeTable(OneSide side)
		{
			var firstCodeTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.Code",
				RowId = "Name"
			};

			firstCodeTable.Rows = new List<Row>
			{
				new Row(firstCodeTable)
				{
					Id = side.FirstChoiceValueName,
					Values = new List<string>
					{
						side.FirstChoiceValueArtifactId,
						side.FirstChoiceValueName
					}
				}
			};

			var secondCodeTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.Code",
				RowId = "Name"
			};

			secondCodeTable.Rows = new List<Row>
			{
				new Row(secondCodeTable)
				{
					Id = side.SecondChoiceValueName,
					Values = new List<string>
					{
						side.SecondChoiceValueArtifactId,
						side.SecondChoiceValueName
					}
				}
			};

			var thirdCodeTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.Code",
				RowId = "Name"
			};

			thirdCodeTable.Rows = new List<Row>
			{
				new Row(thirdCodeTable)
				{
					Id = side.ThirdChoiceValueName,
					Values = new List<string>
					{
						side.ThirdChoiceValueArtifactId,
						side.ThirdChoiceValueName
					}
				}
			};

			var fourthCodeTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ArtifactID", 0 },
					{"Name", 1}
				},
				Ignores = new HashSet<string>
				{
					"ArtifactID"
				},
				Name = "EDDSDBO.Code",
				RowId = "Name"
			};

			fourthCodeTable.Rows = new List<Row>
			{
				new Row(fourthCodeTable)
				{
					Id = side.FourthChoiceValueName,
					Values = new List<string>
					{
						side.FourthChoiceValueArtifactId,
						side.FourthChoiceValueName
					}
				}
			};

			return new List<Table>
			{
				firstCodeTable,
				secondCodeTable,
				thirdCodeTable,
				fourthCodeTable
			};
		}

		private static List<Table> GetZCodeArtifactTable(OneSide side)
		{
			var zCodeArtifactTable = new Table
			{
				Columns = new Dictionary<string, int>
				{
					{"ControlNumber", 0},
					{"CodeArtifactID", 1},
					{"AssociatedArtifactID", 2}
				},
				Name = "EDDSDBO.ZCodeArtifact_{choiceArtifactID}",
				RowId = "ControlNumber"
			};

			zCodeArtifactTable.Rows = new List<Row>
			{
				new Row(zCodeArtifactTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocControlNumber,
						side.FirstSelectedChoiceValueArtifactId,
						side.DocArtifactId
					}
				},
				new Row(zCodeArtifactTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocControlNumber,
						side.SecondSelectedChoiceValueArtifactId,
						side.DocArtifactId
					}
				}
			};

			return new List<Table>
			{
				zCodeArtifactTable
			};
		}
	}
}
