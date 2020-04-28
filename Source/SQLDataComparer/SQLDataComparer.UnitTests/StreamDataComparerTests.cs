using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLDataComparer.Config;
using SQLDataComparer.ConfigCheck;
using SQLDataComparer.DataCompare;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Log;
using SQLDataComparer.Model;
using SQLDataComparer.UnitTests.UseCaseModels;

namespace SQLDataComparer.UnitTests
{
	[TestClass]
	public class StreamDataComparerTests
	{
		private readonly StreamDataComparer _dataComparer;

		private readonly Mock<ILog> _log;
		private readonly Mock<IConfigChecker> _configChecker;
		private readonly Mock<IDataLoader> _dataLoader;

		public StreamDataComparerTests()
		{
			_log = new Mock<ILog>();
			_configChecker = new Mock<IConfigChecker>();
			_dataLoader = new Mock<IDataLoader>();

			_dataComparer = new StreamDataComparer(_log.Object, _configChecker.Object, _dataLoader.Object);
		}

		#region Use cases

		// Case 1
		// Document has different values on the left and right
		// Left:  Doc1->Val1
		// Right: Doc1->Val2
		[TestMethod]
		public void DocumentHasTheSameValues()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentHasTheSameValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 2
		// Document has different values on the left and right
		// Left:  Doc1->Val1
		// Right: Doc1->Val2
		[TestMethod]
		public void DocumentHasDifferentValues()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentHasDifferentValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		
		// Case 3
		// Different documents with the same values
		// Left:  Doc1->Val1
		// Right: Doc2->Val1
		[TestMethod]
		public void DifferentDocumentsWithTheSameValues()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDifferentDocumentsWithTheSameValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}
		
		// Case 4
		// Different documents with the same values
		// Left:  Doc1->Val1
		// Right: Doc2->Val1
		[TestMethod]
		public void DifferentDocumentsWithDifferentValues()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDifferentDocumentsWithDifferentValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}
		
		// Case 5
		// Document is only on the left
		// Left:  Doc1->Val1
		// Right: NULL
		[TestMethod]
		public void DocumentIsOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentIsOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 6
		// Document is only on the right
		// Left:  NULL
		// Right: Doc1
		[TestMethod]
		public void DocumentIsOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentIsOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 7
		// Document has value only on the left
		// Left:  Doc1->Val1
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasValueOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentHasValueOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 8
		// Document has value only on the right
		// Left:  Doc1->Val1
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasValueOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = DocumentUseCaseModels.GetModel();
			DocumentUseCaseModels.MockDocumentHasValueOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 9
		// Document has single object set the same on the left and right
		// Left:  Doc1->SingleObj1->Val1
		// Right: Doc1->SingleObj1->Val1
		[TestMethod]
		public void DocumentHasSingleObjTheSame()
		{
			//arrange
			CompareConfig config = SingleObjectUseCaseModels.GetModel();
			SingleObjectUseCaseModels.MockDocumentHasSingleObjTheSameModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(4, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}
		
		// Case 10
		// Document has single object set the same on the left and right
		// Left:  Doc1->SingleObj1->Val1
		// Right: Doc1->SingleObj2->Val1
		[TestMethod]
		public void DocumentHasSingleObjDifferentSameValue()
		{
			//arrange
			CompareConfig config = SingleObjectUseCaseModels.GetModel();
			SingleObjectUseCaseModels.MockDocumentHasSingleObjDifferentSameValueModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 11
		// Document has single object set the same on the left and right but with different values
		// Left:  Doc1->SingleObj1->Val1
		// Right: Doc1->SingleObj1->Val2
		[TestMethod]
		public void DocumentHasSingleObjTheSameWithDiffValues()
		{
			//arrange
			CompareConfig config = SingleObjectUseCaseModels.GetModel();
			SingleObjectUseCaseModels.MockDocumentHasSingleObjTheSameWithDiffValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 12
		// Document has single object set only on the left
		// Left:  Doc1->SingleObj1->Val1
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasSingleObjOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = SingleObjectUseCaseModels.GetModel();
			SingleObjectUseCaseModels.MockDocumentHasSingleObjOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 13
		// Document has single object set only on the right
		// Left:  Doc1->NULL
		// Right: Doc1->SingleObj1->Val1
		[TestMethod]
		public void DocumentHasSingleObjOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = SingleObjectUseCaseModels.GetModel();
			SingleObjectUseCaseModels.MockDocumentHasSingleObjOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 14
		// Document has single choice set the same on the left and on the right
		// Left:  Doc1->SingleChoice1->Val1
		// Right: Doc1->SingleChoice1->Val1
		[TestMethod]
		public void DocumentHasSingleChoiceSetTheSame()
		{
			//arrange
			CompareConfig config = SingleChoiceUseCaseModels.GetModel();
			SingleChoiceUseCaseModels.MockDocumentHasSingleChoiceSetTheSameModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(4, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 15
		// Document has single choice set differently on the left and on the right
		// Left:  Doc1->SingleChoice1->Val1
		// Right: Doc1->SingleChoice1->Val2
		[TestMethod]
		public void DocumentHasSingleChoiceSetDifferently()
		{
			//arrange
			CompareConfig config = SingleChoiceUseCaseModels.GetModel();
			SingleChoiceUseCaseModels.MockDocumentHasSingleChoiceSetDifferentlyModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 16
		// Document has single choice set only on the left
		// Left:  Doc1->SingleChoice1->Val1
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasSingleChoiceSetOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = SingleChoiceUseCaseModels.GetModel();
			SingleChoiceUseCaseModels.MockDocumentHasSingleChoiceSetOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 17
		// Document has single choice set only on the right
		// Left:  Doc1->NULL
		// Right: Doc1->SingleChoice1->Val1
		[TestMethod]
		public void DocumentHasSingleChoiceSetOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = SingleChoiceUseCaseModels.GetModel();
			SingleChoiceUseCaseModels.MockDocumentHasSingleChoiceSetOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(3, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 18
		// Document has multi choice set the same on the left and on the right
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->MultiChoice1->Val1,Val2
		[TestMethod]
		public void DocumentHasMultiChoiceSetTheSame()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetTheSameModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(6, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 19
		// Document has multi choice set the same on the left and on the right but with different order
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->MultiChoice1->Val2,Val1
		// TODO : This case is marked as false difference for now
		[TestMethod]
		public void DocumentHasMultiChoiceSetTheSameDifferentOrder()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetTheSameDifferentOrderModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(6, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 20
		// Document has multi choice set differently on the left and on the right (all choices different)
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->MultiChoice1->Val3,Val4
		[TestMethod]
		public void DocumentHasMultiChoiceSetDifferentlyAllChoicesDifferent()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetDifferentlyAllChoicesDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 21
		// Document has multi choice set differently on the left and on the right (first choice different)
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->MultiChoice1->Val3,Val2
		[TestMethod]
		public void DocumentHasSingleChoiceSetDifferentlyFirstChoiceDifferent()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetDifferentlyFirstChoiceDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 22
		// Document has multi choice set differently on the left and on the right (second choice different)
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->MultiChoice1->Val1,Val3
		[TestMethod]
		public void DocumentHasMultiChoiceSetDifferentlySecondChoiceDifferent()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetDifferentlySecondChoiceDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 23
		// Document has multi choice set differently on the left and on the right (more choices on the left)
		// Left:  Doc1->MultiChoice1->Val1,Val2,
		// Right: Doc1->MultiChoice1->Val1
		[TestMethod]
		public void DocumentHasMultiChoiceSetDifferentlyMoreSetOnTheLeft()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetDifferentlyMoreSetOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 24
		// Document has multi choice set differently on the left and on the right (more choices on the right)
		// Left:  Doc1->MultiChoice1->Val1
		// Right: Doc1->MultiChoice1->Val1,Val2
		[TestMethod]
		public void DocumentHasMultiChoiceSetDifferentlyMoreSetOnTheRight()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetDifferentlyMoreSetOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 25
		// Document has multi choice set only on the left
		// Left:  Doc1->MultiChoice1->Val1,Val2
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasMultiChoiceSetOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 26
		// Document has multi choice set only on the right
		// Left:  Doc1->NULL
		// Right: Doc1->MultiChoice1->Val1,Val2
		[TestMethod]
		public void DocumentHasMultiChoiceSetOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = MultiChoiceUseCaseModels.GetModel();
			MultiChoiceUseCaseModels.MockDocumentHasMultiChoiceSetOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 27
		// Document has multi object set the same on the left and on the right with the same values
		// Left:  Doc1->Object1->Val1, Object2->Val2
		// Right: Doc1->Object1->Val1, Object2->Val2
		[TestMethod]
		public void DocumentHasMultiObjectSetTheSameWithTheSameValues()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetTheSameWithTheSameValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(6, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 28
		// Document has multi object set the same on the left and on the right with different values
		// Left:  Doc1->Object1->Val1, Object2->Val2
		// Right: Doc1->Object1->Val3, Object2->Val4
		[TestMethod]
		public void DocumentHasMultiObjectSetTheSameWithDifferentValues()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetTheSameWithDifferentValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(4, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(2, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 29
		// Document has multi object set the same on the left and on the right with first object having different value
		// Left:  Doc1->Object1->Val1, Object2->Val2
		// Right: Doc1->Object1->Val3, Object2->Val2
		[TestMethod]
		public void DocumentHasMultiObjectSetTheSameWithFirstObjDifferentValue()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetTheSameWithFirstObjDifferentValueModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 30
		// Document has multi object set the same on the left and on the right with second object having different value
		// Left:  Doc1->Object1->Val1, Object2->Val2
		// Right: Doc1->Object1->Val1, Object2->Val3
		[TestMethod]
		public void DocumentHasMultiObjectSetTheSameWithSecondObjDifferentValue()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetTheSameWithSecondObjDifferentValueModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 31
		// Document has multi object set the same on the left and on the right but with different order
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->Object2, Object1
		// TODO : This case is marked as false difference for now
		[TestMethod]
		public void DocumentHasMultiObjectSetTheSameDifferentOrder()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetTheSameDifferentOrderModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(6, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 32
		// Document has multi object set differently on the left and on the right (all objects different)
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->Object3, Object4
		[TestMethod]
		public void DocumentHasMultiObjectSetDifferentlyAllObjectsDifferent()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetDifferentlyAllObjectsDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 33
		// Document has multi object set differently on the left and on the right (first object different)
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->Object3, Object2
		[TestMethod]
		public void DocumentHasMultiObjectSetDifferentlyFirstObjectDifferent()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetDifferentlyFirstObjectDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 34
		// Document has multi object set differently on the left and on the right (second object different)
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->Object1, Object3
		[TestMethod]
		public void DocumentHasMultiObjectSetDifferentlySecondObjectDifferent()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetDifferentlySecondObjectDifferentModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 35
		// Document has multi choice object differently on the left and on the right (more objects on the left)
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->Object1,
		[TestMethod]
		public void DocumentHasMultiObjectSetDifferentlyMoreSetOnTheLeft()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetDifferentlyMoreSetOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 36
		// Document has multi object set differently on the left and on the right (more objects on the right)
		// Left:  Doc1->Object1,
		// Right: Doc1->Object1, Object2
		[TestMethod]
		public void DocumentHasMultiObjectSetDifferentlyMoreSetOnTheRight()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetDifferentlyMoreSetOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 37
		// Document has multi object set only on the left
		// Left:  Doc1->Object1, Object2
		// Right: Doc1->NULL
		[TestMethod]
		public void DocumentHasMultiObjectSetOnlyOnTheLeft()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetOnlyOnTheLeftModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 38
		// Document has multi object set only on the right
		// Left:  Doc1->NULL
		// Right: Doc1->Object1, Object2
		[TestMethod]
		public void DocumentHasMultiObjectSetOnlyOnTheRight()
		{
			//arrange
			CompareConfig config = MultiObjectUseCaseModels.GetModel();
			MultiObjectUseCaseModels.MockDocumentHasMultiObjectSetOnlyOnTheRightModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(5, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(1, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}

		// Case 39
		// Document has multi object set the same on the left and on the right with the same values
		// Left:  Doc1->Object1->Val1, Object2->Val2
		// Right: Doc1->Object1->Val1, Object2->Val2
		[TestMethod]
		public void DocumentHasSingleAndMultiObjectSetTheSameWithTheSameValues()
		{
			//arrange
			CompareConfig config = SingleAndMultiObjectUseCaseModels.GetModel();
			SingleAndMultiObjectUseCaseModels.MockDocumentHasSingleAndMultiObjectSetTheSameWithTheSameValuesModel(_dataLoader);
			_configChecker.Setup(x => x.CheckTableConfig(It.IsAny<TableConfig>()))
				.Returns(true);

			//act
			List<ComparisonResult> comparisonResults = _dataComparer.CompareData(config).ToList();

			//assert
			Assert.AreEqual(9, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Identical));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.Different));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.LeftOnly));
			Assert.AreEqual(0, comparisonResults.Count(x => x.Result == ComparisonResultEnum.RightOnly));
		}
		#endregion
	}
}