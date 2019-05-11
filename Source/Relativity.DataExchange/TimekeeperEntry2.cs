// ----------------------------------------------------------------------------
// <copyright file="TimekeeperEntry2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	/// <summary>
	/// Represents a single entry stored within <see cref="Timekeeper2"/> that contains metric information. This class cannot be inherited.
	/// </summary>
	public sealed class TimekeeperEntry2
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimekeeperEntry2"/> class.
		/// </summary>
		public TimekeeperEntry2()
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