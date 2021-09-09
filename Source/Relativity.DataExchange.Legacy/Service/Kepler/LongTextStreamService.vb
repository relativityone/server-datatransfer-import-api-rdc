' --------------------------------------------------------------------------------------------------------------------
' <copyright file="LongTextStreamService.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' <summary>
'   Represents a service class object to retrieve long text data through the Object Manager streaming API.
' </summary>
' --------------------------------------------------------------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Service.Replacement
Imports Polly
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer
Imports Relativity.Kepler.Transport
Imports Relativity.Logging
Imports Relativity.Services.Objects
Imports Relativity.Services.Objects.DataContracts
Imports IObjectManager = Relativity.Services.Objects.IObjectManager

Namespace kCura.WinEDDS.Service.Kepler

	''' <summary>
	''' Represents a service class object to retrieve long text data through the Object Manager streaming API.
	''' </summary>
	Friend Class LongTextStreamService
		Implements ILongTextStreamService

		Private Const LongTextRetryCountKey As String = "LongTextRetryCount"
		Private Const LongTextRequestKey As String = "LongTextRequest"
		Private ReadOnly serviceNotification As IServiceNotification
		Private ReadOnly settings As IAppSettings
		Private ReadOnly fileSystem As IFileSystem
		Private ReadOnly logger As ILog
		Private ReadOnly exportLongTextBufferSizeBytes As Integer
		Private ReadOnly exportLongTextLargeFileProgressRateSeconds As Integer
		Private ReadOnly keplerProxy As IKeplerProxy

		Public Sub New(ByVal serviceProxyFactory As IServiceProxyFactory, ByVal serviceNotification As IServiceNotification, ByVal settings As IAppSettings, ByVal fileSystem As IFileSystem, ByVal logger As ILog)
			Me.serviceNotification = serviceNotification.ThrowIfNull(NameOf(serviceNotification))
			Me.settings = settings.ThrowIfNull(NameOf(settings))
			Me.fileSystem = fileSystem.ThrowIfNull(NameOf(fileSystem))
			Me.logger = logger.ThrowIfNull(NameOf(logger))
			serviceProxyFactory.ThrowIfNull(NameOf(serviceProxyFactory))

			Me.keplerProxy = new KeplerProxy(serviceProxyFactory, logger, Sub(exception, duration, retryCount, context) OnRetrySaveLongTextStream(exception, duration, retryCount, context))
			exportLongTextBufferSizeBytes = settings.ExportLongTextBufferSizeBytes
			exportLongTextLargeFileProgressRateSeconds = settings.ExportLongTextLargeFileProgressRateSeconds
		End Sub

		''' <inheritdoc/>
		Public Sub Dispose() Implements IDisposable.Dispose
		End Sub

		''' <inheritdoc/>
		Public Function SaveLongTextStreamAsync(ByVal request As Transfer.LongTextStreamRequest, ByVal token As CancellationToken, ByVal progress As IProgress(Of Transfer.LongTextStreamProgressEventArgs)) As Task(Of Transfer.LongTextStreamResult) Implements Transfer.ILongTextStreamService.SaveLongTextStreamAsync
			request.ThrowIfNull(NameOf(request))
			LongTextStreamService.ValidateRequest(request)
			Dim context As Context = New Context("LongTextExecutionKey") From {
				{LongTextRequestKey, request},
				{LongTextRetryCountKey, 0}
			}
			Return keplerProxy.ExecuteAsync(context, token, Function(ctx, ct, serviceProxyFactory) Me.OnExecuteSaveLongTextStream(serviceProxyFactory, request, context, token, progress))
		End Function

		Private Shared Function GetRetryCount(ByVal context As Context) As Integer
			Dim value As Object = Nothing
			Return If(context.TryGetValue(LongTextRetryCountKey, value), Convert.ToInt32(value), 0)
		End Function

		Private Shared Function GetLongTextStreamRequest(ByVal context As Context) As Transfer.LongTextStreamRequest
			Dim value As Object = Nothing
			Return If(context.TryGetValue(LongTextRequestKey, value), TryCast(value, Transfer.LongTextStreamRequest), New Transfer.LongTextStreamRequest())
		End Function

		Private Shared Sub ValidateRequest(ByVal request As Transfer.LongTextStreamRequest)
			If request.SourceObjectArtifactId < 1 OrElse request.SourceFieldArtifactId < 1 Then
				Throw New ArgumentException($"The long text streaming service request requires a source {If(request.SourceFieldArtifactId < 1, "field", "object")} artifact identifier greater than zero.", NameOf(request))
			End If

			If String.IsNullOrWhiteSpace(request.TargetFile) Then
				Throw New ArgumentException("The long text streaming service request requires the target file to be non-empty.", NameOf(request))
			End If

			If request.TargetEncoding Is Nothing OrElse request.SourceEncoding Is Nothing Then
				Throw New ArgumentException($"The long text streaming service request requires the {If(request.SourceEncoding Is Nothing, "source", "target")} encoding to be defined.", NameOf(request))
			End If

			If request.WorkspaceId < 1 Then
				Throw New ArgumentException($"The long text streaming service request requires the workspace artifact identifier to be greater than zero.", NameOf(request))
			End If
		End Sub

		Private Sub LogNonFatalInvalidRequestWarning(ByVal request As Transfer.LongTextStreamRequest, ByVal exception As Exception)
			logger.LogWarning(exception, "The {ArtifactId} - {FieldArtifactId} long text request returned a non-fatal and non-retryable error because 1 or more parameters is invalid.", request.SourceObjectArtifactId, request.SourceFieldArtifactId)
		End Sub

		Private Sub LogNonFatalObjectManagerWarning(ByVal request As Transfer.LongTextStreamRequest, ByVal exception As Exception)
			logger.LogWarning(exception, "The {ArtifactId} - {FieldArtifactId} long text request returned a non-fatal and non-retryable object manager specific error.", request.SourceObjectArtifactId, request.SourceFieldArtifactId)
		End Sub

		Private Async Function OnExecuteSaveLongTextStream(ByVal serviceProxyFactory As IServiceProxyFactory, ByVal request As Transfer.LongTextStreamRequest, ByVal context As Context, ByVal token As CancellationToken, ByVal progress As IProgress(Of Transfer.LongTextStreamProgressEventArgs)) As Task(Of Transfer.LongTextStreamResult)
			Dim retryCount As Integer = GetRetryCount(context)

			Try
				Dim exportObject As RelativityObjectRef = New RelativityObjectRef With {
					.ArtifactID = request.SourceObjectArtifactId
				}
				Dim longTextField As FieldRef = New FieldRef With {
					.ArtifactID = request.SourceFieldArtifactId
				}

				Try

					Using objectManager As IObjectManager = serviceProxyFactory.CreateProxyInstance(Of IObjectManager)()

						Using keplerStream As IKeplerStream = Await objectManager.StreamLongTextAsync(CInt(request.WorkspaceId), CType(exportObject, RelativityObjectRef), CType(longTextField, FieldRef)).ConfigureAwait(False)

							Using sourceStream As Stream = Await keplerStream.GetStreamAsync().ConfigureAwait(False)

								Using targetStream As FileStream = fileSystem.File.Create(request.TargetFile)
									Return Me.CopySourceStreamData(request, context, sourceStream, targetStream, token, progress)
								End Using
							End Using
						End Using
					End Using

				Catch __unusedException1__ As Exception
					Me.TryDeleteTargetFile(request.TargetFile)
					Throw
				End Try

			Catch e As ArgumentException
				' Non-OM invalid request or path.
				Me.LogNonFatalInvalidRequestWarning(request, e)
				Return Transfer.LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e)
			Catch e As NotSupportedException
				' Non-OM invalid path.
				Me.LogNonFatalInvalidRequestWarning(request, e)
				Return Transfer.LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e)
			Catch e As Exception

				If ObjectManagerExceptionHelper.IsNonFatalError(e) Then
					Me.LogNonFatalObjectManagerWarning(request, e)
					Return Transfer.LongTextStreamResult.CreateNonFatalIssueResult(request, retryCount, e)
				End If


				' All other errors are logged and will go through the service retry policy.
				Throw
			End Try
		End Function

		Private Sub OnRetrySaveLongTextStream(ByVal exception As Exception, ByVal duration As TimeSpan, ByVal retryCount As Integer, ByVal context As Context)
			' Intentionally limit notifications to status messages to avoid a large numbers of warning messages and total warning counts.
			context(LongTextRetryCountKey) = retryCount
			Dim request As Transfer.LongTextStreamRequest = GetLongTextStreamRequest(context)
			logger.LogError(exception, "An error occurred retrieving the {ArtifactId} - {FieldArtifactId} long text data from Object Manager. Currently on attempt {RetryCount} out of {MaxRetries} and waiting {WaitSeconds} seconds before the next retry attempt.", request.SourceObjectArtifactId, request.SourceFieldArtifactId, retryCount, settings.HttpErrorNumberOfRetries, duration.TotalSeconds)
			serviceNotification.NotifyStatus(ErrorMessageFormatter.FormatWebServiceRetryMessage("Long Text Stream Request", exception.Message, duration, retryCount, settings.HttpErrorNumberOfRetries))
		End Sub

		Private Function CopySourceStreamData(ByVal request As Transfer.LongTextStreamRequest, ByVal context As Context, ByVal sourceStream As Stream, ByVal targetStream As FileStream, ByVal token As CancellationToken, ByVal progress As IProgress(Of Transfer.LongTextStreamProgressEventArgs)) As Transfer.LongTextStreamResult
			' Note: this method is critical - avoid all possible overhead (e.g. Stopwatch).
			' Note: synchronous read/write operations are significantly faster than their asynchronous counterparts.
			Dim totalBytesWritten As Long = 0
			Dim startTimestamp As Date = Date.Now
			Const DetectEncodingFromByteOrderMarks As Boolean = False

			Using reader As StreamReader = New StreamReader(sourceStream, request.SourceEncoding, DetectEncodingFromByteOrderMarks, exportLongTextBufferSizeBytes)

				Using writer As StreamWriter = New StreamWriter(targetStream, request.TargetEncoding, exportLongTextBufferSizeBytes)
					Dim buffer As Char() = New Char(CInt(Me.exportLongTextBufferSizeBytes / Len(New Char) - 1)) {}
					Dim progressTimestamp As Date = startTimestamp
					Dim continueReading As Boolean = True

					While continueReading
						token.ThrowIfCancellationRequested()

						Try
							Dim chunkCharacters As Integer = reader.Read(buffer, 0, buffer.Length)

							If chunkCharacters = 0 Then
								continueReading = False
								writer.Flush()
							Else
								writer.Write(buffer, 0, chunkCharacters)
							End If

						Finally
							totalBytesWritten = writer.BaseStream.Length

							If Not continueReading AndAlso startTimestamp <> progressTimestamp OrElse (Date.Now - progressTimestamp).TotalSeconds >= exportLongTextLargeFileProgressRateSeconds Then
								progress?.Report(New Transfer.LongTextStreamProgressEventArgs(request, totalBytesWritten, Not continueReading))
								progressTimestamp = Date.Now
							End If
						End Try
					End While

					Dim endTimestamp As Date = Date.Now
					Return Transfer.LongTextStreamResult.CreateSuccessfulResult(request, request.TargetFile, totalBytesWritten, GetRetryCount(context), endTimestamp - startTimestamp)
				End Using
			End Using
		End Function

		Private Sub TryDeleteTargetFile(ByVal file As String)
			Try
				' Don't allow expected errors to rethrow.
				fileSystem.File.Delete(file)
			Catch e As Exception
				logger.LogWarning(e, "Failed to delete the target file.")
			End Try
		End Sub
	End Class
End Namespace