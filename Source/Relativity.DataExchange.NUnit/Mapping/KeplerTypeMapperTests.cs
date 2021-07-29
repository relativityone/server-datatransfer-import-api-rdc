// <copyright file="KeplerTypeMapperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Mapping
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FluentAssertions;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Mapping;
	using kCura.WinEDDS.Service.Replacement;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	using Relativity.DataExchange.NUnit.Utils;

	[TestFixture]
	public class KeplerTypeMapperTests
	{
		private KeplerTypeMapper mapper;

		private RandomObjectGenerator randomObjectGenerator;

		[SetUp]
		public void SetUp()
		{
			this.mapper = new KeplerTypeMapper();
			this.randomObjectGenerator = new RandomObjectGenerator();
		}

		[Test]
		public void AuditManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IAuditManager>();
		}

		[Test]
		public void BulkImportManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IBulkImportManager>();
		}

		[Test]
		public void CaseManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<ICaseManager>();
		}

		[Test]
		public void CodeManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<ICodeManager>();
		}

		[Test]
		public void DocumentManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IDocumentManager>();
		}

		[Test]
		public void ExportManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IExportManager>();
		}

		[Test]
		public void FieldManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IFieldManager>();
		}

		[Test]
		public void FieldQueryModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IFieldQuery>();
		}

		[Test]
		public void FileIoModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IFileIO>();
		}

		[Test]
		public void FolderManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IFolderManager>();
		}

		[Test]
		public void ObjectManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IObjectManager>();
		}

		[Test]
		public void IObjectTypeManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IObjectTypeManager>();
		}

		[Test]
		public void ProductionManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IProductionManager>();
		}

		[Test]
		public void RelativityManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IRelativityManager>();
		}

		[Test]
		public void SearchManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<ISearchManager>();
		}

		[Test]
		public void UserManagerModelShouldBeCorrectlyMapped()
		{
			this.EnsureMapping<IUserManager>();
		}

		private void EnsureMapping<TManager>()
		{
			this.EnsureParameterTypeMapping<TManager>();
			this.EnsureReturnTypeMapping<TManager>();
		}

		private void EnsureParameterTypeMapping<TManager>()
		{
			var parameterTypes = this.GetParameterTypesForManagerMethods<TManager>();

			foreach (var webApiParameterType in parameterTypes)
			{
				var keplerParameterType = Type.GetType($"Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.{webApiParameterType.Name}, Relativity.DataTransfer.Legacy.SDK");

				TestContext.WriteLine($"Map '{webApiParameterType.FullName}' -> '{keplerParameterType?.FullName}'");

				// Arrange - generate object for WebApi parameter type
				var webApiObject = this.randomObjectGenerator.Generate(webApiParameterType);
				Assert.NotNull(webApiObject, $"{webApiParameterType} object should not be null");

				// Act - map WebApi to Kepler object
				var keplerObject = this.mapper.Map(webApiObject, webApiParameterType, keplerParameterType);
				Assert.NotNull(keplerObject, $"{keplerParameterType} object should not be null");

				// Assert - ensure WebApi and Kepler objects are the same
				this.EnsureObjectsAreEqual(webApiObject, keplerObject, keplerParameterType);
			}
		}

		private void EnsureReturnTypeMapping<TManager>()
		{
			var returnTypes = this.GetReturnTypesForManagerMethods<TManager>();

			foreach (var webApiReturnType in returnTypes)
			{
				var keplerReturnType = Type.GetType($"Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.{webApiReturnType.Name}, Relativity.DataTransfer.Legacy.SDK");

				TestContext.WriteLine($"Map '{keplerReturnType?.FullName}' -> '{webApiReturnType.FullName}'");

				// Arrange - generate object for Kepler return type
				var keplerObject = this.randomObjectGenerator.Generate(keplerReturnType);
				Assert.NotNull(keplerObject, $"{keplerReturnType} object should not be null");

				// Act - map Kepler to WebApi object
				var webApiObject = this.mapper.Map(keplerObject, keplerReturnType, webApiReturnType);
				Assert.NotNull(webApiObject, $"{webApiReturnType} object should not be null");

				// Assert - ensure Kepler and WebApi objects are the same
				this.EnsureObjectsAreEqual(keplerObject, webApiObject, webApiReturnType);
			}
		}

		private Type[] GetParameterTypesForManagerMethods<TManager>()
		{
			var parameterTypesToMap = new List<Type>();
			var parameterTypesSkipped = new List<Type>();

			var methods = typeof(TManager).GetMethods();
			foreach (var method in methods)
			{
				var parameters = method.GetParameters();
				foreach (var parameter in parameters)
				{
					if (this.ShouldIncludeType(parameter.ParameterType))
					{
						parameterTypesToMap.Add(parameter.ParameterType);
					}
					else
					{
						parameterTypesSkipped.Add(parameter.ParameterType);
					}
				}
			}

			TestContext.WriteLine($"Skip mapping test for {typeof(TManager).Name} parameter types: {string.Join(",", parameterTypesSkipped.Distinct().Select(t => t.Name))}");

			return parameterTypesToMap.Distinct().ToArray();
		}

		private Type[] GetReturnTypesForManagerMethods<TManager>()
		{
			var returnTypesToMap = new List<Type>();
			var returnTypesSkipped = new List<Type>();

			var managerMethods = typeof(TManager).GetMethods();
			foreach (var method in managerMethods)
			{
				var returnType = method.ReturnType;
				if (this.ShouldIncludeType(returnType))
				{
					returnTypesToMap.Add(returnType);
				}
				else
				{
					returnTypesSkipped.Add(returnType);
				}
			}

			TestContext.WriteLine($"Skip mapping test for {typeof(TManager).Name} return types: {string.Join(",", returnTypesSkipped.Distinct().Select(t => t.Name))}");

			return returnTypesToMap.Distinct().ToArray();
		}

		private void EnsureObjectsAreEqual(object source, object actualDestination, Type destinationType)
		{
			Assert.NotNull(source);
			Assert.NotNull(actualDestination);

			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.NullValueHandling = NullValueHandling.Ignore;
			jsonSettings.Converters.Add(new StringEnumConverter());

			var sourceJson = JsonConvert.SerializeObject(source, jsonSettings);
			var expectedDestination = JsonConvert.DeserializeObject(sourceJson, destinationType, jsonSettings);

			expectedDestination.Should().BeEquivalentTo(actualDestination);
		}

		private bool ShouldIncludeType(Type type)
		{
			return type.FullName.Contains("kCura.EDDS.WebAPI") ||
			       type.FullName.Contains("Relativity.DataExchange.Service") ||
			       type.FullName.Contains("Relativity.DataTransfer.Legacy.SDK");
		}
	}
}
