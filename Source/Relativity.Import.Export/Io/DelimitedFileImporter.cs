// ----------------------------------------------------------------------------
// <copyright file="DelimitedFileImporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Threading;

	using Microsoft.VisualBasic.CompilerServices;

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
		/// The unbounded character constant.
		/// </summary>
		/// <remarks>
		/// This special value indicates an unspecified bound and puts the importer in the <see cref="DelimitedMode.Simple"/> mode.
		/// </remarks>
		private const char UnboundedChar = char.MaxValue;

		/// <summary>
		/// The unspecified newline proxy character constant.
		/// </summary>
		/// <remarks>
		/// This special value indicates no newline proxy character is defined.
		/// </remarks>
		private const char UnspecifiedNewlineProxyChar = char.MaxValue;

		/// <summary>
		/// The end of file character value.
		/// </summary>
		private const int EofChar = -1;

		/// <summary>
		/// The newline proxy string
		/// </summary>
		private string newlineProxyString;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter string to use while splitting lines. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="bound">
		/// The bounding string surrounding each delimiter. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="newlineProxy">
		/// The string with which to replace system newline characters (ClRf). This is limited to 1 character and optional.
		/// </param>
		protected DelimitedFileImporter(string delimiter, string bound, string newlineProxy)
			: this(delimiter, bound, newlineProxy, new IoReporterContext(), new NullLogger(), CancellationToken.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter character to use while splitting lines. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="bound">
		/// The bounding string surrounding each delimiter. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="newlineProxy">
		/// The string with which to replace system newline characters (ClRf). This is limited to 1 character and optional.
		/// </param>
		/// <param name="retry">
		/// Specify whether retry behavior is required. This flag was added for backwards compatibility with legacy code.
		/// </param>
		protected DelimitedFileImporter(string delimiter, string bound, string newlineProxy, bool retry)
			: this(
				delimiter,
				bound,
				newlineProxy,
				new IoReporterContext { RetryOptions = retry ? RetryOptions.Io : RetryOptions.None },
				new NullLogger(),
				CancellationToken.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter string to use while splitting lines. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="bound">
		/// The bounding string surrounding each delimiter. This is limited to 1 character and must be specified.
		/// </param>
		/// <param name="newlineProxy">
		/// The string with which to replace system newline characters (ClRf). This is limited to 1 character and optional.
		/// </param>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="delimiter"/> or <paramref name="bound"/> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="delimiter"/> or <paramref name="bound"/> is not exactly 1 character.
		/// </exception>
		protected DelimitedFileImporter(
			string delimiter,
			string bound,
			string newlineProxy,
			IoReporterContext context,
			ILog logger,
			CancellationToken token)
			: base(context, logger, token)
		{
			if (string.IsNullOrWhiteSpace(delimiter))
			{
				throw new ArgumentNullException(nameof(delimiter));
			}

			if (delimiter.Length != 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(delimiter),
					"The delimiter string parameter must define 1 character.");
			}

			if (string.IsNullOrWhiteSpace(bound))
			{
				throw new ArgumentNullException(nameof(bound));
			}

			if (bound.Length != 1)
			{
				throw new ArgumentOutOfRangeException(
					nameof(bound),
					"The bound string parameter must define 1 character.");
			}

			this.Delimiter = delimiter[0];
			this.Bound = bound[0];

			// Do NOT be tempted to use IsNullOrWhitespace here!
			this.NewlineProxy = string.IsNullOrEmpty(newlineProxy) ? UnspecifiedNewlineProxyChar : newlineProxy[0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter character to use while splitting lines.
		/// </param>
		/// <param name="retry">
		/// Specify whether retry behavior is required. This flag was added for backwards compatibility with legacy code.
		/// </param>
		protected DelimitedFileImporter(char delimiter, bool retry)
			: this(
				delimiter,
				UnboundedChar,
				UnspecifiedNewlineProxyChar,
				new IoReporterContext { RetryOptions = retry ? RetryOptions.Io : RetryOptions.None },
				new NullLogger(),
				CancellationToken.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter character to use while splitting lines.
		/// </param>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		protected DelimitedFileImporter(char delimiter, IoReporterContext context, ILog logger, CancellationToken token)
			: this(delimiter, UnboundedChar, UnspecifiedNewlineProxyChar, context, logger, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelimitedFileImporter"/> class.
		/// </summary>
		/// <param name="delimiter">
		/// The delimiter character to use while splitting lines.
		/// </param>
		/// <param name="bound">
		/// The bounding characters surrounding each delimiter.
		/// </param>
		/// <param name="newlineProxy">
		/// The character with which to replace system newline characters (ClRf).
		/// </param>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		protected DelimitedFileImporter(
			char delimiter,
			char bound,
			char newlineProxy,
			IoReporterContext context,
			ILog logger,
			CancellationToken token)
			: base(context, logger, token)
		{
			this.Delimiter = delimiter;
			this.Bound = bound;
			this.NewlineProxy = newlineProxy;
		}

		/// <summary>
		/// Gets the current line number.
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		public int CurrentLineNumber
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the bound character.
		/// </summary>
		/// <value>
		/// The character.
		/// </value>
		protected char Bound
		{
			get;
		}

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
		/// Gets the character delimiter.
		/// </summary>
		/// <value>
		/// The character.
		/// </value>
		protected char Delimiter
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the importer has reached the end of file.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when reaching the end of file.; otherwise, <see langword="false" />.
		/// </value>
		protected virtual bool HasReachedEof => this.Peek() == EofChar;

		/// <summary>
		/// Gets the delimited mode.
		/// </summary>
		/// <value>
		/// The <see cref="DelimitedMode"/> value.
		/// </value>
		protected DelimitedMode Mode => this.Bound == UnboundedChar ? DelimitedMode.Simple : DelimitedMode.Bounded;

		/// <summary>
		/// Gets the newline proxy.
		/// </summary>
		/// <value>
		/// The character array.
		/// </value>
		protected char NewlineProxy
		{
			get;
		}

		/// <summary>
		/// Gets or sets the stream reader.
		/// </summary>
		/// <value>
		/// The <see cref="StreamReader"/> instance.
		/// </value>
		protected StreamReader Reader
		{
			get;
			set;
		}

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
		/// Validates the string for variable character.
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
		/// <param name="currentLineNumber">
		/// The current line number.
		/// </param>
		/// <param name="displayName">
		/// The display name of the field to report, should an exception occur.
		/// </param>
		/// <returns>
		/// The validated string.
		/// </returns>
		public static string ValidateStringForVarChar(
			string value,
			int column,
			int fieldLength,
			int currentLineNumber,
			string displayName)
		{
			if (value != null && value.Length > fieldLength)
			{
				throw new InputStringExceedsFixedLengthException(currentLineNumber, column, value.Length, fieldLength, displayName);
			}

			return NullableTypesHelper.ToString(value);
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
		/// <param name="maxLength">
		/// The maximum field length to report, should an exception occur.
		/// </param>
		/// <param name="displayName">
		/// The display name of the field to report, should an exception occur.
		/// </param>
		/// <returns>
		/// null if <see param="value"/> is null or empty; otherwise, <see param="value"/>.
		/// </returns>
		public string GetNullableFixedString(string value, int column, int maxLength, string displayName)
		{
			if (value != null && value.Length > maxLength)
			{
				throw new InputStringExceedsFixedLengthException(this.CurrentLineNumber, column, value.Length, maxLength, displayName);
			}

			return NullableTypesHelper.ToString(value);
		}

		/// <summary>
		/// Gets a nullable integer from the provided string.
		/// </summary>
		/// <param name="value">
		/// The value to parse.
		/// </param>
		/// <param name="column">
		/// The column to report, should an exception occur.
		/// </param>
		/// <returns>
		/// A nullable integer representation of <see paramref="value" />.
		/// </returns>
		/// <exception cref="IntegerImporterException">
		/// Thrown when <paramref name="value"/> exceeds the bounds of an integer
		/// or the <paramref name="value"/> format doesn't comply with <see cref="NumberStyles.Integer"/> or <see cref="NumberStyles.AllowThousands"/>.
		/// </exception>
		public int? GetNullableInteger(string value, int column)
		{
			try
			{
				return NullableTypesHelper.ToNullableInt32(value);
			}
			catch (Exception e)
			{
				throw new IntegerImporterException(this.CurrentLineNumber, column, e);
			}
		}

		/// <summary>
		/// Peeks at the next character of the file without advancing the underlying file cursor.
		/// </summary>
		/// <returns>
		/// The character value.
		/// </returns>
		public virtual int Peek()
		{
			const bool Advance = false;
			return this.GetChar(Advance);
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
			int result = this.Context.WaitAndRetryPolicy.WaitAndRetry<int, Exception>(
				this.Context.RetryOptions == RetryOptions.None ? 0 : this.CachedAppSettings.IoErrorNumberOfRetries,
				i => i == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(this.CachedAppSettings.IoErrorWaitTimeInSeconds),
				(exception, span) =>
					{
						IoWarningEventArgs args = new IoWarningEventArgs(
							this.CachedAppSettings.IoErrorWaitTimeInSeconds,
							exception,
							this.CurrentLineNumber);
						this.PublishWarningMessage(args);
						if (this.Reader.BaseStream is System.IO.FileStream && exception is System.IO.IOException
						                                                   && exception.ToString().ToLowerInvariant()
							                                                   .IndexOf(
								                                                   "network",
								                                                   StringComparison.OrdinalIgnoreCase)
						                                                   != -1)
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
		/// The nullable <see cref="decimal"/> value.
		/// </returns>
		protected virtual decimal? ParseNullableDecimal(string value)
		{
			return NullableTypesHelper.ToNullableDecimal(value);
		}

		/// <summary>
		/// Appends the <paramref name="value"/> character to the <paramref name="sb"/> string builder and automatically updates <paramref name="hasAlertedError"/> if a validation error is published.
		/// </summary>
		/// <param name="sb">
		/// The string builder to append.
		/// </param>
		/// <param name="value">
		/// The character to append to the string builder.
		/// </param>
		/// <param name="startPosition">
		/// The start position.
		/// </param>
		/// <param name="maxCharacterLength">
		/// The maximum length of the character.
		/// </param>
		/// <param name="hasAlertedError">
		/// The reference to a flag indicating whether a validation error has been published.
		/// </param>
		private void Append(
			StringBuilder sb,
			char value,
			long startPosition,
			int maxCharacterLength,
			ref bool hasAlertedError)
		{
			if (this.ValidateAppend(startPosition, maxCharacterLength, ref hasAlertedError))
			{
				sb.Append(value);
			}
		}

		/// <summary>
		/// Appends the <paramref name="value"/> string to the <paramref name="sb"/> string builder and automatically updates <paramref name="hasAlertedError"/> if a validation error is published.
		/// </summary>
		/// <param name="sb">
		/// The string builder to append.
		/// </param>
		/// <param name="value">
		/// The string to append to the string builder.
		/// </param>
		/// <param name="startPosition">
		/// The start position.
		/// </param>
		/// <param name="maxCharacterLength">
		/// The maximum length of the character.
		/// </param>
		/// <param name="hasAlertedError">
		/// The reference to a flag indicating whether a validation error has been published.
		/// </param>
		private void Append(
			StringBuilder sb,
			string value,
			long startPosition,
			int maxCharacterLength,
			ref bool hasAlertedError)
		{
			if (this.ValidateAppend(startPosition, maxCharacterLength, ref hasAlertedError))
			{
				sb.Append(value);
			}
		}

		/// <summary>
		/// Validates the append operation and automatically updates <paramref name="hasAlertedError"/> if a validation error is published.
		/// </summary>
		/// <param name="startPosition">
		/// The start position.
		/// </param>
		/// <param name="maxCharacterLength">
		/// The maximum length of the character.
		/// </param>
		/// <param name="hasAlertedError">
		/// The reference to a flag indicating whether a validation error has been published.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the append is validated; otherwise, <see langword="false" />.
		/// </returns>
		private bool ValidateAppend(long startPosition, int maxCharacterLength, ref bool hasAlertedError)
		{
			if (checked(this.CharacterPosition - startPosition) > (long)maxCharacterLength)
			{
				if (!hasAlertedError)
				{
					hasAlertedError = true;
					string message = "Contents of cell has exceeded maximum length of "
					                 + Conversions.ToString(maxCharacterLength) + " (character "
					                 + Conversions.ToString(this.CharacterPosition) + ")";
					this.PublishWarningMessage(new IoWarningEventArgs(message, this.CurrentLineNumber));
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Converts all newline characters within the specified input string builder instance.
		/// </summary>
		/// <param name="input">
		/// The input string contained within the string builder to convert.
		/// </param>
		/// <returns>
		/// The converted input string.
		/// </returns>
		private string ConvertNewLine(StringBuilder input)
		{
			return this.ConvertNewLine(input.ToString());
		}

		/// <summary>
		/// Converts all newline characters within the specified input string.
		/// </summary>
		/// <param name="input">
		/// The input string to convert.
		/// </param>
		/// <returns>
		/// The converted input string.
		/// </returns>
		private string ConvertNewLine(string input)
		{
			if (this.NewlineProxy != UnspecifiedNewlineProxyChar)
			{
				if (string.IsNullOrEmpty(this.newlineProxyString))
				{
					this.newlineProxyString = Conversions.ToString(this.NewlineProxy);
				}

				input = input.Replace(VisualBasicConstants.VbNewLine, this.newlineProxyString);
				input = input.Replace(VisualBasicConstants.VbCr, this.newlineProxyString);
				input = input.Replace(VisualBasicConstants.VbLf, this.newlineProxyString);
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
			if (this.Peek() == (int)this.Bound)
			{
				// The Read character is NOT used.
				Microsoft.VisualBasic.Strings.ChrW(this.Read());
				if ((int)Microsoft.VisualBasic.Strings.ChrW(this.Peek()) == (int)this.Delimiter ||
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
							Conversions.ToString(this.Bound) + "\r",
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
						this.Bound,
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
				if ((int)ch1 == (int)this.Bound)
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
					if (num == (int)this.Delimiter)
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
								ch2,
								initialCharacterPosition,
								maximumFieldLength,
								ref hasAlertedError);
							break;

						default:
							if (num == (int)this.Bound)
							{
								this.Append(
									stringBuilder,
									this.Bound,
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
						ch1,
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
				if ((int)ch == (int)this.Bound)
				{
					currentArrayList.Add(
						this.GetBoundedFieldValue(ref hasHitEndOfLine, currentArrayList.Count, maximumFieldLength));
				}
				else if ((int)ch == (int)this.Delimiter)
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
					builder.Append(this.NewlineProxy);
				}
				else
				{
					builder.Append(Microsoft.VisualBasic.Strings.ChrW(charCode));
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
				if (num == this.Delimiter)
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
						Microsoft.VisualBasic.Strings.ChrW(charCode),
						initialCharacterPosition,
						maximumCharacters,
						ref hasAlertedError);
				}
				else
				{
					this.Append(
						sb,
						Microsoft.VisualBasic.Strings.ChrW(charCode),
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