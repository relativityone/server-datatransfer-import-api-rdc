using System;
using Relativity.Logging;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Base class for Files Importertes
    /// </summary>
    public abstract class ImportTapiBase
    {
        private ILog _log;
        
        /// <summary>
        /// Current Line Number
        /// </summary>
        protected abstract int CurrentLineNumber { get; }

        /// <inheritdoc />
        protected ImportTapiBase(ILog log)
        {
            _log = log;
        }

        //TODO finish
        //public event EventHandler IoWarningEvent;
    }
}
