// ----------------------------------------------------------------------------
// <copyright file="RdcFileSerializationBinder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Binder to find types in specified assembly.
	/// </summary>
	internal class RdcFileSerializationBinder : System.Runtime.Serialization.SerializationBinder
	{
		private readonly Assembly assembly;

		/// <summary>
		/// Initializes a new instance of the <see cref="RdcFileSerializationBinder"/> class.
		/// </summary>
		/// <param name="assembly">Assembly to find types in.</param>
		public RdcFileSerializationBinder(Assembly assembly)
		{
			this.assembly = assembly;
		}

		/// <summary>
		/// Used by the serializer to find types.
		/// </summary>
		/// <param name="assemblyName">Original assembly.</param>
		/// <param name="typeName">Type to find.</param>
		/// <returns>Matched type.</returns>
		public override Type BindToType(string assemblyName, string typeName)
		{
			return this.assembly.GetType(typeName);
		}
	}
}