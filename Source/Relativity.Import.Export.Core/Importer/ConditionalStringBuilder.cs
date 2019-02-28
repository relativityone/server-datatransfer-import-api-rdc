// ----------------------------------------------------------------------------
// <copyright file="ConditionalStringBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Importer
{
	using System.Text;

	/// <summary>
	/// Represents a class object that conditionally builds a string.
	/// </summary>
	internal class ConditionalStringBuilder
	{
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly bool saveData;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalStringBuilder"/> class.
		/// </summary>
		/// <param name="saveData">
		/// <see langword="true" /> to save the data; otherwise, <see langword="false" />.
		/// </param>
		public ConditionalStringBuilder(bool saveData)
		{
			this.saveData = saveData;
		}

		/// <summary>
		/// Appends a string to the end of the string being built.
		/// </summary>
		/// <param name="input">
		/// The string to append.
		/// </param>
		public void Append(string input)
		{
			if (!this.saveData)
			{
				return;
			}

			this.stringBuilder.Append(input);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return this.stringBuilder.ToString();
		}
	}
}