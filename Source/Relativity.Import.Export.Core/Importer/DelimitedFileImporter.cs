// ----------------------------------------------------------------------------
// <copyright file="DelimitedFileImporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Importer
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Threading;

	using Microsoft.VisualBasic.CompilerServices;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Resources;
	using Relativity.Logging;

	/// <summary>
	/// Represents an abstract representation of a file importer that operates over delimited files.
	/// </summary>
	public abstract class DelimitedFileImporter : IoReporter
	{
		/// <summary>
		/// The maximum column count for line.
		/// </summary>
		public const int MaxColumnCountForLine = 10000;

		/// <summary>
		/// The end of file character value.
		/// </summary>
		private const int EofChar = -1;
		private readonly char[] delimiter;
		private readonly char[] bound;
		private char[] metaDelimiter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter string to use while splitting lines.
		/// </param>
		/// <param name="bound">
		/// The bounding string surrounding each delimiter.
		/// </param>
		/// <param name="newlineProxy">
		/// The string with which to replace system newline characters (ClRf). Only the first character will be used.
		/// </param>
		/// <param name="publisher">
		/// The I/O warning publisher.
		/// </param>
		/// <param name="options">
		/// The configurable retry options.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process a any requested time.
		/// </param>
		protected DelimitedFileImporter(
			string delimiter,
			string bound,
			string newlineProxy,
			IoWarningPublisher publisher,
			RetryOptions options,
			ILog logger,
			CancellationToken token)
			: base(AppSettings.Instance, FileSystem.Instance.DeepCopy(), publisher, options, logger, token)
		{
			if (string.IsNullOrWhiteSpace(delimiter))
			{
				throw new ArgumentNullException(nameof(delimiter));
			}

			if (delimiter.Length < 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(delimiter),
					"The delimiter string parameter must define at least 1 character.");
			}

			if (string.IsNullOrWhiteSpace(bound))
			{
				throw new ArgumentNullException(nameof(bound));
			}

			if (bound.Length < 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(delimiter),
					"The bound string parameter must define at least 1 character.");
			}

			this.delimiter = delimiter.ToCharArray();
			this.bound = bound.ToCharArray();
			if (newlineProxy != null)
			{
				this.NewlineProxy = newlineProxy.ToCharArray();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter character array to use while splitting lines.
		/// </param>
		/// <param name="bound">
		/// The bounding characters surrounding each delimiter.
		/// </param>
		/// <param name="newlineProxy">
		/// An array of characters with which to replace system newline characters (ClRf). Only the first character will be used.
		/// </param>
		/// <param name="publisher">
		/// The I/O warning publisher.
		/// </param>
		/// <param name="options">
		/// The configurable retry options.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process a any requested time.
		/// </param>
		protected DelimitedFileImporter(
			char[] delimiter,
			char[] bound,
			char[] newlineProxy,
			IoWarningPublisher publisher,
			RetryOptions options,
			ILog logger,
			CancellationToken token)
			: base(AppSettings.Instance, FileSystem.Instance.DeepCopy(), publisher, options, logger, token)
		{
			if (delimiter == null)
			{
				throw new ArgumentNullException(nameof(delimiter));
			}

			if (delimiter.Length < 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(delimiter),
					"The delimiter array parameter must define at least 1 character.");
			}

			if (bound == null)
			{
				throw new ArgumentNullException(nameof(bound));
			}

			if (bound.Length < 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(delimiter),
					"The bound array parameter must define at least 1 character.");
			}

			this.delimiter = delimiter;
			this.bound = bound;
			this.NewlineProxy = newlineProxy;
		}

		/// <summary>
		/// Gets the bound.
		/// </summary>
		/// <value>
		/// The bound.
		/// </value>
		protected char Bound => this.bound[0];

		/// <summary>
		/// Gets the current character position.
		/// </summary>
		/// <value>
		/// The position.
		/// </value>
		protected long CharacterPosition
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current line number.
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		protected long CurrentLineNumber { get; private set; }

		/// <summary>
		/// Gets the character delimiter.
		/// </summary>
		/// <value>
		/// The delimiter.
		/// </value>
		protected char Delimiter => this.delimiter[0];

		/// <summary>
		/// Gets a value indicating whether the importer has reached the end of file.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when reaching the end of file.; otherwise, <see langword="false" />.
		/// </value>
		protected virtual bool HasReachedEof => this.Peek() == EofChar;

		/// <summary>
		/// Gets the meta delimiter.
		/// </summary>
		/// <value>
		/// The meta delimiter.
		/// </value>
		protected char[] MetaDelimiter
		{
			get
			{
				if (this.Mode == DelimitedMode.Simple)
				{
					throw new ArgumentOutOfRangeException(nameof(this.Mode), "The meta delimiter cannot be used when the mode is set to Simple.");
				}

				if (this.metaDelimiter == null)
				{
					char[] metaDelim = new char[(this.bound.Length * 2) + this.delimiter.Length - 1];
					int i = 0;
					foreach (char ch in this.bound)
					{
						metaDelim[i] = ch;
						i += 1;
					}

					foreach (char ch in this.delimiter)
					{
						metaDelim[i] = ch;
						i += 1;
					}

					foreach (char ch in this.bound)
					{
						metaDelim[i] = ch;
						i += 1;
					}

					this.metaDelimiter = metaDelim;
				}

				return this.metaDelimiter;
			}
		}

		/// <summary>
		/// Gets the delimited mode.
		/// </summary>
		/// <value>
		/// The <see cref="DelimitedMode"/> value.
		/// </value>
		protected DelimitedMode Mode => this.bound == null ? DelimitedMode.Simple : DelimitedMode.Bounded;

		/// <summary>
		/// Gets the newline proxy.
		/// </summary>
		/// <value>
		/// The character array.
		/// </value>
		protected char[] NewlineProxy { get; }

		/// <summary>
		/// Gets or sets the stream reader.
		/// </summary>
		/// <value>
		/// The <see cref="StreamReader"/> instance.
		/// </value>
		protected StreamReader Reader { get; set; }

		/// <summary>
		/// Gets a value indicating whether to use concordance style bound starts.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> to use concordance style bound starts; otherwise, <see langword="false" />.
		/// </returns>
		protected virtual bool UseConcordanceStyleBoundStart => true;

		/// <summary>
		/// Gets or sets the whitespace trim option.
		/// </summary>
		/// <value>
		/// The <see cref="TrimOption"/> value.
		/// </value>
		protected TrimOption TrimOption
		{
			get;
			set;
		}

		/// <summary>
		/// Closes this importer.
		/// </summary>
		public void Close()
		{
			if (this.Reader == null)
			{
				return;
			}

			this.Reader.Close();
			this.Reader = null;
		}

		/// <summary>
		/// Reads an entire file from the specified path.
		/// </summary>
		/// <param name="path">
		/// The full path to the file.
		/// </param>
		/// <returns>
		/// An object representing the results of the read operation.
		/// </returns>
		public abstract object ReadFile(string path);

		/// <summary>
		/// Gets the boolean representation for the specified string value.
		/// </summary>
		/// <param name="value">
		/// The string value.
		/// </param>
		/// <returns>
		/// The bool value.
		/// </returns>
		public bool GetBoolean(string value)
		{
			if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}

			if (string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return false;
			}

			int parsedValue;
			if (int.TryParse(value, out parsedValue))
			{
				return parsedValue != 0;
			}

			return false;
		}

		/// <summary>
		/// Gets a decimal from the provided string.
		/// </summary>
		/// <param name="value">
		/// The value to parse.
		/// </param>
		/// <param name="column">
		/// The column to report, should an exception occur.
		/// </param>
		/// <returns>
		/// A decimal representation of <see param="value"/>.
		/// </returns>
		public decimal GetDecimal(string value, int column)
		{
			try
			{
				decimal returnDecimal = decimal.Parse(value, CultureInfo.InvariantCulture);
				if (Conversions.ToString(decimal.Truncate(returnDecimal)).Length > 15)
				{
					throw new FormatException($"The decimal value {value} exceeds the max length.");
				}

				return returnDecimal;
			}
			catch (Exception e)
			{
				throw new DecimalImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Gets the nullable boolean representation for the specified string value.
		/// </summary>
		/// <param name="value">
		/// The string value.
		/// </param>
		/// <param name="column">
		/// The index of the column for the specified value.
		/// </param>
		/// <returns>
		/// The nullable bool value.
		/// </returns>
		/// <exception cref="BooleanImporterException">
		/// Thrown when the value cannot be converted.
		/// </exception>
		public bool? GetNullableBoolean(string value, int column)
		{
			try
			{
				return NullableTypesHelper.GetNullableBoolean(value);
			}
			catch (Exception e)
			{
				throw new BooleanImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Gets the nullable boolean representation for the specified string value.
		/// </summary>
		/// <param name="value">
		/// The string value.
		/// </param>
		/// <param name="column">
		/// The index of the column for the specified value.
		/// </param>
		/// <returns>
		/// The nullable bool value.
		/// </returns>
		/// <exception cref="BooleanImporterException">
		/// Thrown when the value cannot be converted.
		/// </exception>
		public DateTime? GetNullableDateTime(string value, int column)
		{
			try
			{
				return NullableTypesHelper.GetNullableDateTime(value);
			}
			catch (Exception e)
			{
				throw new DateImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Gets the nullable decimal representation for the specified string value.
		/// </summary>
		/// <param name="value">
		/// The string value.
		/// </param>
		/// <param name="column">
		/// The index of the column for the specified value.
		/// </param>
		/// <returns>
		/// The nullable decimal value.
		/// </returns>
		/// <exception cref="DecimalImporterException">
		/// Thrown when the value cannot be converted.
		/// </exception>
		public decimal? GetNullableDecimal(string value, int column)
		{
			try
			{
				decimal? returnmDecimal = this.ParseNullableDecimal(value);
				if (returnmDecimal.HasValue && Conversions.ToString(decimal.Truncate(returnmDecimal.Value)).Length > 15)
				{
					throw new FormatException($"The decimal value {value} exceeds the max length.");
				}

				return returnmDecimal;
			}
			catch (Exception e)
			{
				throw new DecimalImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Gets a fixed-length string the from the provided value.
		/// </summary>
		/// <param name="value">
		/// The value to parse.
		/// </param>
		/// <param name="column">
		/// The column to report, should an exception occur.
		/// </param>
		/// <param name="fieldLength">
		/// The field length to report, should an exception occur.
		/// </param>
		/// <returns>
		/// null if <see param="value"/> is null or empty; otherwise, <see param="value"/>.
		/// </returns>
		public string GetNullableFixedString(string value, int column, int fieldLength)
		{
			if (value != null && value.Length > fieldLength)
			{
				throw new StringImporterException(this.CurrentLineNumber, column, fieldLength);
			}

			return NullableTypesHelper.ToString(value);
		}

		/// <summary>
		/// Resets the current line being read to the first line of the file.
		/// </summary>
		public void ResetLineCounter()
		{
			this.CurrentLineNumber = 0;
		}

		/// <summary>
		/// Resets this importer to the beginning of the file.
		/// </summary>
		public void Rewind()
		{
			this.Reader.BaseStream.Seek(0, SeekOrigin.Begin);
			this.CurrentLineNumber = 0;
			this.CharacterPosition = 0;
		}

		/// <summary>
		/// Advances the underlying file cursor to the next line without returning results.
		/// </summary>
		protected void AdvanceLine()
		{
			const bool SaveData = false;
			switch (this.Mode)
			{
				case DelimitedMode.Bounded:
					this.GetLineBounded(SaveData, int.MaxValue, int.MaxValue);
					break;

				case DelimitedMode.Simple:
					this.GetLineSimple(SaveData);
					break;
			}
		}

		/// <summary>
		/// Gets the next line of the file as a string array.
		/// </summary>
		/// <returns>
		/// The next line of the file.
		/// </returns>
		protected string[] GetLine()
		{
			return this.GetLine(int.MaxValue);
		}

		/// <summary>
		/// Gets the next line of the file as a string array.
		/// </summary>
		/// <param name="maximumFieldLength">
		/// The maximum number of field characters before the value is truncated.
		/// </param>
		/// <returns>
		/// The next line of the file.
		/// </returns>
		protected string[] GetLine(int maximumFieldLength)
		{
			string[] retval = null;
			switch (this.Mode)
			{
				case DelimitedMode.Bounded:
					retval = this.GetLineBounded(true, maximumFieldLength, MaxColumnCountForLine);
					break;

				case DelimitedMode.Simple:
					retval = this.GetLineSimple(true);
					break;
			}

			return retval;
		}

		/// <summary>
		/// Peeks at the current character.
		/// </summary>
		/// <returns>
		/// The character value.
		/// </returns>
		protected int Peek()
		{
			const bool Advance = false;
			return this.GetChar(Advance);
		}

		/// <summary>
		/// Gets the character.
		/// </summary>
		/// <param name="advance">
		/// Specify whether to advance the reader pointer.
		/// </param>
		/// <returns>
		/// The character value.
		/// </returns>
		protected int GetChar(bool advance)
		{
			bool reInitReader = false;
			int result = this.WaitAndRetry<int, Exception>(
				this.RetryOptions == RetryOptions.None ? 0 : this.MaxRetryAttempts,
				i => i == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(this.WaitTimeSecondsBetweenRetryAttempts),
				(exception, span) =>
					{
						IoWarningEventArgs args = new IoWarningEventArgs(
							this.WaitTimeSecondsBetweenRetryAttempts,
							exception,
							this.CurrentLineNumber);
						this.PublishWarningMessage(args);
						if (this.Reader.BaseStream is System.IO.FileStream && exception is System.IO.IOException
																		   && exception.ToString().ToLowerInvariant()
																			   .IndexOf("network") != -1)
						{
							reInitReader = true;
						}
					},
				token =>
					{
						if (reInitReader)
						{
							this.ReInitializeReader();
							reInitReader = false;
						}

						if (advance)
						{
							int nextChar = this.Reader.Read();
							this.CharacterPosition++;
							return nextChar;
						}

						return this.Reader.Peek();
					},
				this.CancellationToken);
			return result;
		}

		/// <summary>
		/// Gets a nullable decimal from the provided string.
		/// </summary>
		/// <param name="value">
		/// The value to parse.
		/// </param>
		/// <returns>
		/// The nullable decimal value.
		/// </returns>
		protected virtual decimal? ParseNullableDecimal(string value)
		{
			return NullableTypesHelper.ToNullableDecimal(value);
		}

		private void Append(
			StringBuilder sb,
			string toAppend,
			long startPosition,
			int maxCharacterLength,
			ref bool hasAlertedError)
		{
			if (checked(this.CharacterPosition - startPosition) > (long)maxCharacterLength)
			{
				if (hasAlertedError)
				{
					return;
				}

				hasAlertedError = true;
				string message = "Contents of cell has exceeded maximum length of "
				                 + Conversions.ToString(maxCharacterLength) + " (character "
				                 + Conversions.ToString(this.CharacterPosition) + ")";
				this.PublishWarningMessage(new IoWarningEventArgs(message, this.CurrentLineNumber));
			}
			else
			{
				sb.Append(toAppend);
			}
		}

		private string ConvertNewLine(StringBuilder input)
		{
			return this.ConvertNewLine(input.ToString());
		}

		private string ConvertNewLine(string input)
		{
			if (this.NewlineProxy != null)
			{
				input = input.Replace("\r\n", Conversions.ToString(this.NewlineProxy[0]));
				input = input.Replace('\r', this.NewlineProxy[0]);
				input = input.Replace('\n', this.NewlineProxy[0]);
			}

			switch (this.TrimOption)
			{
				case TrimOption.None:
					return input;

				case TrimOption.Leading:
					return input.TrimStart(' ', '\t');

				case TrimOption.Trailing:
					return input.TrimEnd(' ', '\t');

				case TrimOption.Both:
					return input.Trim(' ', '\t');

				default:
					return null;
			}
		}

		/// <summary>
		/// Gets a bounded field value.
		/// </summary>
		/// <param name="hasHitEndOfLine">
		/// <see langword="true" /> when the end of line has been reached; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="column">
		/// The column index.
		/// </param>
		/// <param name="maximumFieldLength">
		/// The maximum number of field characters before the value is truncated.
		/// </param>
		/// <returns>
		/// The array of strings.
		/// </returns>
		private string GetBoundedFieldValue(ref bool hasHitEndOfLine, int column, int maximumFieldLength)
		{
			StringBuilder stringBuilder = new StringBuilder();
			long initialCharacterPosition = this.CharacterPosition;
			bool hasAlertedError = false;
			if (this.Peek() == (int)this.bound[0])
			{
				// The Read character is NOT used.
				Microsoft.VisualBasic.Strings.ChrW(this.Read());
				if ((int)Microsoft.VisualBasic.Strings.ChrW(this.Peek()) == (int)this.delimiter[0] ||
				    this.Peek() == EofChar ||
				    this.Peek() == 13)
				{
					if (this.Peek() == 13)
					{
						// The Read character is NOT used.
						this.Read();
						if (this.Peek() == 10)
						{
							this.Read();
							hasHitEndOfLine = true;
							return string.Empty;
						}

						this.Append(
							stringBuilder,
							Conversions.ToString(this.bound[0]) + "\r",
							initialCharacterPosition,
							maximumFieldLength,
							ref hasAlertedError);
					}
					else
					{
						this.Read();
						return string.Empty;
					}
				}
				else
				{
					this.Append(
						stringBuilder,
						Conversions.ToString(this.bound[0]),
						initialCharacterPosition,
						maximumFieldLength,
						ref hasAlertedError);
					if (this.UseConcordanceStyleBoundStart)
					{
						// The Read character is NOT used.
						this.Read();
					}
				}
			}

			while (this.Peek() != EofChar)
			{
				char ch1 = Microsoft.VisualBasic.Strings.ChrW(this.Read());
				if ((int)ch1 == (int)this.bound[0])
				{
					if (this.TrimOption == TrimOption.Both ||
					    this.TrimOption == TrimOption.Trailing)
					{
						char[] whitespace = new char[2] { ' ', Microsoft.VisualBasic.ControlChars.Tab };
						while (System.Array.IndexOf<char>(whitespace, Microsoft.VisualBasic.Strings.ChrW(this.Peek())) != EofChar)
						{
							this.Read();
						}
					}

					int num = this.Peek();
					if (num == (int)this.delimiter[0])
					{
						this.Read();
						return this.ConvertNewLine(stringBuilder);
					}

					switch (num)
					{
						case EofChar:
							return this.ConvertNewLine(stringBuilder);

						case 13:
							char ch2 = Microsoft.VisualBasic.Strings.ChrW(this.Read());
							if (this.Peek() == 10)
							{
								this.Read();
								hasHitEndOfLine = true;
								return this.ConvertNewLine(stringBuilder);
							}

							this.Append(
								stringBuilder,
								Conversions.ToString(ch2),
								initialCharacterPosition,
								maximumFieldLength,
								ref hasAlertedError);
							break;

						default:
							if (num == (int)this.bound[0])
							{
								this.Append(
									stringBuilder,
									Conversions.ToString(this.bound[0]),
									initialCharacterPosition,
									maximumFieldLength,
									ref hasAlertedError);
								this.Read();
							}

							break;
					}
				}
				else
				{
					this.Append(
						stringBuilder,
						Conversions.ToString(ch1),
						initialCharacterPosition,
						maximumFieldLength,
						ref hasAlertedError);
				}
			}

			try
			{
				return stringBuilder.ToString();
			}
			catch (Exception e)
			{
				throw new CellImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Gets a bounded line from the reader.
		/// </summary>
		/// <param name="saveData">
		/// <see langword="true" /> to save the data; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="maximumFieldLength">
		/// The maximum number of field characters before the value is truncated.
		/// </param>
		/// <param name="maximumLineLength">
		/// The maximum number of line characters before the value is truncated.
		/// </param>
		/// <returns>
		/// The array of strings.
		/// </returns>
		private string[] GetLineBounded(bool saveData, int maximumFieldLength, int maximumLineLength)
		{
			ConditionalArrayList returnValue = new ConditionalArrayList(saveData);
			ConditionalArrayList currentArrayList = returnValue;
			bool hasHitEndOfLine = false;
			while (this.Peek() != EofChar && !hasHitEndOfLine)
			{
				int charCode = this.Read();
				if (currentArrayList.Count > maximumLineLength)
				{
					currentArrayList = new ConditionalArrayList(false);
				}

				// TODO: Verify the usage of "Or" vs "OrAlso"
				while ((Microsoft.VisualBasic.Strings.ChrW(charCode) == ' '
				        || Microsoft.VisualBasic.Strings.ChrW(charCode) == '\t')
				       && (this.TrimOption == TrimOption.Both
				          || this.TrimOption == TrimOption.Leading))
				{
					charCode = this.Read();
				}

				char ch = Microsoft.VisualBasic.Strings.ChrW(charCode);
				if ((int)ch == (int)this.bound[0])
				{
					currentArrayList.Add(
						this.GetBoundedFieldValue(ref hasHitEndOfLine, currentArrayList.Count, maximumFieldLength));
				}
				else if ((int)ch == (int)this.delimiter[0])
				{
					// Add an empty value
					currentArrayList.Add(string.Empty);
					if (this.Peek() == EofChar)
					{
						// if this delimiter is the last character in the file, add another empty value for the field that comes AFTER the delimiter
						currentArrayList.Add(string.Empty);
					}
				}
				else if (ch == '\r')
				{
					if (this.Peek() == 10)
					{
						currentArrayList.Add(string.Empty);
						this.Read();
						hasHitEndOfLine = true;
					}
					else
					{
						currentArrayList.Add(
							Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(charCode))
							+ this.GetSimpleDelimitedValue(ref hasHitEndOfLine, maximumFieldLength));
					}
				}
				else if (!((Microsoft.VisualBasic.Strings.ChrW(charCode) == ' '
				            || Microsoft.VisualBasic.Strings.ChrW(charCode) == '\t')
				           && (this.TrimOption == TrimOption.Both ||
				               this.TrimOption == TrimOption.Leading)))
				{
					currentArrayList.Add(
						Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(charCode))
						+ this.GetSimpleDelimitedValue(ref hasHitEndOfLine, maximumFieldLength));
				}
			}

			this.CurrentLineNumber++;
			if (currentArrayList != returnValue)
			{
				string message = "This line's column length has exceeded the maximum defined column length of "
				                 + Conversions.ToString(10000) + ".  Remaining columns in line truncated";
				IoWarningEventArgs args = new IoWarningEventArgs(message, this.CurrentLineNumber);
				this.PublishWarningMessage(args);
			}

			return (string[])returnValue.ToArray(typeof(string));
		}

		/// <summary>
		/// Gets an unbounded line from the reader.
		/// </summary>
		/// <param name="saveData">
		/// <see langword="true" /> to save the data; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// The array of strings.
		/// </returns>
		private string[] GetLineSimple(bool saveData)
		{
			ConditionalStringBuilder builder = new ConditionalStringBuilder(saveData);
			while (this.Peek() != EofChar)
			{
				int charCode = this.Read();
				if (charCode == 13)
				{
					if (this.Read() == 10)
					{
						break;
					}
				}
				else if (charCode == 10)
				{
					builder.Append(Conversions.ToString(this.NewlineProxy[0]));
				}
				else
				{
					builder.Append(Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(charCode)));
				}
			}

			string line = builder.ToString();
			this.CurrentLineNumber++;
			return line.Split(this.Delimiter);
		}

		/// <summary>
		/// Retrieves a simple delimited value from the current line.
		/// </summary>
		/// <param name="hasHitEndOfLine">
		/// <see langword="true" /> when the end of line has been reached; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="maximumCharacters">
		/// The maximum characters before the string is truncated.
		/// </param>
		/// <returns>
		/// The string value.
		/// </returns>
		private string GetSimpleDelimitedValue(ref bool hasHitEndOfLine, int maximumCharacters)
		{
			StringBuilder sb = new StringBuilder();
			long initialCharacterPosition = this.CharacterPosition;
			bool hasAlertedError = false;
			while (this.Peek() != EofChar)
			{
				int charCode = this.Read();
				int num = charCode;
				if (num == this.delimiter[0])
				{
					return sb.ToString();
				}

				if (num == 13)
				{
					if (this.Peek() == 10)
					{
						this.Read();
						hasHitEndOfLine = true;
						return sb.ToString();
					}

					this.Append(
						sb,
						Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(charCode)),
						initialCharacterPosition,
						maximumCharacters,
						ref hasAlertedError);
				}
				else
				{
					this.Append(
						sb,
						Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(charCode)),
						initialCharacterPosition,
						maximumCharacters,
						ref hasAlertedError);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Reads the next character of the file and advancing the underlying file cursor.
		/// </summary>
		/// <returns>
		/// The next character of the file.
		/// </returns>
		private int Read()
		{
			const bool Advance = true;
			return this.GetChar(Advance);
		}

		/// <summary>
		/// Re-initialized the reader.
		/// </summary>
		private void ReInitializeReader()
		{
			FileStream fileStream = this.Reader.BaseStream as FileStream;
			if (fileStream == null)
			{
				throw new InvalidOperationException(Strings.ReinitializeReaderNotFileStreamError);
			}

			this.Reader = new System.IO.StreamReader(fileStream.Name, this.Reader.CurrentEncoding);
			if (this.CharacterPosition > 0)
			{
				this.PublishWarningMessage(
					new IoWarningEventArgs(
						"Re-initializing reader for broken network connection",
						this.CurrentLineNumber));
				if (this.CharacterPosition > 100000)
				{
					this.PublishWarningMessage(
						new IoWarningEventArgs(
							"This could take up to several minutes.",
							this.CurrentLineNumber));
				}

				for (int i = 0; i <= this.CharacterPosition; i++)
				{
					this.Reader.Read();
				}
			}
		}
	}
}