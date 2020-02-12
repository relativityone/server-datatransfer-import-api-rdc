// ----------------------------------------------------------------------------
// <copyright file="DataReaderBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Data;
	using System.Globalization;

	/// <summary>
	/// The IDataReader is very redundant. This class implements all methods
	/// that can be implemented in terms of other methods in the IDataReader interface.
	/// </summary>
	public abstract class DataReaderBase : IDataReader
	{
		public abstract int Depth { get; }

		public abstract int FieldCount { get; }

		public abstract bool IsClosed { get; }

		public abstract int RecordsAffected { get; }

		public abstract object this[int i] { get; }

		public object this[string name]
		{
			get
			{
				int ordinal = this.GetOrdinal(name);
				return this[ordinal];
			}
		}

		public abstract string GetName(int i);

		public abstract int GetOrdinal(string name);

		public abstract Type GetFieldType(int i);

		public abstract void Close();

		public abstract bool NextResult();

		public abstract bool Read();

		public string GetDataTypeName(int i)
		{
			return this.GetFieldType(i).Name;
		}

		public object GetValue(int i)
		{
			return this[i];
		}

		public int GetValues(object[] values)
		{
			int length = Math.Min(this.FieldCount, values.Length);
			for (int i = 0; i < length; ++i)
			{
				values[i] = this[i];
			}

			return length;
		}

		public bool GetBoolean(int i)
		{
			return (bool)this[i];
		}

		public byte GetByte(int i)
		{
			return (byte)this[i];
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			byte[] array = (byte[])this[i];
			Array.Copy(array, fieldOffset, buffer, bufferoffset, length);
			return Math.Min(length, array.LongLength - fieldOffset);
		}

		public char GetChar(int i)
		{
			return (char)this[i];
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			char[] array = (char[])this[i];
			Array.Copy(array, fieldoffset, buffer, bufferoffset, length);
			return Math.Min(length, array.LongLength - fieldoffset);
		}

		public Guid GetGuid(int i)
		{
			return (Guid)this[i];
		}

		public short GetInt16(int i)
		{
			return (short)this[i];
		}

		public int GetInt32(int i)
		{
			return (int)this[i];
		}

		public long GetInt64(int i)
		{
			return (long)this[i];
		}

		public float GetFloat(int i)
		{
			return (float)this[i];
		}

		public double GetDouble(int i)
		{
			return (double)this[i];
		}

		public string GetString(int i)
		{
			return (string)this[i];
		}

		public decimal GetDecimal(int i)
		{
			return (decimal)this[i];
		}

		public DateTime GetDateTime(int i)
		{
			return (DateTime)this[i];
		}

		public IDataReader GetData(int i)
		{
			return (IDataReader)this[i];
		}

		public bool IsDBNull(int i)
		{
			return this[i] is DBNull;
		}

		public DataTable GetSchemaTable()
		{
			using (var table = new DataTable())
			{
				table.Locale = CultureInfo.InvariantCulture;

				for (int i = 0; i < this.FieldCount; ++i)
				{
					table.Columns.Add(new DataColumn(this.GetName(i), this.GetFieldType(i)));
				}

				using (IDataReader reader = table.CreateDataReader())
				{
					return reader.GetSchemaTable();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}
	}
}
