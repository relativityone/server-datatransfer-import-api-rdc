NotInheritable Class EpochDateConverter

	Private Sub New()

	End Sub

	Private Const EpochStartDate As Date = #1/1/1970#

	Public Shared Function ConvertDateTimeToEpoch(ByVal dateToConvert As Date) As Double
		Return (dateToConvert - EpochStartDate).TotalSeconds
	End Function

End Class
