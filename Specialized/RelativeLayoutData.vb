Imports System.Windows.Forms
Imports System.Linq
Namespace kCura.Windows.Forms
	Public Class RelativeLayoutData
		Public Property BaseControl() As Control
		Public Property BaseControlPropertyTypeForDifference() As LayoutBasePropertyTypeForDifference
		Public Property BaseControlPropertyTypeForRatio() As LayoutBasePropertyTypeForRatio
		Public Property RelativeControlPropertyTypeForDifference() As LayoutRelativePropertyTypeForDifference
		Public Property RelativeControlPropertyTypeForRatio() As LayoutRelativePropertyTypeForRatio
		Public Property RelativeControl() As Control

		'Private _relativeControlPropertyType As LayoutPropertyType
		'Public Property RelativeControlPropertyTypeForDifference() As LayoutPropertyType
		'	Get
		'		Return _relativeControlPropertyType
		'	End Get
		'	Set(value As LayoutPropertyType)
		'		'These are read-only types.  Since relativeControl properties may be overwritten, 
		'		'it is not valid for the relative control to use one of these
		'		System.Diagnostics.Debug.Assert(value <> LayoutPropertyType.Right)
		'		System.Diagnostics.Debug.Assert(value <> LayoutPropertyType.Bottom)
		'		_relativeControlPropertyType = value
		'	End Set
		'End Property

		Public Property Operation As LayoutOperation

		Private _difference As Int32 = 0
		Public Property Difference() As Int32
			Get
				Return _difference
			End Get
			Set(value As Int32)
				_difference = value
				Me.Operation = LayoutOperation.Difference
			End Set
		End Property

		Private _fraction As Double = 1.0
		Public Property Fraction() As Double
			Get
				Return _fraction
			End Get
			Set(value As Double)
				'These are read-only types.  Since relativeControl properties may be overwritten, 
				'it is not valid for the relative control to use one of these
				_fraction = value
				Me.Operation = LayoutOperation.Ratio
			End Set
		End Property

		Public Property InitialBaseValue As Int32
		Public Property InitialRelativeValue As Int32

		Public Sub New()
		End Sub

		Public Sub New(baseControl As Control, baseControlPropertyType As LayoutBasePropertyTypeForDifference, _
		relativeControl As Control, relativeControlPropertyType As LayoutRelativePropertyTypeForDifference)
			Me.BaseControl = baseControl
			Me.BaseControlPropertyTypeForDifference = baseControlPropertyType
			Me.RelativeControl = relativeControl
			Me.RelativeControlPropertyTypeForDifference = relativeControlPropertyType
			Me.Operation = LayoutOperation.Difference
		End Sub

		Public Sub New(baseControl As Control, baseControlPropertyType As LayoutBasePropertyTypeForRatio, _
		 relativeControl As Control, relativeControlPropertyType As LayoutRelativePropertyTypeForRatio, _
		 fraction As Double)

			Me.BaseControl = baseControl
			Me.BaseControlPropertyTypeForRatio = baseControlPropertyType
			Me.RelativeControl = relativeControl
			Me.RelativeControlPropertyTypeForRatio = relativeControlPropertyType
			Me.Operation = LayoutOperation.Ratio
			Me.Fraction = fraction
		End Sub

		Public Sub InitializeDifference()
			Me.Operation = LayoutOperation.Difference
			Difference = RelativePropertyValue - BasePropertyValue
		End Sub

		Public Sub InitalizeRatioValues()
			Me.Operation = LayoutOperation.Ratio
			Me.InitialBaseValue = BasePropertyValue
			Me.InitialRelativeValue = RelativePropertyValue
		End Sub

		Public Sub AdjustRelativeControlBasedOnDifference()
			System.Diagnostics.Debug.Assert(Me.Operation = LayoutOperation.Difference)
			Select Case Me.RelativeControlPropertyTypeForDifference
				Case LayoutRelativePropertyTypeForDifference.Left
					RelativeControl.Left = BasePropertyValue + Difference
				Case LayoutRelativePropertyTypeForDifference.Top
					RelativeControl.Top = BasePropertyValue + Difference
				Case LayoutRelativePropertyTypeForDifference.Width
					RelativeControl.Width = BasePropertyValue + Difference
				Case LayoutRelativePropertyTypeForDifference.Height
					RelativeControl.Height = BasePropertyValue + Difference
				Case Else
					System.Diagnostics.Debug.Assert(False)
					Throw New Exception("Unexpected RelativeControlPropertyType: " + Me.RelativeControlPropertyTypeForDifference.ToString())
			End Select
		End Sub

		Public Sub AdjustRelativeControlBasedOnRatio()
			System.Diagnostics.Debug.Assert(Me.Operation = LayoutOperation.Ratio)
			Dim baseDifference As Int32 = BasePropertyValue - InitialBaseValue
			Select Case Me.RelativeControlPropertyTypeForRatio
				Case LayoutRelativePropertyTypeForRatio.Width
					RelativeControl.Width = InitialRelativeValue + CInt(Math.Round(baseDifference * Fraction))
				Case LayoutRelativePropertyTypeForRatio.Height
					RelativeControl.Height = InitialRelativeValue + CInt(Math.Round(baseDifference * Fraction))
				Case LayoutRelativePropertyTypeForRatio.Left
					RelativeControl.Left = InitialRelativeValue + CInt(Math.Round(baseDifference * Fraction))
				Case LayoutRelativePropertyTypeForRatio.Top
					RelativeControl.Top = InitialRelativeValue + CInt(Math.Round(baseDifference * Fraction))
				Case Else
					System.Diagnostics.Debug.Assert(False)
					Throw New Exception("Unexpected RelativeControlPropertyType: " + Me.RelativeControlPropertyTypeForRatio.ToString())
			End Select
		End Sub

		Public ReadOnly Property BasePropertyValue() As Int32
			Get
				Dim value As Int32 = 0

				If Me.Operation = LayoutOperation.Difference Then
					Select Case Me.BaseControlPropertyTypeForDifference
						Case LayoutBasePropertyTypeForDifference.Left
							value = Me.BaseControl.Left
						Case LayoutBasePropertyTypeForDifference.Top
							value = Me.BaseControl.Top
						Case LayoutBasePropertyTypeForDifference.Width
							value = Me.BaseControl.Width
						Case LayoutBasePropertyTypeForDifference.Height
							value = Me.BaseControl.Height
						Case LayoutBasePropertyTypeForDifference.Right
							value = Me.BaseControl.Right
						Case LayoutBasePropertyTypeForDifference.Bottom
							value = Me.BaseControl.Bottom
						Case Else
							System.Diagnostics.Debug.Assert(False)
							Throw New Exception("Unexpected BaseControlPropertyTypeForDifference: " + Me.BaseControlPropertyTypeForDifference.ToString())
					End Select
				Else
					System.Diagnostics.Debug.Assert(Me.Operation = LayoutOperation.Ratio)
					Select Case Me.BaseControlPropertyTypeForRatio
						Case LayoutBasePropertyTypeForRatio.Width
							value = Me.BaseControl.Width
						Case LayoutBasePropertyTypeForRatio.Height
							value = Me.BaseControl.Height
						Case Else
							System.Diagnostics.Debug.Assert(False)
							Throw New Exception("Unexpected BaseControlPropertyTypeForRatio: " + Me.BaseControlPropertyTypeForRatio.ToString())
					End Select
				End If

				Return value
			End Get
		End Property

		Public ReadOnly Property RelativePropertyValue() As Int32
			Get
				Dim value As Int32 = 0
				If Me.Operation = LayoutOperation.Difference Then
					Select Case Me.RelativeControlPropertyTypeForDifference
						Case LayoutRelativePropertyTypeForDifference.Left
							Return Me.RelativeControl.Left
						Case LayoutRelativePropertyTypeForDifference.Top
							Return Me.RelativeControl.Top
						Case LayoutRelativePropertyTypeForDifference.Width
							Return Me.RelativeControl.Width
						Case LayoutRelativePropertyTypeForDifference.Height
							Return Me.RelativeControl.Height
						Case Else
							System.Diagnostics.Debug.Assert(False)
							Throw New Exception("Unexpected RelativeControlPropertyTypeForDifference: " + Me.RelativeControlPropertyTypeForDifference.ToString())
					End Select
				Else
					System.Diagnostics.Debug.Assert(Me.Operation = LayoutOperation.Ratio)
					Select Case Me.RelativeControlPropertyTypeForRatio
						Case LayoutRelativePropertyTypeForRatio.Width
							value = Me.RelativeControl.Width
						Case LayoutRelativePropertyTypeForRatio.Height
							value = Me.RelativeControl.Height
						Case LayoutRelativePropertyTypeForRatio.Left
							value = Me.RelativeControl.Left
						Case LayoutRelativePropertyTypeForRatio.Top
							value = Me.RelativeControl.Top
						Case Else
							System.Diagnostics.Debug.Assert(False)
							Throw New Exception("Unexpected RelativeControlPropertyTypeForRatio: " + Me.RelativeControlPropertyTypeForRatio.ToString())
					End Select
				End If

				Return value
			End Get
		End Property
	End Class
End Namespace

