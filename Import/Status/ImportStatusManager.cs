using System;
namespace kCura.WinEDDS.Core.Import.Status
{
	public class ImportStatusManager : IImportStatusManager
	{
		private ImportContext _importContext;

		public event EventHandler<ImportStatusEventArgs> StatusChanged;

		#region Interface Methods

		public void RaiseStartImportEvent(object sender)
		{
			StatusChanged?.Invoke(sender, CreateStatusArgs(ImportProcessStatus.Start, string.Empty));
		}

		public void RaiseEndImportEvent(object sender)
		{
			StatusChanged?.Invoke(sender, CreateStatusArgs(ImportProcessStatus.End, string.Empty));
		}

		public void RaiseErrorImportEvent(object sender, string message, int recordIndex)
		{
			StatusChanged?.Invoke(sender, CreateStatusArgs(ImportProcessStatus.Error, message, recordIndex));
		}

		public void RaiseUpdateImportEvent(object sender, string message, int recordIndex)
		{
			StatusChanged?.Invoke(sender, CreateStatusArgs(ImportProcessStatus.Update, message, recordIndex));
		}
		public void ReiseStatusChangedEvent(object sender, ImportStatusEventArgs args)
		{
			StatusChanged?.Invoke(sender, args);
		}

		public void RaiseFatalErrorImportEvent(object sender, string message, int recordIndex, Exception ex)
		{
			ImportStatusEventArgs args = CreateStatusArgs(ImportProcessStatus.FatalError, message, recordIndex, ex);
			args.JobRunId = _importContext?.JobRunId;
			StatusChanged?.Invoke(sender, args);
		}

		public void RaiseTranserModeChangedEvent(object sender, string message)
		{
			StatusChanged?.Invoke(sender, CreateStatusArgs(ImportProcessStatus.TransferModeChanged, message));
		}

	public void OnSetJobContext(object sender, ImportContext importContext)
		{
			_importContext = importContext;
		}

		#endregion Interface Methods

		#region Private Methods

		private ImportStatusEventArgs CreateStatusArgs(ImportProcessStatus state, string msg)
		{
			return new ImportStatusEventArgs(state, msg);
		}

		private ImportStatusEventArgs CreateStatusArgs(ImportProcessStatus state, string msg, int recordIndex)
		{
			return new ImportStatusEventArgs(state, msg, recordIndex);
		}

		private ImportStatusEventArgs CreateStatusArgs(ImportProcessStatus state, string msg, int recordIndex, Exception ex)
		{
			return new ImportStatusEventArgs(state, msg, recordIndex, ex);
		}

		#endregion Private Methods
	}
}
