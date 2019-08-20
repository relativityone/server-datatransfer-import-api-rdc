// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiModeService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create Transfer API objects.
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
	/// Represents a class object to expose Transfer API transfer mode operations.
	/// </summary>
	internal class TapiModeService : ITapiModeService
	{
		/// <summary>
		/// The transfer object service.
		/// </summary>
		private readonly ITapiObjectService tapiObjectService;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiModeService"/> class.
		/// </summary>
		public TapiModeService()
			: this(new TapiObjectService())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiModeService"/> class.
		/// </summary>
		/// <param name="service">
		/// The Transfer API object service.
		/// </param>
		public TapiModeService(ITapiObjectService service)
		{
			this.tapiObjectService = service.ThrowIfNull(nameof(service));
		}

		/// <summary>
		/// Gets the dictionary that orders the Transfer API clients.
		/// </summary>
		private static IDictionary<TapiClient, int> TapiClientOrderMap =>
			new Dictionary<TapiClient, int>
				{
					{ TapiClient.Direct, 0 }, { TapiClient.Aspera, 1 }, { TapiClient.Web, 2 },
				};

		/// <inheritdoc />
		public string BuildDocText(bool includeBulk)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("FILE TRANSFER MODES:");
			sb.Append(BuildDocText());
			sb.AppendLine();
			sb.AppendLine();
			if (includeBulk)
			{
				sb.AppendLine("SQL INSERT MODES:");
				sb.AppendLine(" • Bulk • ");
				sb.Append("The upload process has access to the SQL share on the appropriate case database.  This ensures the fastest transfer of information between the desktop client and the relativity servers.");
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(" • Single •");
				sb.Append("The upload process has NO access to the SQL share on the appropriate case database.  This is a slower method of import. If the process is using single mode, contact your Relativity Database Administrator to see if a SQL share can be opened for the desired case.");
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public string BuildImportStatusText(
			bool nativeFilesCopied,
			TapiClient? native,
			TapiClient? metadata)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat(
				Strings.FileTransferModeTextNative,
				nativeFilesCopied ? this.GetFileTransferModeText(native != null ? new[] { native.Value } : new TapiClient[] { }) : Strings.FileTransferModeDisabled);
			sb.Append(", ");
			sb.AppendFormat(Strings.FileTransferModeTextMetadata, this.GetFileTransferModeText(metadata != null ? new[] { metadata.Value } : new TapiClient[] { }));
			return sb.ToString();
		}

		/// <inheritdoc />
		public string BuildExportStatusText(IEnumerable<TapiClient> natives)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat(Strings.FileTransferModeTextNative, this.GetFileTransferModeText(natives));
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
		/// Searches for all available clients and builds the documentation text from the discovered metadata.
		/// </summary>
		/// <returns>
		/// The documentation text.
		/// </returns>
		private static string BuildDocText()
		{
			using (var transferLog = new RelativityTransferLog())
			{
				var sb = new StringBuilder();
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

				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets the file transfer mode text for the specified client.
		/// </summary>
		/// <param name="clients">
		/// The list of clients.
		/// </param>
		/// <returns>
		/// The text.
		/// </returns>
		private string GetFileTransferModeText(IEnumerable<TapiClient> clients)
		{
			StringBuilder sb = new StringBuilder();
			foreach (TapiClient flaggedClient in clients.Distinct().Except(new[] { TapiClient.None })
				.OrderBy(x => TapiClientOrderMap[x]))
			{
				if (sb.Length > 0)
				{
					sb.Append("/");
				}

				sb.Append(this.tapiObjectService.GetClientDisplayName(GetClientId(flaggedClient)));
			}

			if (sb.Length == 0)
			{
				sb.Append(Strings.FileTransferModePending);
			}

			return sb.ToString();
		}
	}
}