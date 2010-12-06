Namespace kCura.WinEDDS.UIControls

	Public Class FieldMap
		Inherits System.Windows.Forms.UserControl

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
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me._fieldColumnsLabel = New System.Windows.Forms.Label
			Me._loadFileColumnsLabel = New System.Windows.Forms.Label
			Me._fieldColumns = New kCura.Windows.Forms.TwoListBox
			Me._loadFileColumns = New kCura.Windows.Forms.TwoListBox
			Me.SuspendLayout()
			'
			'_fieldColumnsLabel
			'
			Me._fieldColumnsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._fieldColumnsLabel.Location = New System.Drawing.Point(0, 8)
			Me._fieldColumnsLabel.Name = "_fieldColumnsLabel"
			Me._fieldColumnsLabel.Size = New System.Drawing.Size(145, 16)
			Me._fieldColumnsLabel.TabIndex = 4
			Me._fieldColumnsLabel.Text = "Workspace Fields"
			Me._fieldColumnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'_loadFileColumnsLabel
			'
			Me._loadFileColumnsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._loadFileColumnsLabel.Location = New System.Drawing.Point(572, 8)
			Me._loadFileColumnsLabel.Name = "_loadFileColumnsLabel"
			Me._loadFileColumnsLabel.Size = New System.Drawing.Size(140, 16)
			Me._loadFileColumnsLabel.TabIndex = 7
			Me._loadFileColumnsLabel.Text = "Load File Fields"
			Me._loadFileColumnsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_fieldColumns
			'
			Me._fieldColumns.AlternateRowColors = True
			Me._fieldColumns.KeepButtonsCentered = True
			Me._fieldColumns.LeftOrderControlsVisible = False
			Me._fieldColumns.Location = New System.Drawing.Point(0, 24)
			Me._fieldColumns.Name = "_fieldColumns"
			Me._fieldColumns.RightOrderControlVisible = True
			Me._fieldColumns.Size = New System.Drawing.Size(364, 276)
			Me._fieldColumns.TabIndex = 8
			Me._fieldColumns.OuterBox = kCura.Windows.Forms.TwoListBox.ListBoxLocation.Left
			'
			'_loadFileColumns
			'
			Me._loadFileColumns.AlternateRowColors = True
			Me._loadFileColumns.KeepButtonsCentered = True
			Me._loadFileColumns.LeftOrderControlsVisible = True
			Me._loadFileColumns.Location = New System.Drawing.Point(360, 24)
			Me._loadFileColumns.Name = "_loadFileColumns"
			Me._loadFileColumns.RightOrderControlVisible = False
			Me._loadFileColumns.Size = New System.Drawing.Size(388, 276)
			Me._loadFileColumns.TabIndex = 9
			Me._loadFileColumns.OuterBox = kCura.Windows.Forms.TwoListBox.ListBoxLocation.Right
			'
			'FieldMap
			'
			Me.Controls.Add(Me._loadFileColumns)
			Me.Controls.Add(Me._fieldColumns)
			Me.Controls.Add(Me._loadFileColumnsLabel)
			Me.Controls.Add(Me._fieldColumnsLabel)
			Me.Name = "FieldMap"
			Me.Size = New System.Drawing.Size(732, 292)
			Me.ResumeLayout(False)

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

		Private Sub _FieldColumns_ItemsShifted() Handles _fieldColumns.ItemsShifted
			RaiseEvent FieldColumnsItemsShifted()
		End Sub

		Private Sub _LoadFileColumns_ItemsShifted() Handles _loadFileColumns.ItemsShifted
			RaiseEvent LoadFileColumnsItemsShifted()
		End Sub

		Public Event FieldColumnsItemsShifted()
		Public Event LoadFileColumnsItemsShifted()

	End Class

End Namespace