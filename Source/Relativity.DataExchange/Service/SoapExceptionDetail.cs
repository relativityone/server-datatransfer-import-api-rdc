﻿//------------------------------------------------------------------------------
// <auto-generated>
// </auto-generated>
//------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;

	/// <summary>
	/// Represents a Relativity SOAP-based exception detail data object. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	/// <remarks>
	/// There's too much risk and too many expectation to make this type internal.
	/// </remarks>
	[System.Xml.Serialization.XmlType("SoapExceptionDetail")]
	[System.Xml.Serialization.XmlRoot(ElementName ="detail")]
	[Serializable]
	public sealed class SoapExceptionDetail
	{
		[System.Xml.Serialization.XmlElement("Details")]
		public string[] Details { get; set; }

		[System.Xml.Serialization.XmlElement("ExceptionFullText")]
		public string ExceptionFullText { get; set; }

		[System.Xml.Serialization.XmlElement("ExceptionMessage")]
		public string ExceptionMessage { get; set; }

		[System.Xml.Serialization.XmlElement("ExceptionTrace")]
		public string ExceptionTrace { get; set; }

		[System.Xml.Serialization.XmlElement("ExceptionType")]
		public string ExceptionType { get; set; }

		public SoapExceptionDetail()
		{
		}

		public SoapExceptionDetail(Exception ex)
		{
			if (ex == null)
			{
				throw new ArgumentNullException(nameof(ex));
			}

			this.ExceptionType = ex.GetType().ToString();
			this.SetMessageText(ex);
			this.ExceptionTrace = ex.StackTrace;
			this.ExceptionFullText = ex.ToString();
		}

		private void SetMessageText(Exception ex)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			this.GetBaseMessageAndAllInnerMessages(ex, stringBuilder);
			this.ExceptionMessage = stringBuilder.ToString();
		}

		private System.Text.StringBuilder GetBaseMessageAndAllInnerMessages(Exception ex, System.Text.StringBuilder sb)
		{
			sb.AppendLine("Error: " + ex.Message);
			if ((ex.InnerException != null))
			{
				sb.AppendLine("---Additional Errors---");
				this.GetBaseMessageAndAllInnerMessages(ex.InnerException, sb);
			}
			return sb;
		}
	}
}