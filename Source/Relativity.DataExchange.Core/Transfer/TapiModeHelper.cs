// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiModeHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines transfer mode related static helper methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Defines transfer mode related static helper methods.
	/// </summary>
	internal static class TapiModeHelper
	{
		/// <summary>
		/// Gets the dictionary that orders the Transfer API clients.
		/// </summary>
		private static IDictionary<TapiClient, int> TapiClientOrderMap =>
			new Dictionary<TapiClient, int>
				{
					{ TapiClient.Direct, 0 }, { TapiClient.Aspera, 1 }, { TapiClient.Web, 2 },
				};

		/// <summary>
		/// Dynamically builds the file transfer mode documentation text.
		/// </summary>
		/// <returns>
		/// The help text.
		/// </returns>
		public static string BuildDocText()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("FILE TRANSFER MODES:");
			using (var transferLog = new RelativityTransferLog())
			{
				foreach (var clientMetadata in Relativity.Transfer.TransferClientHelper.SearchAvailableClients(transferLog)
					.OrderBy(x => x.DisplayName))
				{
					if (sb.Length > 0)
					{
						sb.AppendLine();
						sb.AppendLine();
					}

					sb.AppendFormat(" • {0} • ", clientMetadata.DisplayName);
					sb.AppendLine();
					sb.Append(clientMetadata.Description);
				}
			}

			sb.AppendLine();
			sb.AppendLine();
			return sb.ToString();
		}

		/// <summary>
		/// Builds the import file transfer mode status text from the native and metadata transfer clients.
		/// </summary>
		/// <param name="nativeFilesCopied">
		/// <see langword="true" /> to copy all natives; otherwise, <see langword="false" /> skips copying natives.
		/// </param>
		/// <param name="native">
		/// Specify the client used to transfer native files.
		/// </param>
		/// <param name="metadata">
		/// Specify the client used to transfer metadata files.
		/// </param>
		/// <returns>
		/// The status text.
		/// </returns>
		public static string BuildImportStatusText(
			bool nativeFilesCopied,
			TapiClient? native,
			TapiClient? metadata)
		{
			ITapiObjectService tapiObjectService = new TapiObjectService(new NullAuthTokenProvider());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string nativeFilesMode = GetFileTransferModeText(
				tapiObjectService,
				native != null ? new[] { native.Value } : new TapiClient[] { });
			sb.AppendFormat(
				Strings.FileTransferStatusTextModePrefix,
				nativeFilesCopied ? nativeFilesMode : Strings.FileTransferModeDisabled);
			sb.Append(", ");
			string metadataFilesMode = GetFileTransferModeText(
				tapiObjectService,
				metadata != null ? new[] { metadata.Value } : new TapiClient[] { });
			sb.AppendFormat(Strings.FileTransferStatusTextMetadataPrefix, metadataFilesMode);
			return sb.ToString();
		}

		/// <summary>
		/// Builds the import file transfer mode status text from th list of native transfer clients.
		/// </summary>
		/// <param name="natives">
		/// The list of native transfer clients.
		/// </param>
		/// <returns>
		/// The status text.
		/// </returns>
		public static string BuildExportStatusText(IEnumerable<TapiClient> natives)
		{
			ITapiObjectService tapiObjectService = new TapiObjectService(new NullAuthTokenProvider());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat(
				Strings.FileTransferStatusTextModePrefix,
				GetFileTransferModeText(tapiObjectService, natives));
			return sb.ToString();
		}

		/// <summary>
		/// Gets the client identifier for the specified transfer client.
		/// </summary>
		/// <param name="client">
		/// The client.
		/// </param>
		/// <returns>
		/// The unique identifier.
		/// </returns>
		private static Guid GetClientId(TapiClient client)
		{
			switch (client)
			{
				case TapiClient.Aspera:
					return new Guid(Relativity.Transfer.TransferClientConstants.AsperaClientId);

				case TapiClient.Direct:
					return new Guid(Relativity.Transfer.TransferClientConstants.FileShareClientId);

				case TapiClient.Web:
					return new Guid(Relativity.Transfer.TransferClientConstants.HttpClientId);

				default:
					return Guid.Empty;
			}
		}

		/// <summary>
		/// Gets the file transfer mode text for the specified client.
		/// </summary>
		/// <param name="service">
		/// The Transfer API object service.
		/// </param>
		/// <param name="clients">
		/// The list of clients.
		/// </param>
		/// <returns>
		/// The text.
		/// </returns>
		private static string GetFileTransferModeText(ITapiObjectService service, IEnumerable<TapiClient> clients)
		{
			StringBuilder sb = new StringBuilder();
			foreach (TapiClient flaggedClient in clients.Distinct().Except(new[] { TapiClient.None })
				.OrderBy(x => TapiClientOrderMap[x]))
			{
				if (sb.Length > 0)
				{
					sb.Append("/");
				}

				sb.Append(service.GetClientDisplayName(GetClientId(flaggedClient)));
			}

			if (sb.Length == 0)
			{
				sb.Append(Strings.FileTransferModePending);
			}

			return sb.ToString();
		}
	}
}