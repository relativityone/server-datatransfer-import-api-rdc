// ----------------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System.IO;
	using System.Runtime.Serialization;

	/// <summary>
	/// Defines static methods to serialize and deserialize objects.
	/// </summary>
	internal static class SerializationHelper
	{
		/// <summary>
		/// The SOAP formatter instance.
		/// </summary>
		private static readonly System.Runtime.Serialization.Formatters.Soap.SoapFormatter SoapFormatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();

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
			using (System.IO.StreamWriter writer = new System.IO.StreamWriter(file))
			{
				SoapFormatter.Serialize(writer.BaseStream, value);
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

			try
			{
				return SoapFormatterDeserialize<T>(soap);
			}
			catch (SerializationException)
			{
				System.Type t = typeof(T);
				string assemblyName = t.Assembly.GetName().Name;

				// HACK! This is a temporary workaround to address serialization compatibility until a proper solution is in place.
				soap = soap.Replace("/kCura.WinEDDS%2C%20Version%3D", $"/{assemblyName}%2C%20Version%3D");
				return SoapFormatterDeserialize<T>(soap);
			}
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
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(soap)))
			{
				T deserialized = (T)SoapFormatter.Deserialize(ms);
				return deserialized;
			}
		}
	}
}