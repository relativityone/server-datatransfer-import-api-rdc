// ----------------------------------------------------------------------------
// <copyright file="RdcFileSerializationSurrogate.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;

	using Microsoft.VisualBasic;

	class RdcFileSerializationSurrogate : ISerializationSurrogate
	{
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			Type type = obj.GetType();
			var regex = new Regex(@"(\w+\+)?_?(\w+)");
			foreach (SerializationEntry entry in info)
			{
				Match match = regex.Match(entry.Name);
				PropertyInfo propertyInfo = type.GetProperty(match.Groups[2].Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
				if (propertyInfo == null)
				{
					throw new Exception();
				}

				object value = propertyInfo.PropertyType.IsEnum ? Enum.Parse(propertyInfo.PropertyType, entry.Value.ToString()) : Conversion.CTypeDynamic(entry.Value, propertyInfo.PropertyType);
				propertyInfo.SetMethod.Invoke(obj, new object[] { value });
			}

			return obj;
		}

		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}
	}
}