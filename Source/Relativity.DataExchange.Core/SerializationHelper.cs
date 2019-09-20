// ----------------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System.IO;
	using System.Runtime.Serialization.Formatters.Soap;

	/// <summary>
	/// Defines static methods to serialize and deserialize objects.
	/// </summary>
	internal static class SerializationHelper
	{
		/// <summary>
		/// Serializes the specified object to a file.
		/// </summary>
		/// <param name="value">
		/// The object to serialize.
		/// </param>
		/// <param name="file">
		/// The file to persist the serialized object graph.
		/// </param>
		public static void SerializeToSoapFile(object value, string file)
		{
			using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var soapFormatter = new SoapFormatter();
				soapFormatter.Serialize(stream, value);
			}
		}

		/// <summary>
		/// Deserializes the SOAP file content to an object of the specified type.
		/// </summary>
		/// <typeparam name="T">
		/// The object type to deserialize to.
		/// </typeparam>
		/// <param name="file">
		/// The full path to the file.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		public static T DeserializeFromSoapFile<T>(string file)
		{
			string soap = File.ReadAllText(file);
			return DeserializeFromSoap<T>(soap);
		}

		/// <summary>
		/// Deserializes the SOAP content to an object of the specified type.
		/// </summary>
		/// <typeparam name="T">
		/// The object type to deserialize to.
		/// </typeparam>
		/// <param name="soap">
		/// The SOAP content to deserialize.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		public static T DeserializeFromSoap<T>(string soap)
		{
			if (string.IsNullOrEmpty(soap))
			{
				return default(T);
			}

			return SoapFormatterDeserialize<T>(soap);
		}

		/// <summary>
		/// Deserializes the SOAP content to an object of the specified type.
		/// </summary>
		/// <typeparam name="T">
		/// The object type to deserialize to.
		/// </typeparam>
		/// <param name="soap">
		/// The SOAP content to deserialize.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		private static T SoapFormatterDeserialize<T>(string soap)
		{
			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(soap)))
			{
				var soapFormatter = new SoapFormatter
				{
					Binder = new RdcFileSerializationBinder(typeof(T).Assembly),
				};

				T deserialized = (T)soapFormatter.Deserialize(ms);
				return deserialized;
			}
		}
	}
}