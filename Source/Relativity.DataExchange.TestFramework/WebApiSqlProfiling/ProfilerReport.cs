// <copyright file="ProfilerReport.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling
{
	using System.Collections.Generic;
	using System.Linq;

	public class ProfilerReport
	{
		public ProfilerReport(string description, IEnumerable<TextFileDto> files)
		{
			this.Description = description;
			this.Files = files;
		}

		public ProfilerReport(string description)
			: this(description, Enumerable.Empty<TextFileDto>())
		{
		}

		public string Description { get; }

		public IEnumerable<TextFileDto> Files { get; }
	}
}
