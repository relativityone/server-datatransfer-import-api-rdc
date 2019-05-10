// ----------------------------------------------------------------------------
// <copyright file="ImporterException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Data
{
	using System;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Microsoft.VisualBasic.CompilerServices;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents an exception that occured while attempting to import data.
	/// </summary>
	[Serializable]
	public class ImporterException : Exception
	{
		/// <summary>
		/// The maximum column ordinal before the Excel-style ordinal requires more than 1 character.
		/// </summary>
		public const int ExcelSingleCharMaxOrdinal = 26;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException" /> class.
		/// </summary>
		public ImporterException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public ImporterException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ImporterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		protected ImporterException(long row, string fieldName, string additionalInfo)
			: this(row, fieldName, additionalInfo, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <param name="innerException">
		/// The exception that caused this exception.
		/// </param>
		protected ImporterException(long row, string fieldName, string additionalInfo, Exception innerException)
			: base(GetErrorMessage(row, fieldName, additionalInfo), innerException)
		{
			this.Row = row;
			this.FieldName = fieldName;
			this.AdditionalInfo = additionalInfo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		protected ImporterException(long row, int column, string fieldName, string additionalInfo)
			: base(GetErrorMessage(row, fieldName, additionalInfo))
		{
			this.Row = row;
			this.Column = column;
			this.FieldName = fieldName;
			this.AdditionalInfo = additionalInfo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <param name="innerException">
		/// The exception that caused this exception.
		/// </param>
		protected ImporterException(
			long row,
			int column,
			string fieldName,
			string additionalInfo,
			Exception innerException)
			: base(GetErrorMessage(row, fieldName, additionalInfo), innerException)
		{
			this.Row = row;
			this.Column = column;
			this.FieldName = fieldName;
			this.AdditionalInfo = additionalInfo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		protected ImporterException(long row, int column, string additionalInfo)
			: this(row, column, additionalInfo, (Exception)null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <param name="innerException">
		/// The exception that caused this exception.
		/// </param>
		protected ImporterException(long row, int column, string additionalInfo, Exception innerException)
			: base(GetExcelStyleErrorMessage(row, column, additionalInfo), innerException)
		{
			this.Row = row;
			this.Column = column;
			this.AdditionalInfo = additionalInfo;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImporterException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		protected ImporterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.AdditionalInfo = info.GetString(nameof(this.AdditionalInfo));
			this.Column = info.GetInt32(nameof(this.Column));
			this.FieldName = info.GetString(nameof(this.FieldName));
			this.Row = info.GetInt64(nameof(this.Row));
		}

		/// <summary>
		/// Gets or sets the additional information describing this failure.
		/// </summary>
		/// <value>
		/// The additional information.
		/// </value>
		public string AdditionalInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the index of the column causing the exception.
		/// </summary>
		/// <value>
		/// The column number.
		/// </value>
		public int Column
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the field causing the exception.
		/// </summary>
		/// <value>
		/// The field name.
		/// </value>
		public string FieldName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the index of the row causing the exception.
		/// </summary>
		/// <value>
		/// The row number.
		/// </value>
		public long Row
		{
			get;
			set;
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue(nameof(this.AdditionalInfo), this.AdditionalInfo);
			info.AddValue(nameof(this.Column), this.Column);
			info.AddValue(nameof(this.FieldName), this.FieldName);
			info.AddValue(nameof(this.Row), this.Row);
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets an error message that includes row, field, and additional information.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetErrorMessage(long row, string fieldName, string additionalInfo)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.ImporterStandardFieldError,
				row,
				fieldName,
				additionalInfo);
		}

		/// <summary>
		/// Gets an error message that includes row, column, and additional information.
		/// The <paramref name="column"/> is automatically transformed into an Excel-based column string if greater than 26.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetExcelStyleErrorMessage(long row, int column, string additionalInfo)
		{
			return GetFormattedExcelErrorMessage(row, GetExcelStyleColumnOrdinal(column), additionalInfo);
		}

		/// <summary>
		/// Gets a formatted error message that includes row, column, and additional information.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The column ordinal or name causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetFormattedErrorMessage(long row, string column, string additionalInfo)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.ImporterStandardError,
				row,
				column,
				additionalInfo);
		}

		/// <summary>
		/// Gets a formatted error Excel-based message that includes row, column, and additional information.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The column ordinal or name causing the exception.
		/// </param>
		/// <param name="additionalInfo">
		/// Additional information describing this failure.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetFormattedExcelErrorMessage(long row, string column, string additionalInfo)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.ImporterExcelError,
				row,
				column,
				additionalInfo);
		}

		/// <summary>
		/// Gets the Excel style column ordinal value.
		/// </summary>
		/// <param name="column">
		/// The column index.
		/// </param>
		/// <returns>
		/// The column ordinal string.
		/// </returns>
		internal static string GetExcelStyleColumnOrdinal(int column)
		{
			if (column < ExcelSingleCharMaxOrdinal)
			{
				return Conversions.ToString(Microsoft.VisualBasic.Strings.ChrW(checked(column + 65)));
			}

			return GetExcelStyleColumnOrdinal(checked((int)Math.Round(Math.Floor(column / 26.0)) - 1))
			       + GetExcelStyleColumnOrdinal(column % 26);
		}
	}
}