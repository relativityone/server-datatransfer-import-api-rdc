// -----------------------------------------------------------------------------------------------------
// <copyright file="ArtifactTypeHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ArtifactTypeHelper"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.Import.Export.Services;

	[TestFixture]
	public static class ArtifactTypeHelperTests
	{
		[TestCase(ArtifactType.System, false)]
		[TestCase(ArtifactType.User, false)]
		[TestCase(ArtifactType.Group, false)]
		[TestCase(ArtifactType.View, false)]
		[TestCase(ArtifactType.Client, false)]
		[TestCase(ArtifactType.Matter, false)]
		[TestCase(ArtifactType.Code, false)]
		[TestCase(ArtifactType.Case, false)]
		[TestCase(ArtifactType.Folder, false)]
		[TestCase(ArtifactType.Document, true)]
		[TestCase(ArtifactType.Field, false)]
		[TestCase(ArtifactType.Search, false)]
		[TestCase(ArtifactType.Layout, false)]
		[TestCase(ArtifactType.Production, true)]
		[TestCase(ArtifactType.Error, false)]
		[TestCase(ArtifactType.Report, false)]
		[TestCase(ArtifactType.Agent, false)]
		[TestCase(ArtifactType.Sync, false)]
		[TestCase(ArtifactType.MarkupSet, true)]
		[TestCase(ArtifactType.Tab, false)]
		[TestCase(ArtifactType.BatchSet, false)]
		[TestCase(ArtifactType.ObjectType, false)]
		[TestCase(ArtifactType.SearchContainer, false)]
		[TestCase(ArtifactType.Batch, false)]
		[TestCase(ArtifactType.RelativityScript, false)]
		[TestCase(ArtifactType.SearchContainer, false)]
		[TestCase(ArtifactType.Batch, false)]
		[TestCase(ArtifactType.RelativityScript, false)]
		[TestCase(ArtifactType.SearchProvider, false)]
		[TestCase(ArtifactType.ResourceFile, false)]
		[TestCase(ArtifactType.ResourceGroup, false)]
		[TestCase(ArtifactType.ResourceServer, false)]
		[TestCase(ArtifactType.ObjectRule, false)]
		[TestCase(ArtifactType.LibraryApplication, false)]
		[TestCase(ArtifactType.AgentType, false)]
		[TestCase(ArtifactType.WorkspaceApplication, false)]
		[TestCase(ArtifactType.ApplicationInstall, false)]
		[TestCase(ArtifactType.License, false)]
		[TestCase(ArtifactType.InstallEventHandler, true)]
		[TestCase(ArtifactType.MassOperation, true)]
		[TestCase(ArtifactType.InstanceSetting, false)]
		[TestCase(ArtifactType.Credential, false)]
		[TestCase(ArtifactType.VirtualField, false)]
		[TestCase(ArtifactType.EventHandler, false)]
		[TestCase(ArtifactType.History, false)]
		public static void ShouldDetermineIfTheArtifactTypeIsDynamic(ArtifactType type, bool expected)
		{
			bool actual = ArtifactTypeHelper.IsDynamic((int)type);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}