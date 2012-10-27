Imports System.Windows.Forms

Namespace kCura.Windows.Forms
	Public Class RelativeLayoutData
		Public Property BaseControl() As Control
		Public Property BaseControlPropertyType() As LayoutPropertyType
		Public Property RelativeControl() As Control

		Private _relativeControlPropertyType As LayoutPropertyType
		Public Property RelativeControlPropertyType() As LayoutPropertyType
			Get
				Return _relativeControlPropertyType
			End Get
			Set(value As LayoutPropertyType)
				'These are read-only types.  Since relativeControl properties may be overwritten, 
				'it is not valid for the relative control to use one of these
				System.Diagnostics.Debug.Assert(value <> LayoutPropertyType.Right)
				System.Diagnostics.Debug.Assert(value <> LayoutPropertyType.Bottom)
				_relativeControlPropertyType = value
			End Set
		End Property
		Public Property Difference As Int32

		Public Sub New()
		End Sub

		Public Sub New(baseControl As Control, baseControlPropertyType As LayoutPropertyType, _
			relativeControl As Control, relativeControlPropertyType As LayoutPropertyType)
			'These are read-only types.  Since relativeControl properties may be overwritten, 
			'it is not valid for the relative control to use one of these
			System.Diagnostics.Debug.Assert(relativeControlPropertyType <> LayoutPropertyType.Right)
			System.Diagnostics.Debug.Assert(relativeControlPropertyType <> LayoutPropertyType.Bottom)

			Me.BaseControl = baseControl
			Me.BaseControlPropertyType = baseControlPropertyType
			Me.RelativeControl = relativeControl
			Me.RelativeControlPropertyType = relativeControlPropertyType
		End Sub

		Public Sub InitializeDifference()
			Difference = RelativePropertyValue - BasePropertyValue
		End Sub

		Public Sub AdjustRelativeControlBasedOnDifference()
			Select Case Me.RelativeControlPropertyType
				Case LayoutPropertyType.Left
					RelativeControl.Left = BasePropertyValue + Difference
				Case LayoutPropertyType.Top
					RelativeControl.Top = BasePropertyValue + Difference
				Case LayoutPropertyType.Width
					RelativeControl.Width = BasePropertyValue + Difference
				Case LayoutPropertyType.Height
					RelativeControl.Height = BasePropertyValue + Difference
				Case Else
					System.Diagnostics.Debug.Assert(False)
					Throw New Exception("Unexpected RelativeControlPropertyType: " + Me.RelativeControlPropertyType.ToString())
			End Select
		End Sub

		Public ReadOnly Property BasePropertyValue() As Int32
			Get
				Select Case Me.BaseControlPropertyType
					Case LayoutPropertyType.Left
						Return Me.BaseControl.Left
					Case LayoutPropertyType.Top
						Return Me.BaseControl.Top
					Case LayoutPropertyType.Width
						Return Me.BaseControl.Width
					Case LayoutPropertyType.Height
						Return Me.BaseControl.Height
					Case LayoutPropertyType.Right
						Return Me.BaseControl.Right
					Case LayoutPropertyType.Bottom
						Return Me.BaseControl.Bottom
					Case Else
						System.Diagnostics.Debug.Assert(False)
						Throw New Exception("Unexpected BaseControlPropertyType: " + Me.BaseControlPropertyType.ToString())
				End Select
			End Get
		End Property

		Public ReadOnly Property RelativePropertyValue() As Int32
			Get
				Select Case Me.RelativeControlPropertyType
					Case LayoutPropertyType.Left
						Return Me.RelativeControl.Left
					Case LayoutPropertyType.Top
						Return Me.RelativeControl.Top
					Case LayoutPropertyType.Width
						Return Me.RelativeControl.Width
					Case LayoutPropertyType.Height
						Return Me.RelativeControl.Height
					Case Else
						System.Diagnostics.Debug.Assert(False)
						Throw New Exception("Unexpected RelativeControlPropertyType: " + Me.RelativeControlPropertyType.ToString())
				End Select
			End Get
		End Property
	End Class
End Namespace

