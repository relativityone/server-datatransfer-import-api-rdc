// ----------------------------------------------------------------------------
// <copyright file="ProfilingReport.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------;

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Text;
	using System.Xml.Linq;

	public class ProfilingReport
	{
		private readonly StringBuilder sql;

		private bool printSql;
		private ulong cpuTime;
		private ulong duration;
		private ulong physicalReads;
		private ulong logicalReads;
		private ulong writes;
		private ulong rowCount;

		public ProfilingReport(bool printSql)
		{
			this.printSql = printSql;
			this.sql = new StringBuilder();
		}

		public void Add(XElement element)
		{
			element.ThrowIfNull(nameof(element));

			switch (element.Name.LocalName)
			{
				case "action":
				case "data":
					switch (element.Attribute("name").Value)
					{
						case "cpu_time":
							this.cpuTime += ulong.Parse(element.Element("value").Value);
							break;
						case "duration":
							this.duration += ulong.Parse(element.Element("value").Value);
							break;
						case "physical_reads":
							this.physicalReads += ulong.Parse(element.Element("value").Value);
							break;
						case "logical_reads":
							this.logicalReads += ulong.Parse(element.Element("value").Value);
							break;
						case "writes":
							this.writes += ulong.Parse(element.Element("value").Value);
							break;
						case "row_count":
							this.rowCount += ulong.Parse(element.Element("value").Value);
							break;
						case "batch_text":
						case "statement":
							if (this.printSql)
							{
								this.sql.AppendLine(element.Element("value").Value);
								this.sql.AppendLine();
								this.sql.AppendLine("----------------------------------------------------------");
								this.sql.AppendLine();
							}

							break;
					}

					break;
				default:
					throw new ArgumentException($"Unsupported element type: {element.Name.LocalName}", nameof(element));
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			if (this.printSql)
			{
				builder.Append(this.sql);
			}

			builder.AppendLine($"{nameof(this.cpuTime)}: {this.cpuTime}");
			builder.AppendLine($"{nameof(this.duration)}: {this.duration}");
			builder.AppendLine($"{nameof(this.physicalReads)}: {this.physicalReads}");
			builder.AppendLine($"{nameof(this.logicalReads)}: {this.logicalReads}");
			builder.AppendLine($"{nameof(this.writes)}: {this.writes}");
			builder.AppendLine($"{nameof(this.rowCount)}: {this.rowCount}");

			return builder.ToString();
		}
	}
}
