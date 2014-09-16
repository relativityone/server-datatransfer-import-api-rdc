Namespace kCura.Windows.Forms
	''' <summary>
	''' These are the valid properties for the control whose values are
	''' used as a reference when calculating the property of another control,
	''' in the case where the base control and the reference control's properties always
	''' differ by the same amount.
	''' </summary>
	Public Enum LayoutBasePropertyTypeForDifference
		Left
		Top
		Width
		Height
		Right
		Bottom
	End Enum

	''' <summary>
	''' These are the valid properties for the control whose values will be changing
	''' when the value of the reference control's property changes, 
	''' in the case where the base control and the reference control's properties always
	''' differ by the same amount. 
	''' </summary>
	Public Enum LayoutRelativePropertyTypeForDifference
		Left
		Top
		Width
		Height
	End Enum

	''' <summary>
	''' These are the valid properties for the control whose values are
	''' used as a reference when calculating the property of another control,
	''' in the case where the base control and the reference control's properties 
	''' differ by a ratio amount
	''' </summary>
	''' <example>Used if when the base control width changes by 2 pixels, the
	''' relative control width changes by 1 pixel.</example>
	Public Enum LayoutBasePropertyTypeForRatio
		Width
		Height
	End Enum

	''' <summary>
	''' These are the valid properties for the control whose values will be changing
	''' when the value of the reference control's property changes, 
	''' in the case where the base control and the reference control's properties 
	''' differ by a ratio amount
	''' </summary>
	''' <example>Used if when the base control width changes by 2 pixels, the
	''' relative control width changes by 1 pixel.</example>
	Public Enum LayoutRelativePropertyTypeForRatio
		Width
		Height
		Left
		Top
	End Enum

	''' <summary>
	''' Indicates the type of relationship between the base control property and the
	''' relative control property
	''' </summary>
	Public Enum LayoutOperation
		''' <summary>
		''' Indicates that the base control and the reference control's properties always
		''' differ by the same amount. 
		''' </summary>
		Difference
		''' <summary>
		''' Indicates that the base control and the reference control's properties 
		''' differ by a ratio amount
		''' </summary>
		''' <example>Used if when the base control width changes by 2 pixels, the
		''' relative control width changes by 1 pixel.</example>
		Ratio
	End Enum
End Namespace

