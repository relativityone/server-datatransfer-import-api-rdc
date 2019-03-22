namespace Relativity.Import.Export.Services
{
	public enum FieldInfoImportBehaviorChoice : int
	{

		LeaveBlankValuesUnchanged = 1,

		ReplaceBlankValuesWithIdentifier = 2,

		ObjectFieldContainsArtifactId = 3,
	}
}