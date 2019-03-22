namespace Relativity.Import.Export.Services
{
	public partial class ErrorFileKey
	{
		private string LogKeyField;

		private string OpticonKeyField;

		public string LogKey
		{
			get
			{
				return this.LogKeyField;
			}
			set
			{
				this.LogKeyField = value;
			}
		}

		public string OpticonKey
		{
			get
			{
				return this.OpticonKeyField;
			}
			set
			{
				this.OpticonKeyField = value;
			}
		}
	}
}