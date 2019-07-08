// ----------------------------------------------------------------------------
// <copyright file="AppSettingsDictionary.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Defines a custom dictionary that applies all changes to the injected <see cref="IAppSettings"/> instance.
	/// </summary>
	/// <remarks>
	/// This class is provided for backwards compatibility for those API users that apply application settings
	/// directly to the dictionary instead of using an app.config file.
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1035:ICollectionImplementationsHaveStronglyTypedMembers",
		Justification = "This isn't required.")]
	internal class AppSettingsDictionary : IDictionary
	{
		private readonly Dictionary<object, object> dictionary = new Dictionary<object, object>();
		private readonly IAppSettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppSettingsDictionary"/> class.
		/// </summary>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		public AppSettingsDictionary(IAppSettings settings)
		{
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
			AppSettingsManager.Copy(settings, this);
		}

		/// <inheritdoc />
		public int Count => this.dictionary.Count;

		/// <inheritdoc />
		public bool IsFixedSize => ((IDictionary)this.dictionary).IsFixedSize;

		/// <inheritdoc />
		public bool IsReadOnly => ((IDictionary)this.dictionary).IsReadOnly;

		/// <inheritdoc />
		public bool IsSynchronized => ((IDictionary)this.dictionary).IsSynchronized;

		/// <inheritdoc />
		public ICollection Keys => this.dictionary.Keys;

		/// <inheritdoc />
		public object SyncRoot => ((IDictionary)this.dictionary).SyncRoot;

		/// <inheritdoc />
		public ICollection Values => this.dictionary.Values;

		/// <inheritdoc />
		public object this[object key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return this.dictionary[key];
			}

			set
			{
				if (key == null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				this.dictionary[key] = value;
				AppSettingsManager.SetDynamicValue(
					this.settings,
					key.ToString(),
					value != null ? value.ToString() : string.Empty);
			}
		}

		/// <inheritdoc />
		public void Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			this.dictionary.Add(key, value);
			AppSettingsManager.SetDynamicValue(
				this.settings,
				key.ToString(),
				value != null ? value.ToString() : string.Empty);
		}

		/// <inheritdoc />
		public void Clear()
		{
			this.dictionary.Clear();
		}

		/// <inheritdoc />
		public bool Contains(object key)
		{
			return this.dictionary.ContainsKey(key);
		}

		/// <inheritdoc />
		public void CopyTo(Array array, int index)
		{
			((ICollection)this.dictionary).CopyTo(array, index);
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		/// <inheritdoc />
		public IDictionaryEnumerator GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		/// <inheritdoc />
		public void Remove(object key)
		{
			this.dictionary.Remove(key);
		}
	}
}