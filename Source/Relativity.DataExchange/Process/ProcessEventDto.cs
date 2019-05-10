// ----------------------------------------------------------------------------
// <copyright file="ProcessEventDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Xml.Serialization;

	/// <summary>
	/// Represents the process event data transfer object. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// The Xml serialization attributes are used strictly for backwards compatibility.
	/// </remarks>
	[Serializable]
	[XmlRoot(ElementName = "ProcessEvent")]
	public sealed class ProcessEventDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessEventDto"/> class.
		/// </summary>
		/// <remarks>
		/// This is only used for serialization purposes.
		/// </remarks>
		public ProcessEventDto()
		{
			this.EventType = ProcessEventType.Status;
			this.RecordInfo = string.Empty;
			this.Message = string.Empty;
			this.Timestamp = DateTime.Now;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessEventDto"/> class.
		/// </summary>
		/// <param name="eventType">
		/// The process event type.
		/// </param>
		/// <param name="recordInfo">
		/// The current record information.
		/// </param>
		/// <param name="message">
		/// The event message.
		/// </param>
		/// <param name="timestamp">
		/// The event timestamp.
		/// </param>
		public ProcessEventDto(ProcessEventType eventType, string recordInfo, string message, DateTime timestamp)
		{
			this.EventType = eventType;
			this.RecordInfo = recordInfo;
			this.Message = message;
			this.Timestamp = timestamp;
		}

		/// <summary>
		/// Gets or sets the process event type.
		/// </summary>
		/// <value>
		/// The <see cref="ProcessEventType"/> value.
		/// </value>
		[XmlElement(ElementName = "Type")]
		public ProcessEventType EventType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the event message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[XmlElement(ElementName = "Message")]
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the record information associated with this event.
		/// </summary>
		/// <value>
		/// The record information.
		/// </value>
		[XmlElement(ElementName = "RecordInfo")]
		public string RecordInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the event timestamp.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		[XmlElement(ElementName = "DateTime")]
		public DateTime Timestamp
		{
			get;
			set;
		}
	}
}