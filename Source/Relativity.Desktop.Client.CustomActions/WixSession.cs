// -----------------------------------------------------------------------------------------------------
// <copyright file="ISession.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Deployment.WindowsInstaller;

namespace Relativity.Desktop.Client.CustomActions
{
	/// <summary>
	/// Represents a WIX session class object.
	/// </summary>
	public class WixSession : IWixSession
	{
		/// <summary>
		/// The session backing.
		/// </summary>
		private readonly Session session;

		/// <summary>
		/// Initializes a new instance of the <see cref="WixSession"/> class.
		/// </summary>
		/// <param name="session">
		/// The session.
		/// </param>
		public WixSession(Session session)
		{
			this.session = session;
		}

		public void Log(string msg)
		{
			this.session.Log(msg);
		}

		/// <inheritdoc />
		public string GetStringPropertyValue(string propertyName)
		{
			string stringValue = string.Empty;
			var isPropertyFound = false;
			if (session.CustomActionData != null)
			{
				isPropertyFound = session.CustomActionData.TryGetValue(propertyName, out stringValue);
			}

			if (!isPropertyFound)
			{
				try
				{
					// The Try pattern is expected.
					stringValue = session[propertyName];
				}
				catch
				{
					stringValue = string.Empty;
				}
			}

			return stringValue;
		}

		/// <inheritdoc />
		public T GetPropertyValue<T>(string propertyName)
		{
			string value = GetStringPropertyValue(propertyName);
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

			try
			{
				return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		/// <inheritdoc />
		public void SetPropertyValue(string propertyName, string value)
		{
			session[propertyName] = value;
		}
	}
}