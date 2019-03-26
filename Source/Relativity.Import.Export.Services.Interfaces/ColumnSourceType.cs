namespace Relativity.Import.Export.Services
{
	public enum ColumnSourceType : int
	{

		Computed = 0,

		Artifact = 1,

		MetadataJoin = 2,

		MainTable = 3,

		NoDatabaseColumn = 4,
	}
}