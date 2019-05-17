// -----------------------------------------------------------------------------------------------------
// <copyright file="CustomActions.cs" company="Relativity ODA LLC">
//   � Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Desktop.Client.CustomActions
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Windows.Forms;

	using Microsoft.Deployment.WindowsInstaller;
	using Microsoft.Win32;

	using Relativity.Desktop.Client.CustomActions.Resources;

	/// <summary>
	/// Represents all custom actions used by the Remote Desktop Client installer.
	/// </summary>
	public class CustomActions
	{
		private const string PropertyNameProductName = "ProductName";
		private const string PropertyNameAppProcessName = "AppProcessName";
		private const string PropertyNameUiLevel = "UILevel";
		private const int UiLevelNone = 2;

		[CustomAction]
		public static ActionResult BackupConfiguration(Session session)
		{
			AppConfigService service = new AppConfigService(new WixSession(session));
			ActionResult result = service.Backup();
			return result;
		}

		[CustomAction]
		public static ActionResult Cleanup(Session session)
		{
			AppConfigService service = new AppConfigService(new WixSession(session));
			ActionResult result = service.Delete();
			return result;
		}

		[CustomAction]
		public static ActionResult CloseRunningApplications(Session session)
		{
			try
			{
				WixSession wixSession = new WixSession(session);
				string processInternalName = wixSession.GetStringPropertyValue(PropertyNameAppProcessName);
				string productName = GetProductName(session);
				Process[] processes = Process.GetProcessesByName(processInternalName);
				if (!processes.Any())
				{
					return ActionResult.Success;
				}

				if (IsSilentMode(session))
				{
					CloseAllInstances(processes);
				}
				else
				{
					DialogResult dialogResult = MessageBox.Show(new Form { TopMost = true },
						string.Format(Strings.FilesInUseError, productName), productName,
						MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
					if (dialogResult == DialogResult.OK)
					{
						CloseAllInstances(processes);
					}
					else
					{
						return ActionResult.UserExit;
					}
				}

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Unable to kill running processes due to exception {e}");
				return ActionResult.Failure;
			}
		}

		[CustomAction]
		public static ActionResult GetOldRdcVersion(Session session)
		{
			try
			{
				IWixSession wixSession = new WixSession(session);
				List<RegistryView> views = new List<RegistryView> { RegistryView.Registry64, RegistryView.Registry32 };
				foreach (RegistryView view in views)
				{
					using (RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, view))
					using (var key = localKey.OpenSubKey(@"SOFTWARE\kCura\RelativityDesktopClient"))
					{
						if (key != null)
						{
							session.Log($"The RDC {view} Registry key exists.");
							string path = key.GetValue("Path") as string;
							if (!string.IsNullOrEmpty(path))
							{
								session.Log($"The old RDC Registry path value DOES exist. Path={path}");
								wixSession.SetPropertyValue(AppConfigService.PropertyNameOldRdcPath, path);
							}
							else
							{
								session.Log($"The old RDC Registry path value does NOT exist. Path={path}");
							}

							break;
						}

						session.Log($"The RDC {view} Registry key does NOT exist.");
					}
				}

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Unable to get the OLD RDC version due to exception {e}");
				return ActionResult.Failure;
			}
		}

		[CustomAction]
		public static ActionResult RestoreConfiguration(Session session)
		{
			AppConfigService service = new AppConfigService(new WixSession(session));
			ActionResult result = service.Merge();
			return result == ActionResult.Success
				? result
				: HandleInstallationError(session, Strings.ConfigurationRestoreError);
		}

		private static void CloseAllInstances(IEnumerable<Process> processes)
		{
			foreach (Process process in processes)
			{
				process.Kill();
			}
		}

		private static bool IsSilentMode(Session session)
		{
			try
			{
				WixSession wixSession = new WixSession(session);
				int uiLevel = wixSession.GetPropertyValue<int>(PropertyNameUiLevel);
				return uiLevel == UiLevelNone;
			}
			catch (Exception)
			{
				// If we fail just to determine the mode, this is very likely a deferred CA trying to access
				return true;
			}
		}

		private static ActionResult HandleInstallationError(Session session, string message)
		{
			if (IsSilentMode(session))
			{
				return ActionResult.Failure;
			}

			string productName = GetProductName(session);
			DialogResult result = MessageBox.Show(new Form {TopMost = true}, message, productName,
				MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			return result == DialogResult.OK ? ActionResult.Success : ActionResult.Failure;
		}

		private static string GetProductName(Session session)
		{
			WixSession wixSession = new WixSession(session);
			string productName = wixSession.GetStringPropertyValue(PropertyNameProductName);
			return productName;
		}
	}
}