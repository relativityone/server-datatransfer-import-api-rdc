// ----------------------------------------------------------------------------
// <copyright file="ProcessPool2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Collections.Concurrent;
	using System.Threading;

	/// <summary>
	/// Represents a class object to manage 1 or more <see cref="IRunnable"/> objects. This class cannot be inherited.
	/// </summary>
	internal sealed class ProcessPool2
	{
		/// <summary>
		/// The dictionary that maps runnable processes to a thread.
		/// </summary>
		private readonly ConcurrentDictionary<Guid, Thread> threadDictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessPool2"/> class.
		/// </summary>
		public ProcessPool2()
		{
			this.threadDictionary = new ConcurrentDictionary<Guid, Thread>();
		}

		/// <summary>
		/// Aborts the process that contains the specified process identifier.
		/// </summary>
		/// <param name="processId">
		/// The process identifier.
		/// </param>
		public void Abort(Guid processId)
		{
			if (this.threadDictionary.TryRemove(processId, out Thread value))
			{
				// Note: the thread has already been removed from the collection.
				value.Abort();
			}
		}

		/// <summary>
		/// Aborts the process that contains the specified process identifier.
		/// </summary>
		/// <param name="processId">
		/// The process identifier.
		/// </param>
		public void Remove(Guid processId)
		{
			if (this.threadDictionary.TryRemove(processId, out Thread value))
			{
				if (value.ThreadState != ThreadState.Stopped)
				{
					value.Abort();
				}
			}
		}

		/// <summary>
		/// Creates a new <see cref="Thread"/> with an <see cref="ApartmentState.STA"/> configuration for the runnable object and returns the unique identifier.
		/// </summary>
		/// <param name="runnable">
		/// The runnable object to start.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> value.
		/// </returns>
		public Guid Start(IRunnable runnable)
		{
			if (runnable == null)
			{
				throw new ArgumentNullException(nameof(runnable));
			}

			Guid key = Guid.NewGuid();
			runnable.ProcessId = key;
			Thread thread = new Thread(runnable.Start);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			this.threadDictionary.TryAdd(key, thread);
			return key;
		}
	}
}