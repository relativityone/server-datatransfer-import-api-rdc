Imports kCura.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic

Namespace kCura.WinEDDS.UIControls

	Public Class FieldMap
		Inherits System.Windows.Forms.UserControl

		Public Event FieldColumnsItemsShifted(ByVal sender As Object, ByVal e As EventArgs)
		Public Event LoadFileColumnsItemsShifted(ByVal sender As Object, ByVal e As EventArgs)

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call

		End Sub

		'UserControl overrides dispose to clean up the component list.
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not (components Is Nothing) Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		Friend WithEvents _fieldColumnsLabel As System.Windows.Forms.Label
		Friend WithEvents _loadFileColumnsLabel As System.Windows.Forms.Label
		Friend WithEvents _fieldColumns As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _loadFileColumns As kCura.Windows.Forms.TwoListBox

		Private Sub InitializeComponent()
			Me._fieldColumnsLabel = New System.Windows.Forms.Label
			Me._loadFileColumnsLabel = New System.Windows.Forms.Label
			Me._fieldColumns = New kCura.Windows.Forms.TwoListBox
			Me._loadFileColumns = New kCura.Windows.Forms.TwoListBox
			Me.SuspendLayout()
			Me.DoubleBuffered = True
			'
			'_fieldColumnsLabel
			'
			Me._fieldColumnsLabel.Name = "_fieldColumnsLabel"
			Me._fieldColumnsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._fieldColumnsLabel.Location = New System.Drawing.Point(0, 4)
			Me._fieldColumnsLabel.Size = New System.Drawing.Size(145, 16)
			Me._fieldColumnsLabel.TabIndex = 4
			Me._fieldColumnsLabel.Text = "Workspace Fields"
			Me._fieldColumnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'_loadFileColumnsLabel
			'
			Me._loadFileColumnsLabel.Name = "_loadFileColumnsLabel"
			Me._loadFileColumnsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._loadFileColumnsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._loadFileColumnsLabel.Location = New System.Drawing.Point(574, 4)
			Me._loadFileColumnsLabel.Size = New System.Drawing.Size(140, 16)
			Me._loadFileColumnsLabel.TabIndex = 7
			Me._loadFileColumnsLabel.Text = "Load File Fields"
			Me._loadFileColumnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_fieldColumns
			'
			Me._fieldColumns.Name = "_fieldColumns"
			Me._fieldColumns.Location = New System.Drawing.Point(0, 20)
			Me._fieldColumns.Size = New System.Drawing.Size(356, 264)
			Me._fieldColumns.AlternateRowColors = True
			Me._fieldColumns.KeepButtonsCentered = True
			Me._fieldColumns.LeftOrderControlsVisible = False
			Me._fieldColumns.RightOrderControlVisible = True
			Me._fieldColumns.TabIndex = 8
			Me._fieldColumns.OuterBox = kCura.Windows.Forms.ListBoxLocation.Left
			'
			'_loadFileColumns
			'
			Me._loadFileColumns.Name = "_loadFileColumns"
			Me._loadFileColumns.Location = New System.Drawing.Point(360, 20)
			Me._loadFileColumns.Size = New System.Drawing.Size(356, 264)
			Me._loadFileColumns.AlternateRowColors = True
			Me._loadFileColumns.KeepButtonsCentered = True
			Me._loadFileColumns.LeftOrderControlsVisible = True
			Me._loadFileColumns.RightOrderControlVisible = False
			Me._loadFileColumns.TabIndex = 9
			Me._loadFileColumns.OuterBox = kCura.Windows.Forms.ListBoxLocation.Right
			'
			'FieldMap
			'
			Me.Controls.Add(Me._loadFileColumns)
			Me.Controls.Add(Me._fieldColumns)
			Me.Controls.Add(Me._loadFileColumnsLabel)
			Me.Controls.Add(Me._fieldColumnsLabel)
			Me.Name = "FieldMap"
			Me.Size = New System.Drawing.Size(716, 288)

			Me.ResumeLayout(False)

		End Sub

#End Region

#Region "Resizing"
		'These member variables are populated with data needed to resize the controls

		'Avoid adjusting the layout if the size hasn't changed
		Private _layoutControlSize As Size

		' Used to keep track of whether we need to calculate the layout values.  In addition to
		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		' will change the distance between concrols which we would not expect to change.  If this
		' happens, the _layout info which contains the relative location of controls needs to be 
		' updated.
		Private _layoutReferenceDistance As Int32 = 0

		Private _layoutRatioList As List(Of RelativeLayoutData)
		Private _layoutDifferenceList As List(Of RelativeLayoutData)

		Private Function CalcReferenceDistance() As Int32
			Return _fieldColumnsLabel.Width
		End Function

		Private Sub OnControl_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			System.Diagnostics.Debug.WriteLine("LoadFileForm.OnForm_Layout occurred.  Width=" + Me.Width.ToString() + ", Height=" + Me.Height.ToString())

			If _layoutReferenceDistance <> CalcReferenceDistance() Then
				InitializeLayout()
			Else
				AdjustLayout()
			End If
		End Sub

		Private Sub InitializeLayout()
			_layoutControlSize = Me.Size

			'Layout properties which are based on a ratio to another layout property. 
			If _layoutRatioList Is Nothing Then
				_layoutRatioList = New List(Of RelativeLayoutData)

				'When the width of the dialog increases by 2 pixels, each groupbox increases by 1 pixel.  The ratio is 1/2 = .5
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _fieldColumns, LayoutPropertyType.Width, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _loadFileColumns, LayoutPropertyType.Width, 0.5))
			End If

			_layoutRatioList.ForEach(Sub(x)
																 x.InitalizeRatioValues()
															 End Sub)

			'Layout properties which are directly based on another layout property
			If _layoutDifferenceList Is Nothing Then
				_layoutDifferenceList = New List(Of RelativeLayoutData)

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _fieldColumns, LayoutPropertyType.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _loadFileColumns, LayoutPropertyType.Height))

				_layoutDifferenceList.Add(New RelativeLayoutData(_fieldColumns, LayoutPropertyType.Right, _loadFileColumns, LayoutPropertyType.Left))

				' No need to adjust _loadFileColumnsLabel because it is using anchoring
			End If

			_layoutDifferenceList.ForEach(Sub(x)
																			x.InitializeDifference()
																		End Sub)

			_layoutReferenceDistance = CalcReferenceDistance()
		End Sub

		Public Sub AdjustLayout()
			
			If Not _layoutControlSize.Equals(Me.Size) Then
				For Each x As RelativeLayoutData In _layoutRatioList
					x.AdjustRelativeControlBasedOnRatio()
				Next

				For Each x As RelativeLayoutData In _layoutDifferenceList
					x.AdjustRelativeControlBasedOnDifference()
				Next

				_layoutControlSize = Me.Size
			End If
		End Sub
#End Region

#Region "Properties"

		Public Property FieldColumns() As kCura.Windows.Forms.TwoListBox
			Get
				Return _fieldColumns
			End Get
			Set(ByVal Value As kCura.Windows.Forms.TwoListBox)
				_fieldColumns = Value
			End Set
		End Property

		Public Property LoadFileColumns() As kCura.Windows.Forms.TwoListBox
			Get
				Return _loadFileColumns
			End Get
			Set(ByVal Value As kCura.Windows.Forms.TwoListBox)
				_loadFileColumns = Value
			End Set
		End Property

#End Region

		Public Sub MapCaseFieldsToLoadFileFields(ByVal caseFields As String(), ByVal columnHeaders As String(), ByVal selectedFieldNameList As System.Collections.ArrayList, ByVal selectedColumnNameList As System.Collections.ArrayList)
			Dim selectedFieldNames As String() = DirectCast(selectedFieldNameList.ToArray(GetType(String)), String())
			Dim selectedColumnNames As String() = DirectCast(selectedColumnNameList.ToArray(GetType(String)), String())
			Me.FieldColumns.RightListBoxItems.AddRange(selectedFieldNames)
			Me.LoadFileColumns.LeftListBoxItems.AddRange(selectedColumnNames)
			Dim name As String
			For Each name In caseFields
				If Array.IndexOf(selectedFieldNames, name) = -1 Then
					Me.FieldColumns.LeftListBoxItems.Add(name)
				End If
			Next
			For Each name In columnHeaders
				If Array.IndexOf(selectedColumnNames, name) = -1 Then
					Me.LoadFileColumns.RightListBoxItems.Add(name)
				End If
			Next
		End Sub

		Public Sub ClearAll()
			_fieldColumns.ClearAll()
			_loadFileColumns.ClearAll()
		End Sub

#Region " Event handlers "
		Private Sub _FieldColumns_ItemsShifted() Handles _fieldColumns.ItemsShifted
			RaiseEvent FieldColumnsItemsShifted(Me, New EventArgs)
		End Sub

		Private Sub _LoadFileColumns_ItemsShifted() Handles _loadFileColumns.ItemsShifted
			RaiseEvent LoadFileColumnsItemsShifted(Me, New EventArgs)
		End Sub

		Private Sub _fieldColumns_ClearHighlightedItems(ByVal sender As Object, ByVal e As HighlightItemEventArgs) Handles _fieldColumns.ClearHighlightedItems
			_loadFileColumns.ClearHighlight(e.Location)
		End Sub

		Private Sub _fieldColumns_HighlightItemByLocationAndIndex(ByVal sender As Object, ByVal e As HighlightItemEventArgs) Handles _fieldColumns.HighlightItemByLocationAndIndex
			_loadFileColumns.HighlightItembyIndex(e.Index, e.Location)
		End Sub

		Private Sub _loadFileColumns_ClearHighlightedItems(ByVal sender As Object, ByVal e As HighlightItemEventArgs) Handles _loadFileColumns.ClearHighlightedItems
			_fieldColumns.ClearHighlight(e.Location)
		End Sub

		Private Sub _loadFileColumns_HighlightItemByLocationAndIndex(ByVal sender As Object, ByVal e As HighlightItemEventArgs) Handles _loadFileColumns.HighlightItemByLocationAndIndex
			_fieldColumns.HighlightItembyIndex(e.Index, e.Location)
		End Sub
#End Region
	End Class

End Namespace