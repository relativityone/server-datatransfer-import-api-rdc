// -----------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Runtime.Serialization.Formatters.Soap;

	public class SerializationTestsBase
	{
		protected static object BinarySerialize(object graph)
		{
			return Serialize(new BinaryFormatter(), graph);
		}

		protected static object SoapSerialize(object graph)
		{
			return Serialize(new SoapFormatter(), graph);
		}

		protected static object Serialize(IFormatter formatter, object graph)
		{
			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, graph);
				stream.Seek(0, SeekOrigin.Begin);
				object deserializedObject = formatter.Deserialize(stream);
				return deserializedObject;
			}
		}
	}
}