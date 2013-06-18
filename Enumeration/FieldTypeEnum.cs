using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kCura.Relativity.ImportAPI.Enumeration 
{
	/// <summary>
	/// Specifies the type of data that a field can hold.
	/// </summary>
	public enum FieldTypeEnum
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
		/// The field represents a list from which one item can be selected.
		/// </summary>
		Code = 5,
		/// <summary>
		/// The field holds a decimal number.
		/// </summary>
		Decimal = 6,
		/// <summary>
		/// The field represents a type of currency.
		/// </summary>
		Currency = 7,
		/// <summary>
		/// The field represents a list from which multiple items can be selected.
		/// </summary>
		MultiCode = 8,
		/// <summary>
		/// The field holds a file. Not in use in Relativity.
		/// </summary>
		File = 9,
		/// <summary>
		/// The field holds an object. Not in use in Relativity.
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
		/// The field holds a list of objects.
		/// </summary>
		Objects = 13

	}


}
