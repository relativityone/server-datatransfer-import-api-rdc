// ----------------------------------------------------------------------------
// <copyright file="DocumentObjectsDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.Collections.Generic;
	using System.Linq;

	public class DocumentObjectsDto
	{
		public DocumentObjectsDto(string controlNumber, IEnumerable<string> objectsNames)
		{
			this.ControlNumber = controlNumber;
			this.ObjectsNames = RemoveEmptyChoices(objectsNames).ToList();
		}

		public string ControlNumber { get; }

		public List<string> ObjectsNames { get; }

		private static IEnumerable<string> RemoveEmptyChoices(IEnumerable<string> choices) =>
			choices.Where(choice => !string.IsNullOrEmpty(choice));
	}
}
