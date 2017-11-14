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
        /// IO reporter object
        /// </summary>
        protected IIoReporter _ioReporter;

        //TODO Do we need constructor here?
        /// <summary>
        /// Constructor for ImportTapiBase
        /// </summary>
        /// <param name="ioReporter"></param>
        //protected ImportTapiBase(IIoReporter ioReporter)
        //{
        //    _ioReporter = ioReporter;
        //}

        /// <summary>
        /// Current Line Number
        /// </summary>
        protected abstract int CurrentLineNumber { get; }

        /// <inheritdoc />
        protected ImportTapiBase(ILog log)
        {
            //TODO Introduce argument check
            _log = log;
        }
    }
}
