using System.Collections.Generic;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Model;

namespace SQLDataComparer.UnitTests.UseCaseModels
{
	public static class MultiObjectUseCaseModels
	{
		private struct OneSide
		{
			public string DocArtifactId;
			public string DocControlNumber;
			public string DocValue;
			
			public string FirstObjectName;
			public string FirstObjectArtifactId;
			public string FirstObjectValue;
			public string SecondObjectName;
			public string SecondObjectArtifactId;
			public string SecondObjectValue;
			public string ThirdObjectName;
			public string ThirdObjectArtifactId;
			public string ThirdObjectValue;
			public string FourthObjectName;
			public string FourthObjectArtifactId;
			public string FourthObjectValue;
			public string FirstSelectedObjectValueArtifactId;
			public string SecondSelectedObjectValueArtifactId;
		}

		public static void MockDocumentHasMultiObjectSetTheSameWithTheSameValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetTheSameWithDifferentValuesModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val211",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val212",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetTheSameWithFirstObjDifferentValueModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val211",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetTheSameWithSecondObjDifferentValueModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val212",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetTheSameDifferentOrderModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "212",
				SecondSelectedObjectValueArtifactId = "211"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetDifferentlyAllObjectsDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "213",
				SecondSelectedObjectValueArtifactId = "214"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetDifferentlyFirstObjectDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "213",
				SecondSelectedObjectValueArtifactId = "212"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}


		public static void MockDocumentHasMultiObjectSetDifferentlySecondObjectDifferentModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "213"
			};

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetDifferentlyMoreSetOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			var rightTables = GetMultiObjectMappingTable(rightSide);
			rightTables[0].Rows.RemoveAt(1);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(rightTables);
		}


		public static void MockDocumentHasMultiObjectSetDifferentlyMoreSetOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			var leftTables = GetMultiObjectMappingTable(leftSide);
			leftTables[0].Rows.RemoveAt(1);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(leftTables);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(GetMultiObjectMappingTable(rightSide));
		}

		public static void MockDocumentHasMultiObjectSetOnlyOnTheLeftModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			var rightTables = GetMultiObjectMappingTable(rightSide);
			rightTables[0].Rows.RemoveAt(1);
			rightTables[0].Rows.RemoveAt(0);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(GetMultiObjectMappingTable(leftSide));

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
				.Returns(rightTables);
		}

		public static void MockDocumentHasMultiObjectSetOnlyOnTheRightModel(Mock<IDataLoader> dataLoader)
		{
			var leftSide = new OneSide
			{
				DocArtifactId = "1",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "111",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "112",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "113",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "114",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "111",
				SecondSelectedObjectValueArtifactId = "112"
			};

			var rightSide = new OneSide
			{
				DocArtifactId = "2",
				DocControlNumber = "Doc1",
				DocValue = "Val1",
				FirstObjectName = "Obj11",
				FirstObjectArtifactId = "211",
				FirstObjectValue = "Val111",
				SecondObjectName = "Obj12",
				SecondObjectArtifactId = "212",
				SecondObjectValue = "Val112",
				ThirdObjectName = "Obj13",
				ThirdObjectArtifactId = "213",
				ThirdObjectValue = "Val113",
				FourthObjectName = "Obj14",
				FourthObjectArtifactId = "214",
				FourthObjectValue = "Val114",
				FirstSelectedObjectValueArtifactId = "211",
				SecondSelectedObjectValueArtifactId = "212"
			};

			var leftTables = GetMultiObjectMappingTable(leftSide);
			leftTables[0].Rows.RemoveAt(1);
			leftTables[0].Rows.RemoveAt(0);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Left))
				.Returns(GetDocumentTable(leftSide))
				.Returns(GetMutliObjTable(leftSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Left))
				.Returns(leftTables);

			dataLoader.SetupSequence(x => x.GetDataTable(It.IsAny<TableConfig>(), SideEnum.Right))
				.Returns(GetDocumentTable(rightSide))
				.Returns(GetMutliObjTable(rightSide));
			dataLoader.Setup(x => x.GetMappingTable(It.IsAny<TableConfig>(), It.IsAny<MappingConfig>(), It.IsAny<Dictionary<string, string>>(), SideEnum.Right))
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
							}
						},
						MappingsConfig =
						{
							new MappingConfig
							{
								Name = "MultiObj",
								Type = MappingType.MultiObject,
								TargetTable = "EDDSDBO.MultiObj"
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
					Id = side.FirstObjectName,
					Values = new List<string>
					{
						side.FirstObjectArtifactId,
						side.FirstObjectName,
						side.FirstObjectValue
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
					Id = side.SecondObjectName,
					Values = new List<string>
					{
						side.SecondObjectArtifactId,
						side.SecondObjectName,
						side.SecondObjectValue
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
					Id = side.ThirdObjectName,
					Values = new List<string>
					{
						side.ThirdObjectArtifactId,
						side.ThirdObjectName,
						side.ThirdObjectValue
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
					Id = side.FourthObjectValue,
					Values = new List<string>
					{
						side.FourthObjectArtifactId,
						side.FourthObjectName,
						side.FourthObjectValue
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
						side.FirstSelectedObjectValueArtifactId
					}
				},
				new Row(fTable)
				{
					Id = side.DocControlNumber,
					Values = new List<string>
					{
						side.DocControlNumber,
						side.DocArtifactId,
						side.SecondSelectedObjectValueArtifactId
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
