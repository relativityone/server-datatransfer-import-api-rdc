// ----------------------------------------------------------------------------
// <copyright file="WellKnownFieldRequests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System.Collections.Generic;

	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;

	public static class WellKnownFieldRequests
	{
		/// <summary>
		/// Gets request that can be used to update identifier field in workspace to be compatible with our integration tests.
		/// </summary>
		public static FixedLengthFieldRequest IdentifierFieldRequest =>
			new FixedLengthFieldRequest()
				{
					Name = WellKnownFields.ControlNumber,
					ObjectType = new ObjectTypeIdentifier() { Name = WellKnownArtifactTypes.DocumentArtifactTypeName },
					Length = 255,
					IsRequired = true,
					IncludeInTextIndex = true,
					FilterType = FilterType.TextBox,
					AllowSortTally = true,
					AllowGroupBy = false,
					AllowPivot = false,
					HasUnicode = true,
					OpenToAssociations = false,
					IsRelational = false,
					AllowHtml = false,
					IsLinked = true,
					Wrapping = true,
				};

		/// <summary>
		/// Gets list of requests that can be used to create or update all non system fields used in our integration tests.
		/// </summary>
		public static List<BaseFieldRequest> NonSystemFieldRequests =>
			new List<BaseFieldRequest>
				{
					new SingleChoiceFieldRequest
						{
							Name = WellKnownFields.ConfidentialDesignation,
							ObjectType = new ObjectTypeIdentifier { Name = WellKnownArtifactTypes.DocumentArtifactTypeName },
							IsRequired = false,
							AvailableInFieldTree = true,
							FilterType = FilterType.MultiList,
							AllowSortTally = true,
							AllowPivot = true,
							AllowGroupBy = true,
							HasUnicode = true,
							OpenToAssociations = false,
							Wrapping = true,
							AutoAddChoices = true,
							IsLinked = false,
						},
					new MultipleChoiceFieldRequest
						{
							Name = WellKnownFields.PrivilegeDesignation,
							ObjectType = new ObjectTypeIdentifier { Name = WellKnownArtifactTypes.DocumentArtifactTypeName },
							IsRequired = false,
							AvailableInFieldTree = true,
							FilterType = FilterType.MultiList,
							AllowSortTally = true,
							AllowPivot = true,
							AllowGroupBy = true,
							HasUnicode = true,
							OpenToAssociations = false,
							Wrapping = true,
							AutoAddChoices = true,
							IsLinked = false,
						},
					new FixedLengthFieldRequest
						{
							Name = WellKnownFields.FileName,
							ObjectType = new ObjectTypeIdentifier { Name = WellKnownArtifactTypes.DocumentArtifactTypeName },
							Length = 255,
							IsRequired = false,
							IncludeInTextIndex = false,
							FilterType = FilterType.TextBox,
							AllowSortTally = true,
							AllowPivot = false,
							AllowGroupBy = false,
							HasUnicode = true,
							OpenToAssociations = false,
							IsRelational = false,
							AllowHtml = false,
							Wrapping = true,
							IsLinked = false,
						},
				};
	}
}