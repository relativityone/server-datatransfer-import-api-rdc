
using System;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportExceptionHandlerExec
	{
		T TryCatchExec<T>(Func<T> executeAction, T defaultRetValue = default(T), Action finalizeAction = null);

		void TryCatchExec(Action executeAction, Action finalizeAction = null);

		void IgnoreOnExceprionExec<TException>(Action action, Func<bool> condition = null);
	}
}
