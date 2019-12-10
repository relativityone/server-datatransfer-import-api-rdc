// <copyright file="TextFileDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling
{
	public class TextFileDto
	{
		public TextFileDto(string name, string extension, string content)
		{
			this.Name = name;
			this.Extension = extension;
			this.Content = content;
		}

		public string Name { get; }

		public string Extension { get; }

		public string Content { get; }
	}
}
