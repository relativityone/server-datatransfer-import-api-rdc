// <copyright file="ProfilerReportBuilderBase.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Linq;

	public abstract class ProfilerReportBuilderBase<TRow> : IProfilerReportBuilder
	where TRow : class, new()
	{
		private TRow currentRow;

		protected ProfilerReportBuilderBase()
		{
			this.Rows = new List<TRow>();
		}

		protected List<TRow> Rows { get; private set; }

		public void CreateNewRow()
		{
			this.currentRow = new TRow();
		}

		public void CommitRow()
		{
			this.Rows.Add(this.currentRow);
			this.currentRow = null;
		}

		public abstract ProfilerReport Build();

		public void AddDetailsToCurrentRow(XElement element)
		{
			element = element ?? throw new ArgumentNullException(nameof(element));
			if (this.currentRow == null)
			{
				throw new InvalidOperationException("Cannot add details to current row, because it does not exist.");
			}

			switch (element.Name.LocalName)
			{
				case "action":
				case "data":
					this.AddDetailsToRow(element, this.currentRow);

					break;
				default:
					throw new NotSupportedException($"Unsupported element type: {element.Name.LocalName}");
			}
		}

		protected abstract void AddDetailsToRow(XElement element, TRow row);

		protected ulong GetNumericValue(XContainer element)
		{
			element = element ?? throw new ArgumentNullException(nameof(element));
			return ulong.Parse(element.Element("value").Value);
		}
	}
}
