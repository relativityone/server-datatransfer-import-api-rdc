Imports System.Collections.Generic
Imports kCura.Windows.Forms

Namespace kCura.EDDS.WinForm
	Public Class EncodingForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call

		End Sub

		'Form overrides dispose to clean up the component list.
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
		Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
		Friend WithEvents EncodingList As System.Windows.Forms.ListBox
		Friend WithEvents Cancel As System.Windows.Forms.Button
		Friend WithEvents OK As System.Windows.Forms.Button

		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.TextBox1 = New System.Windows.Forms.TextBox
			Me.EncodingList = New System.Windows.Forms.ListBox
			Me.Cancel = New System.Windows.Forms.Button
			Me.OK = New System.Windows.Forms.Button
			Me.SuspendLayout()
			'
			'TextBox1
			'
			Me.TextBox1.Location = New System.Drawing.Point(0, 0)
			Me.TextBox1.Name = "TextBox1"
			Me.TextBox1.Size = New System.Drawing.Size(292, 20)
			Me.TextBox1.TabIndex = 1
			Me.TextBox1.Text = ""
			Me.TextBox1.Anchor = CType(System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left _
			 Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
			'
			'EncodingList
			'
			Me.EncodingList.Location = New System.Drawing.Point(0, 28)
			Me.EncodingList.Name = "EncodingList"
			Me.EncodingList.Size = New System.Drawing.Size(292, 212)
			Me.EncodingList.TabIndex = 3
			Me.EncodingList.Anchor = CType(System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left _
			 Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
			'
			'Cancel
			'
			Me.Cancel.Location = New System.Drawing.Point(216, 244)
			Me.Cancel.Name = "Cancel"
			Me.Cancel.TabIndex = 6
			Me.Cancel.Text = "Cancel"
			Me.Cancel.Anchor = CType(System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
			'
			'OK
			'
			Me.OK.Location = New System.Drawing.Point(136, 244)
			Me.OK.Name = "OK"
			Me.OK.TabIndex = 5
			Me.OK.Text = "OK"
			Me.OK.Anchor = CType(System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
			'
			'EncodingForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(292, 269)
			Me.Controls.Add(Me.Cancel)
			Me.Controls.Add(Me.OK)
			Me.Controls.Add(Me.EncodingList)
			Me.Controls.Add(Me.TextBox1)
			Me.Name = "EncodingForm"
			Me.Text = "Pick Encoding"
			Me.ResumeLayout(False)

		End Sub

#End Region
		Private _comboBox As ComboBox
		Public Property DropDownToUpdate() As ComboBox
			Get
				Return _comboBox
			End Get
			Set(ByVal value As ComboBox)
				_comboBox = value
			End Set
		End Property

		Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
			EncodingList.Items.Clear()
			If TextBox1.Text <> "" Then
				For Each enc As EncodingItem In Constants.AllEncodings
					If enc.ToString.ToLower.IndexOf(TextBox1.Text.ToLower) <> -1 Then
						EncodingList.Items.Add(enc)
					End If
				Next
			Else
				EncodingList.Items.AddRange(Constants.AllEncodings)
			End If
		End Sub

		Private Sub EncodingForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			EncodingList.Items.Clear()
			EncodingList.Items.AddRange(Constants.AllEncodings)
		End Sub

		Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
			Me.Close()
		End Sub

		Public Event EncodingOK(ByVal ei As EncodingItem)


		Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click, EncodingList.DoubleClick
			If Not EncodingList.SelectedItem Is Nothing Then
				RaiseEvent EncodingOK(DirectCast(EncodingList.SelectedItem, EncodingItem))
				Me.Close()
			End If
		End Sub

		'#Region "Resizing"
		'		'These member variables are populated with data needed to resize the controls

		'		'Avoid adjusting the layout if the size hasn't changed
		'		Private _layoutControlSize As Size

		'		' Used to keep track of whether we need to calculate the layout values.  In addition to
		'		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		'		' will change the distance between concrols which we would not expect to change.  If this
		'		' happens, the _layout info which contains the relative location of controls needs to be 
		'		' updated.
		'		Private _layoutReferenceDistance As Int32 = 0

		'		Private _layoutDifferenceList As List(Of kCura.Windows.Forms.RelativeLayoutData)

		'		Private Function CalcReferenceDistance() As Int32
		'			Return Me.AutoScaleBaseSize.Width * Me.AutoScaleBaseSize.Height
		'		End Function

		'		Private Sub OnControl_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
		'			If _layoutReferenceDistance <> CalcReferenceDistance() Then
		'				InitializeLayout()
		'			Else
		'				AdjustLayout()
		'			End If
		'		End Sub

		'		Private Sub InitializeLayout()
		'			'Layout properties which are directly based on another layout property
		'			If _layoutDifferenceList Is Nothing Then
		'				_layoutDifferenceList = New List(Of RelativeLayoutData)

		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, EncodingList, LayoutPropertyType.Height))
		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, EncodingList, LayoutPropertyType.Width))
		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, EncodingList, LayoutPropertyType.Width))

		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, Me.OK, LayoutPropertyType.Left))
		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, Me.OK, LayoutPropertyType.Top))
		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, Me.Cancel, LayoutPropertyType.Left))
		'				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, Me.Cancel, LayoutPropertyType.Top))
		'			End If

		'			_layoutDifferenceList.ForEach(Sub(x)
		'																			x.InitializeDifference()
		'																		End Sub)

		'			_layoutReferenceDistance = CalcReferenceDistance()
		'		End Sub

		'		Public Sub AdjustLayout()
		'			If Not _layoutControlSize.Equals(Me.Size) Then
		'				For Each x As RelativeLayoutData In _layoutDifferenceList
		'					x.AdjustRelativeControlBasedOnDifference()
		'				Next

		'				_layoutControlSize = Me.Size
		'			End If
		'		End Sub
		'#End Region


	End Class

	Public Class EncodingItem
		Private _encoding As System.Text.Encoding

		Public ReadOnly Property Encoding() As System.Text.Encoding
			Get
				Return _encoding
			End Get
		End Property

		Public Shared Function GetEncodingItemFromCodePageId(ByVal codePageID As Int32) As EncodingItem
			Try
				Return New EncodingItem(codePageID)
			Catch ex As Exception
				Return Nothing
			End Try
		End Function

		Private Sub New(ByVal codePageId As Int32)
			_encoding = System.Text.Encoding.GetEncoding(codePageId)
		End Sub

		Public ReadOnly Property CodePageId() As Int32
			Get
				Return _encoding.CodePage
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return _encoding.EncodingName
		End Function

		Public Class EncodingItemComparer
			Implements IComparer

			Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
				Return String.Compare(x.ToString, y.ToString)
			End Function
		End Class
	End Class
End Namespace


