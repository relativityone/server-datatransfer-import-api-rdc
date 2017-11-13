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

        public ImportTapiBase()
        {
            
        }

        /// <summary>
        /// Current Line Number
        /// </summary>
        protected int CurrentLineNumber { get; }

        /// <inheritdoc />
        protected ImportTapiBase(ILog log)
        {
            _log = log;
        }
    }
}
