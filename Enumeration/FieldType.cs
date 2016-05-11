using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Enumeration 
{
	/// <summary>
	/// Specifies the type of data that a field can hold.
	/// </summary>
	public enum FieldType
	{
		/// <summary>
		/// The field can hold nothing.
		/// </summary>
		Empty = -1,
		/// <summary>
		/// The field holds a set of characters of variable length.
		/// </summary>
		VarChar = 0,
		/// <summary>
		/// The field holds a whole number.
		/// </summary>
		Integer = 1,
		/// <summary>
		/// The field holds a date in the format "MM/DD/YYYY h:mm XM".
		/// </summary>
		Date = 2,
		/// <summary>
		/// The field holds a value of Yes or No.
		/// </summary>
		Boolean = 3,
		/// <summary>
		/// The field holds some amount of text.
		/// </summary>
		Text = 5,
		/// <summary>
		/// The field represents a single value selected from a list.
		/// </summary>
		Code = 5,
		/// <summary>
		/// The field holds a decimal number.
		/// </summary>
		Decimal = 6,
		/// <summary>
		/// The field represents an amount of currency.
		/// </summary>
		Currency = 7,
		/// <summary>
		/// The field represents a number of values selected from a list.
		/// </summary>
		MultiCode = 8,
		/// <summary>
		/// The field holds a file.
		/// </summary>
		File = 9,
		/// <summary>
		/// The field holds an object.
		/// </summary>
		Object = 10,
		/// <summary>
		/// The field identifies a user.
		/// </summary>
		User = 11,
		/// <summary>
		/// The field holds text for a layout. Do not use.
		/// </summary>
		LayoutText = 12,
		/// <summary>
		/// The field is a multiple object field; it defines a relationship between multiple objects.
		/// </summary>
		Objects = 13

	}


}
