

using System;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Class for IO warning event arguments
    /// </summary>
    public class IoWarningEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TypeEnum
        {
            /// <summary>
            /// Message
            /// </summary>
            Message,
            /// <summary>
            /// Instant Retry Error
            /// </summary>
            InstantRetryError,
            /// <summary>
            /// Wait Retry Error
            /// </summary>
            WaitRetryError
        }

        private int _waitTime;
        private System.Exception _exception;
        private long _currentLineNumber;
        private string _message;

        private TypeEnum _type;

        /// <summary>
        /// 
        /// </summary>
        public IoWarningEventArgs(int waitTime, System.Exception ex, long currentLineNumber)
        {
            _waitTime = waitTime;
            _exception = ex;
            _currentLineNumber = currentLineNumber;
            if (waitTime > 0)
            {
                _type = TypeEnum.WaitRetryError;
            }
            else
            {
                _type = TypeEnum.InstantRetryError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IoWarningEventArgs(string message, long currentLineNumber)
        {
            _message = message;
            _currentLineNumber = currentLineNumber;
            _type = TypeEnum.Message;
        }

        /// <summary>
        /// Wait time
        /// </summary>
        public int WaitTime => _waitTime;

        /// <summary>
        /// Exception
        /// </summary>
        public System.Exception Exception => _exception;

        /// <summary>
        /// Current line number
        /// </summary>
        public long CurrentLineNumber => _currentLineNumber;

        /// <summary>
        /// Message
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Type
        /// </summary>
        public TypeEnum Type => _type;
    }
}
