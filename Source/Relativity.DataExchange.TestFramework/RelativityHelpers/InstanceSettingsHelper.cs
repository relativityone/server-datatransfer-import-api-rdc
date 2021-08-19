// ----------------------------------------------------------------------------
// <copyright file="InstanceSettingsHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services;
	using Relativity.Services.InstanceSetting;

	/// <summary>
	/// Defines static helper methods to manage instance settings in tests.
	/// </summary>
	public static class InstanceSettingsHelper
	{
		private const int WaitTimeForInstanceSettingChangeInSeconds = 30;

		/// <summary>
		/// This method changes value of given instance setting.
		/// </summary>
		/// <param name="parameters">Test context parameters.</param>
		/// <param name="section">Instance setting section.</param>
		/// <param name="name">Instance setting name.</param>
		/// <param name="newValue">New value for instance setting.</param>
		/// <returns><see langword="true"/> when value was changed,
		/// <see langword="false"/> when instance setting was already set to desired value.</returns>
		public static async Task<bool> ChangeInstanceSetting(
			IntegrationTestParameters parameters,
			string section,
			string name,
			string newValue)
		{
			using (var instanceSettingManager = ServiceHelper.GetServiceProxy<IInstanceSettingManager>(parameters))
			{
				var query = new Query
				{
					Condition = $"'Section' == '{section}' AND 'Name' == '{name}'",
				};
				InstanceSettingQueryResultSet results = await instanceSettingManager.QueryAsync(query).ConfigureAwait(false);
				InstanceSetting setting = results.Results.Single().Artifact;

				bool valueShouldBeChanged = setting.Value != newValue;
				if (valueShouldBeChanged)
				{
					setting.Value = newValue;
					await instanceSettingManager.UpdateSingleAsync(setting).ConfigureAwait(false);
				}

				return valueShouldBeChanged;
			}
		}

		/// <summary>
		/// This method changes value of given instance setting. If value was changed method will wait for <see cref="WaitTimeForInstanceSettingChangeInSeconds"/> seconds.
		/// </summary>
		/// <param name="parameters">Test context parameters.</param>
		/// <param name="section">Instance setting section.</param>
		/// <param name="name">Instance setting name.</param>
		/// <param name="newValue">New value for instance setting.</param>
		/// <returns><see langword="true"/> when value was changed,
		/// <see langword="false"/> when instance setting was already set to desired value.</returns>
		public static async Task<bool> ChangeInstanceSettingAndWait(
			IntegrationTestParameters parameters,
			string section,
			string name,
			string newValue)
		{
			bool wasChanged = await ChangeInstanceSetting(parameters, section, name, newValue)
								  .ConfigureAwait(false);
			if (wasChanged)
			{
				await Task.Delay(TimeSpan.FromSeconds(WaitTimeForInstanceSettingChangeInSeconds))
					.ConfigureAwait(false);
			}

			return wasChanged;
		}

		/// <summary>
		/// This method gets value of given instance setting.
		/// </summary>
		/// <param name="parameters">Test context parameters.</param>
		/// <param name="section">Instance setting section.</param>
		/// <param name="name">Instance setting name.</param>
		/// <returns>Instance setting value.</returns>
		public static async Task<string> QueryInstanceSetting(
			IntegrationTestParameters parameters,
			string section,
			string name)
		{
			using (var instanceSettingManager = ServiceHelper.GetServiceProxy<IInstanceSettingManager>(parameters))
			{
				var query = new Query
				{
					Condition = $"'Section' == '{section}' AND 'Name' == '{name}'",
				};
				InstanceSettingQueryResultSet results = await instanceSettingManager.QueryAsync(query).ConfigureAwait(false);
				InstanceSetting setting = results.Results.Single().Artifact;

				return setting.Value;
			}
		}
	}
}