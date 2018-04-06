' ----------------------------------------------------------------------------
' <copyright file="RelativityLogFactory.vb" company="kCura Corp">
'   kCura Corp (C) 2017 All Rights Reserved.
' </copyright>
' ----------------------------------------------------------------------------

''' <summary>
''' Represents a factory to create <see cref="Relativity.Logging.ILog"/> instances.
''' </summary>
Public Class RelativityLogFactory

	''' <summary>
	''' Creates a new Relativity log instance.
	''' </summary>
	''' <param name="subSystem">
	''' The sub-system.
	''' </param>
	''' <returns>
	''' The <see cref="Relativity.Logging.ILog"/> instance.
	''' </returns>
	Public Shared Function CreateLog(ByVal subSystem As String) As Relativity.Logging.ILog

		Try
			Dim log As Relativity.Logging.ILog = Relativity.Logging.Log.Logger
			If log Is Nothing OrElse log.GetType() = GetType(Relativity.Logging.NullLogger)
				Dim options As Relativity.Logging.LoggerOptions = Relativity.Logging.Factory.LogFactory.GetOptionsFromAppDomain()
				If options Is Nothing
					options = new Relativity.Logging.LoggerOptions				
				End If

				If String.IsNullOrEmpty(options.ConfigurationFileLocation) Then
					options.ConfigurationFileLocation = GetLogConfigFilePath(kCura.WinEDDS.Config.LogConfigFile)
				End If

				If String.IsNullOrEmpty(options.System) Then
					options.System = "WinEDDS"
				End If

				If String.IsNullOrEmpty(options.SubSystem) Then
					options.SubSystem = subSystem
				End If

				log = Relativity.Logging.Factory.LogFactory.GetLogger(options)
				Relativity.Logging.Log.Logger = log
			End If

			Return log
		Catch e As Exception
			Try
				Relativity.Logging.Tools.InternalLogger.WriteFromExternal(
					$"Failed to setup WinEDDS logging. Exception: {e.ToString()}",
					New Relativity.Logging.LoggerOptions With {.System = "WinEDDS"})
			Catch
				' Being overly cautious to ensure no fatal errors occur due to logging.
			End Try

			Return Relativity.Logging.Factory.LogFactory.GetNullLogger()
		End Try		
	End Function

	''' <summary>
	''' Gets the logger configuration file path.
	''' </summary>
	''' <param name="logConfigFile">
	''' The source file to identify.
	''' </param>
	''' <returns>
	''' The full path.
	''' </returns>
	Private Shared Function GetLogConfigFilePath(ByVal logConfigFile As String) As String
		Try
			Dim path As String = logConfigFile
			If String.IsNullOrEmpty(path) OrElse (System.IO.Path.IsPathRooted(path) AndAlso Not System.IO.File.Exists(path)) Then
				Return String.Empty
			End If

			If System.IO.Path.IsPathRooted(path) AndAlso System.IO.File.Exists(path) Then
				Return logConfigFile
			End If

			' Be careful with GetEntryAssembly. This is null when executed via NUnit.
			Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
			If assembly Is Nothing
				assembly = System.Reflection.Assembly.GetExecutingAssembly()
			End If

			Dim directory As String = System.IO.Directory.GetParent(assembly.Location).FullName
			Dim file As String = System.IO.Path.Combine(directory, "LogConfig.xml")
			Return file
		Catch
			Return String.Empty
		End Try
	End Function
End Class