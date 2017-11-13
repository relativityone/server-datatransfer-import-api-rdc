using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Logging;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Base class for IO reporter
    /// </summary>
    public class IoReporter : IIoReporter
    {
        private IFileSystemService _fileSystemService;
        private IWaitAndRetryPolicy _waitAndRetryPolicy;
        private ILog _log;
        private bool _disableNativeLocationValidation;

        /// <summary>
        /// Constructor for IO reporter
        /// </summary>
        public IoReporter(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry, ILog log, bool disableNativeLocationValidation)
        {
            _fileSystemService = fileService;
            _waitAndRetryPolicy = waitAndRetry;
            _log = log;

            _disableNativeLocationValidation = disableNativeLocationValidation;

            IoWarningEvent += OnIoWarningEvent;
        }

        /// <inheritdoc />
        public long GetFileLength(string fileName)
        {
            long fileLength = 0;
            _waitAndRetryPolicy.WaitAndRetry<Exception>(retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : _waitAndRetryPolicy.WaitTimeBetweenRetryAttempts),
                (exception, timeSpan) =>
                {
                    getFileLengthRetryAction(exception, timeSpan, fileName);
                },
                () => { fileLength = _fileSystemService.GetFileLength(fileName); }
            );

            return fileLength;
        }

        /// <summary>
        /// Raise IO Warning
        /// </summary>
        public void RaiseIoWarning(IoWarningEventArgs e)
        {
            IoWarningEvent?.Invoke(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<IoWarningEventArgs> IoWarningEvent;

        private void getFileLengthRetryAction(Exception ex, TimeSpan timeSpan, string fileName)
        {
            if (_disableNativeLocationValidation && ex is ArgumentException &&
                ex.Message.Contains("Illegal characters in path."))
            {
                //TODO how to break WaitAndRetry in Polly?
                //throw new FileInfoFailedException(String.Format("File {0} not found: illegal characters in path.", filename))
            }

            RaiseIoWarning(new IoWarningEventArgs(timeSpan.Seconds, ex, ));
        }

        private void OnIoWarningEvent(object sender, IoWarningEventArgs ioWarningEventArgs)
        {
            System.Threading.Monitor.Enter(this.ProcessObserver);
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            switch (ioWarningEventArgs.Type)
            {
                case IoWarningEventArgs.TypeEnum.Message:
                    message.Append(ioWarningEventArgs.Message);
                    break;
                default:
                    message.Append("Error accessing opticon file - retrying");
                    if (ioWarningEventArgs.WaitTime > 0)
                        message.Append(" in " + ioWarningEventArgs.WaitTime + " seconds");
                    message.Append(Environment.NewLine);
                    message.Append("Actual error: " + ioWarningEventArgs.Exception);
                    break;
            }
            this.ProcessObserver.RaiseWarningEvent((ioWarningEventArgs.CurrentLineNumber + 1).ToString, message.ToString);
            System.Threading.Monitor.Exit(this.ProcessObserver);

            //TODO Log to Relativity
        }
    }
}
