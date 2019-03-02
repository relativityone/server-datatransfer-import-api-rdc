// ----------------------------------------------------------------------------
// <copyright file="TimekeeperEntry.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	/// <summary>
	/// Represents a single entry stored within <see cref="Timekeeper"/> that contains metric information.
	/// </summary>
	public class TimekeeperEntry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimekeeperEntry"/> class.
		/// </summary>
		public TimekeeperEntry()
		{
			this.Count = 0;
			this.Length = 0L;
			this.StartTime = System.DateTime.Now.Ticks;
		}

		/// <summary>
		/// Gets or sets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the operation length in ticks.
		/// </summary>
		/// <value>
		/// The total number of ticks.
		/// </value>
		public long Length
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the operation start time in ticks.
		/// </summary>
		/// <value>
		/// The total number of ticks.
		/// </value>
		public long StartTime
		{
			get;
			set;
		}
	}
}