Namespace kCura.EDDS.WebAPI.TemplateManagerBase
	''' <summary>
	''' This class is used as a placeholder in the listboxes on the four list field map control.
	''' </summary>
	Partial Public Class TargetField
		Public Shared ReadOnly Property Empty() As TargetField
			Get
				Return New TargetField() With {.MyName = " - "}
			End Get
		End Property
		Public ReadOnly Property IsEmpty() As Boolean
			Get
				Return Me.ArtifactID = 0
			End Get
		End Property
	End Class
End Namespace

