// ----------------------------------------------------------------------------
// <copyright file="TestRelativityLogFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    using global::Relativity.Logging;

    /// <summary>
    /// Represents a factory to create <see cref="TestRelativityLog"/> instances.
    /// </summary>
    public static class TestRelativityLogFactory
    {
        /// <summary>
        /// Creastes a new 
        /// </summary>
        /// <returns>
        /// The <see cref="TestRelativityLog"/> instance.
        /// </returns>
        public static TestRelativityLog Create()
        {
            // By setting the singleton, all logs are captured to the console.
            var consoleLog = new TestRelativityLog();
            consoleLog.Application = "Integration-Tests";
            consoleLog.System = "Data-Transfer";
            consoleLog.SubSystem = "IAPI";
            consoleLog.IsEnabled = true;
            Log.Logger = consoleLog;
            return consoleLog;
        }
    }
}