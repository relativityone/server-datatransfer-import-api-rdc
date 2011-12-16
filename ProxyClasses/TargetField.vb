Namespace kCura.EDDS.WebAPI.TemplateManagerBase
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

