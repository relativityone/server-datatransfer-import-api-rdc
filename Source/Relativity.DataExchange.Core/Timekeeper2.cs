// ----------------------------------------------------------------------------
// <copyright file="Timekeeper2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections.Concurrent;
	using System.IO;
	using System.Security;
	using System.Text;

	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents a class object that tracks operational metrics. This class cannot be inherited.
	/// </summary>
	public sealed class Timekeeper2
	{
		/// <summary>
		/// The default thread when one isn't specified.
		/// </summary>
		private const int DefaultThread = 0;
		private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, TimekeeperEntry2>> dictionary;
		private readonly IFileSystem fileSystem;
		private readonly IAppSettings settings;
		private bool? logAllEvents;

		/// <summary>
		/// Initializes a new instance of the <see cref="Timekeeper2"/> class.
		/// </summary>
		public Timekeeper2()
			: this(FileSystem.Instance.DeepCopy(), AppSettings.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Timekeeper2"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		public Timekeeper2(IFileSystem fileSystem, IAppSettings settings)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			this.fileSystem = fileSystem;
			this.settings = settings;
			this.dictionary = new ConcurrentDictionary<string, ConcurrentDictionary<int, TimekeeperEntry2>>();
		}

		/// <summary>
		/// Gets the time keeper entry count.
		/// </summary>
		/// <value>
		/// The total number of entries.
		/// </value>
		public int Count => this.dictionary.Count;

		/// <summary>
		/// Retrieves the entry for the specified operation key.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		/// <returns>
		/// The <see cref="TimekeeperEntry2"/> instance.
		/// </returns>
		public TimekeeperEntry2 GetEntry(string key)
		{
			return this.GetEntry(key, DefaultThread);
		}

		/// <summary>
		/// Retrieves the entry for the specified operation key and thread.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		/// <param name="thread">
		/// The thread key associated with the operation.
		/// </param>
		/// <returns>
		/// The <see cref="TimekeeperEntry2"/> instance.
		/// </returns>
		public TimekeeperEntry2 GetEntry(string key, int thread)
		{
			if (!this.dictionary.ContainsKey(key))
			{
				return null;
			}

			ConcurrentDictionary<int, TimekeeperEntry2> entryDictionary = this.dictionary[key];
			return entryDictionary.ContainsKey(thread) ? entryDictionary[thread] : null;
		}

		/// <summary>
		/// Captures the ending time of an operation.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		public void MarkEnd(string key)
		{
			this.MarkEnd(key, DefaultThread);
		}

		/// <summary>
		/// Captures the ending time of an operation.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		/// <param name="thread">
		/// The thread key associated with the import operation.
		/// </param>
		public void MarkEnd(string key, int thread)
		{
			if (!this.LogAllEvents())
			{
				return;
			}

			ConcurrentDictionary<int, TimekeeperEntry2> entries = this.dictionary[key];
			TimekeeperEntry2 entry = entries?[thread];
			if (entry == null)
			{
				return;
			}

			entry.Count++;
			entry.Length += (System.DateTime.Now.Ticks - entry.StartTime) / 10000;
			entry.StartTime = System.DateTime.Now.Ticks;
		}

		/// <summary>
		/// Captures the starting time of an operation.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		public void MarkStart(string key)
		{
			this.MarkStart(key, 0);
		}

		/// <summary>
		/// Captures the starting time of an operation.
		/// </summary>
		/// <param name="key">
		/// The operation key.
		/// </param>
		/// <param name="thread">
		/// The thread key associated with the operation.
		/// </param>
		public void MarkStart(string key, int thread)
		{
			if (!this.LogAllEvents())
			{
				return;
			}

			if (this.dictionary.ContainsKey(key))
			{
				ConcurrentDictionary<int, TimekeeperEntry2> entryDictionary = this.dictionary[key];
				if (entryDictionary.ContainsKey(thread))
				{
					entryDictionary[thread].StartTime = System.DateTime.Now.Ticks;
				}
				else
				{
					entryDictionary[thread] = new TimekeeperEntry2();
				}
			}
			else
			{
				this.dictionary[key] = new ConcurrentDictionary<int, TimekeeperEntry2>()
					                       {
						                       [thread] = new TimekeeperEntry2(),
					                       };
			}
		}

		/// <summary>
		/// Generates a CSV report of all entries to the current working directory, with individual items represented as columns.
		/// </summary>
		/// <exception cref="ArgumentException">
		/// The file name is empty, contains only white spaces, or contains invalid characters.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// The file path is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// The file does not exist.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The file path is in an invalid format.
		/// </exception>
		/// <exception cref="SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// Access to file path is denied.
		/// </exception>
		/// <exception cref="PathTooLongException">
		/// The file path is too long (greater than 260 characters).
		/// </exception>
		public void GenerateCsvReportItemsAsRows()
		{
			this.GenerateCsvReportItemsAsRows(string.Empty, string.Empty);
		}

		/// <summary>
		/// Generates a CSV report of all entries to the specified directory, with individual items represented as columns.
		/// </summary>
		/// <param name="filenameSuffix">
		/// The suffix with which to look up files.
		/// </param>
		/// <param name="directory">
		/// The directory in which to store the result.
		/// </param>
		/// <exception cref="ArgumentException">
		/// The file name is empty, contains only white spaces, or contains invalid characters.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// The file path is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// The file does not exist.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The file path is in an invalid format.
		/// </exception>
		/// <exception cref="SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// Access to file path is denied.
		/// </exception>
		/// <exception cref="PathTooLongException">
		/// The file path is too long (greater than 260 characters).
		/// </exception>
		public void GenerateCsvReportItemsAsRows(string filenameSuffix, string directory)
		{
			if (!this.LogAllEvents())
			{
				return;
			}

			string fileName = this.GetFileName(filenameSuffix);
			string file = directory + fileName;
			if (!string.IsNullOrEmpty(directory))
			{
				file = this.fileSystem.Path.Combine(directory, fileName);
			}

			using (IStreamWriter writer = this.fileSystem.CreateStreamWriter(file, false, Encoding.Default))
			{
				writer.WriteLine("\"Function Name\",\"Number of calls\",\"Total Length (ms)\"");
				string[] keys = new string[checked(this.dictionary.Keys.Count - 1 + 1)];
				this.dictionary.Keys.CopyTo(keys, 0);
				System.Array.Sort<string>(keys);
				foreach (string key in keys)
				{
					ConcurrentDictionary<int, TimekeeperEntry2> dict = this.dictionary[key];
					int count = 0;
					long length = 0;
					foreach (TimekeeperEntry2 entry in dict.Values)
					{
						count += entry.Count;
						length += entry.Length;
					}

					writer.WriteLine($"\"{key}\",\"{count}\",\"{length}\"");
				}
			}
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <param name="filenameSuffix">
		/// The filename suffix.
		/// </param>
		/// <returns>
		/// The filename.
		/// </returns>
		private string GetFileName(string filenameSuffix)
		{
			if (!this.LogAllEvents())
			{
				return null;
			}

			System.Text.StringBuilder filename = new System.Text.StringBuilder();
			filename.Append(DateTime.Now.Year);
			filename.Append(DateTime.Now.Month.ToString().PadLeft(2, '0'));
			filename.Append(DateTime.Now.Day.ToString().PadLeft(2, '0'));
			filename.Append(DateTime.Now.Hour.ToString().PadLeft(2, '0'));
			filename.Append(DateTime.Now.Minute.ToString().PadLeft(2, '0'));
			filename.Append(DateTime.Now.Second.ToString().PadLeft(2, '0'));
			filename.Append(filenameSuffix);
			filename.Append(".csv");
			return filename.ToString();
		}

		private bool LogAllEvents()
		{
			if (this.logAllEvents == null)
			{
				this.logAllEvents = this.settings.LogAllEvents;
			}

			return this.logAllEvents == true;
		}
	}
}