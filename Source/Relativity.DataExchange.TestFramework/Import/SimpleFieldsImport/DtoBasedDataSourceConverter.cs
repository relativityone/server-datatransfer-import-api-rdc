// <copyright file="DtoBasedDataSourceConverter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;

	public static class DtoBasedDataSourceConverter
	{
		/// <summary>
		/// That method converts <see cref="IEnumerable{T}"/> to a <see cref="ImportDataSource{T}"/>.
		/// It reads all public properties of an dto object and use them as a fields in a <see cref="ImportDataSource{T}"/>.
		/// When property is decorated with <see cref="DisplayNameAttribute"/> and <see cref="DisplayNameAttribute.DisplayName"/>
		/// is non empty, than its value is used as a field name, otherwise property name is used.
		/// </summary>
		/// <typeparam name="T">Type of dto object.</typeparam>
		/// <param name="enumerable">Collection of dto object, which is used to build <see cref="ImportDataSource{T}"/>.</param>
		/// <returns><see cref="ImportDataSource{T}"/>.</returns>
		public static ImportDataSource<T> ConvertDtoCollectionToImportDataSource<T>(IEnumerable<T> enumerable)
		{
			enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));

			PropertyInfo[] properties = typeof(T).GetProperties();
			Func<T, object>[] getters = properties
				.Select(p => Delegate.CreateDelegate(typeof(Func<T, object>), p.GetMethod))
				.Cast<Func<T, object>>()
				.ToArray();
			string[] fieldNames = properties
				.Select(p => (p.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute)?.DisplayName ?? p.Name)
				.ToArray();

			return new ImportDataSource<T>(enumerable, fieldNames, getters);
		}
	}
}
