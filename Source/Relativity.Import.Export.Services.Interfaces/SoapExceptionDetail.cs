namespace Relativity.Import.Export.Services
{
	public partial class SoapExceptionDetail
	{
		private string[] _DetailsField;

		private string _ExceptionFullTextField;

		private string _ExceptionMessageField;

		private string _ExceptionTraceField;

		private string _ExceptionTypeField;

		public string[] _Details
		{
			get
			{
				return this._DetailsField;
			}
			set
			{
				this._DetailsField = value;
			}
		}

		public string _ExceptionFullText
		{
			get
			{
				return this._ExceptionFullTextField;
			}
			set
			{
				this._ExceptionFullTextField = value;
			}
		}

		public string _ExceptionMessage
		{
			get
			{
				return this._ExceptionMessageField;
			}
			set
			{
				this._ExceptionMessageField = value;
			}
		}

		public string _ExceptionTrace
		{
			get
			{
				return this._ExceptionTraceField;
			}
			set
			{
				this._ExceptionTraceField = value;
			}
		}

		public string _ExceptionType
		{
			get
			{
				return this._ExceptionTypeField;
			}
			set
			{
				this._ExceptionTypeField = value;
			}
		}
	}
}