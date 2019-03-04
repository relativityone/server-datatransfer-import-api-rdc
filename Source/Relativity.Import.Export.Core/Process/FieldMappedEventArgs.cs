// ----------------------------------------------------------------------------
// <copyright file="FieldMappedEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the field mapped event argument data.
	/// </summary>
	[Serializable]
	public sealed class FieldMappedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FieldMappedEventArgs"/> class.
		/// </summary>
		/// <param name="sourceField">
		/// The source field name.
		/// </param>
		/// <param name="targetField">
		/// The target field name.
		/// </param>
		public FieldMappedEventArgs(string sourceField, string targetField)
		{
			this.SourceField = sourceField;
			this.TargetField = targetField;
		}

		/// <summary>
		/// Gets the source field name.
		/// </summary>
		/// <value>
		/// The field name.
		/// </value>
		public string SourceField
		{
			get;
		}

		/// <summary>
		/// Gets the target field name.
		/// </summary>
		/// <value>
		/// The field name.
		/// </value>
		public string TargetField
		{
			get;
		}
	}
}