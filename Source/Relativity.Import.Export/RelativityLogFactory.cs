// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityLogFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents static methods to create Relativity log instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;

	/// <summary>
    /// Represents a wait and retry policy class objects with a default back-off time strategy.
    /// </summary>
    public static class RelativityLogFactory
    {
		/// <summary>
		/// The default sub-system.
		/// </summary>
		public const string WinEDDSSubSystem = "WinEDDS";

		/// <summary>
		/// Creates a Relativity logging instance.
		/// </summary>
		/// <returns>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </returns>
		public static Relativity.Logging.ILog CreateLog()
		{
			return CreateLog(WinEDDSSubSystem);
		}

		/// <summary>
		/// Creates a Relativity logging instance.
		/// </summary>
		/// <param name="subSystem">
		/// The sub system.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </returns>
		public static Relativity.Logging.ILog CreateLog(string subSystem)
	    {
		    try
		    {
			    Relativity.Logging.ILog log = Relativity.Logging.Log.Logger;
			    if (log == null || log is Relativity.Logging.NullLogger)
			    {
				    Relativity.Logging.LoggerOptions options =
					    Relativity.Logging.Factory.LogFactory.GetOptionsFromAppDomain();
				    if (options == null)
				    {
					    options = new Relativity.Logging.LoggerOptions();
					}

				    if (string.IsNullOrEmpty(options.ConfigurationFileLocation))
				    {
					    options.ConfigurationFileLocation = GetLogConfigFilePath(AppSettings.Instance.LogConfigXmlFileName);
				    }

				    if (string.IsNullOrEmpty(options.System))
				    {
					    options.System = "WinEDDS";
				    }

				    if (string.IsNullOrEmpty(options.SubSystem))
				    {
					    options.SubSystem = subSystem;
				    }

				    log = Relativity.Logging.Factory.LogFactory.GetLogger(options);
				    Relativity.Logging.Log.Logger = log;
			    }

			    return log;
		    }
		    catch (Exception e)
		    {
			    try
			    {
				    Relativity.Logging.Tools.InternalLogger.WriteFromExternal(
					    $"Failed to setup WinEDDS logging. Exception: {e.ToString()}",
					    new Relativity.Logging.LoggerOptions { System = "WinEDDS" });
			    }
			    catch
			    {
					// Being overly cautious to ensure no fatal errors occur due to logging.
				}

			    return Relativity.Logging.Factory.LogFactory.GetNullLogger();
		    }
	    }

		/// <summary>
		/// Gets the logger configuration file path.
		/// </summary>
		/// <param name="logConfigFile">
		/// The source file to identify.
		/// </param>
		/// <returns>
		/// The full path.
		/// </returns>
		private static string GetLogConfigFilePath(string logConfigFile)
		{
			try
			{
				string path = logConfigFile;
				if (string.IsNullOrEmpty(path) || (System.IO.Path.IsPathRooted(path) && !System.IO.File.Exists(path)))
				{
					return string.Empty;
				}

				if (System.IO.Path.IsPathRooted(path) && System.IO.File.Exists(path))
				{
					return logConfigFile;
				}

				// Be careful with GetEntryAssembly. This is null when executed via NUnit.
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
				if (assembly == null)
				{
					assembly = System.Reflection.Assembly.GetExecutingAssembly();
				}

				string directory = System.IO.Directory.GetParent(assembly.Location).FullName;
				string file = System.IO.Path.Combine(directory, "LogConfig.xml");
				return file;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}