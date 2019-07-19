// <copyright file="IErrorArguments.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	using System.Collections.Generic;

	public interface IErrorArguments
	{
		IEnumerable<string> ValuesForErrorFile();
	}
}
