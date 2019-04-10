// ----------------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
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

			// HACK! This is a temporary workaround to address serialization compatibility until a proper solution is in place.
			soap = soap.Replace("/kCura.WinEDDS%2C%20Version%3D", "/Relativity.Import.Export.Legacy%2C%20Version%3D");
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(soap)))
			{
				T deserialized = (T)SoapFormatter.Deserialize(ms);
				return deserialized;
			}
		}
	}
}