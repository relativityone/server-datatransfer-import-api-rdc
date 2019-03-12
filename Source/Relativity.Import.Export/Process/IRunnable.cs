// ----------------------------------------------------------------------------
// <copyright file="IRunnable.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents an abstract object that performs some runnable process.
	/// </summary>
	public interface IRunnable
	{
		/// <summary>
		/// Gets or sets the process unique identifier. A new <see cref="Guid"/> is assigned by default.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		Guid ProcessId
		{
			get;
			set;
		}

		/// <summary>
		/// Starts the runnable process.
		/// </summary>
		void Start();
	}
}