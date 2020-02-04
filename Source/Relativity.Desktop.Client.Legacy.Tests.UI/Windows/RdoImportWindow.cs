using OpenQA.Selenium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class RdoImportWindow : RdcWindowBase<RdoImportWindow>
	{
		private readonly ButtonUIElement autoMapFieldsButton;
		private readonly ButtonUIElement browseButton;
		private readonly TabItemUIElement fieldMapTab;
		private readonly TabItemUIElement loadFileTab;
		private readonly MenuItemUIElement menuBar;
		private readonly OpenSavedFieldMapDialog openSavedFieldMapDialog;
		private readonly ChooseLoadFileDialog chooseLoadFileDialog;
		private readonly RdoImportProfile profile;

		public RdoImportWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window, RdoImportProfile profile)
			: base(logger, windowsManager, window)
		{
			this.profile = profile;
			menuBar = FindMenuBar("Application");
			openSavedFieldMapDialog = new OpenSavedFieldMapDialog(logger, FindWindow("Open Saved Field Map"));
			chooseLoadFileDialog = new ChooseLoadFileDialog(logger, FindWindow("Open")).WaitFor();
			var tabs = FindTabsWithAutomationId("TabControl1");
			loadFileTab = tabs.FindTabItem("Load File");
			fieldMapTab = tabs.FindTabItem("Field Map");
			autoMapFieldsButton = FindButton("Auto Map Fields");
			browseButton = FindButtonWithAutomationId("_browseButton");
		}

		public void SetupImport(RdoImportWindowSetupParameters parameters)
		{
			menuBar.SendKeys(Keys.Escape); // In some cases after open dialog with import settings automatically menu bar option is selected

			loadFileTab.Click();
			
			LoadSettings(parameters);

			fieldMapTab.Click();

			AutoMapFields(parameters);
		}

		public void ClickImportFileMenuItem()
		{
			// Import|Import File...
			menuBar.SendKeys(Keys.Alt + "I");
			menuBar.SendKeys("I"); // enter
		}

		public ProgressWindow RunImport()
		{
			ClickImportFileMenuItem();
			return WindowsManager.SwitchToProgressWindow(profile.ProgressWindow);
		}

		private void LoadSettings(string settingsFile, string importFile)
		{
			// File|Load Import Settings (kwe)
			menuBar.SendKeys(Keys.LeftAlt + "F");
			menuBar.SendKeys(Keys.Enter);
			openSavedFieldMapDialog.LoadKweFile(settingsFile, importFile);
		}

		private void LoadSettings(RdoImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.SettingsFilePath))
			{
				LoadSettings(parameters.SettingsFilePath, parameters.ImportFilePath);
			}
			else
			{
				SetImportFilePath(parameters);
			}
		}

		private void SetImportFilePath(RdoImportWindowSetupParameters parameters)
		{
			browseButton.Click();
			chooseLoadFileDialog.LoadDatFile(parameters.ImportFilePath);
		}

		private void AutoMapFields(RdoImportWindowSetupParameters parameters)
		{
			if (parameters.AutoMapFields)
			{
				autoMapFieldsButton.Click();
			}
		}
	}
}