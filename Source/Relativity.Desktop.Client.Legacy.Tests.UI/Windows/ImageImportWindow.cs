using OpenQA.Selenium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class ImageImportWindow : RdcWindowBase<ImageImportWindow>
	{
		private readonly EditUIElement filePathEdit;
		private readonly MenuItemUIElement menuBar;
		private readonly OpenSettingsDialog openSettingsDialog;
		private readonly ComboBoxUIElement overwriteComboBox;
		private readonly ComboBoxUIElement productionComboBox;
		private readonly ImageImportProfile profile;

		public ImageImportWindow(ILog logger, RdcWindowsManager windowsManager,
			WindowDetails window,
			ImageImportProfile profile) :
			base(logger, windowsManager, window)
		{
			this.profile = profile;

			menuBar = FindMenuBar("Application");
			filePathEdit = FindEditWithAutomationId("_filePath");
			overwriteComboBox = FindComboBoxWithAutomationId("_overwriteDropdown");
			productionComboBox = FindComboBoxWithAutomationId("_productionDropdown");
			openSettingsDialog = new OpenSettingsDialog(logger, FindWindow("Open"));
		}

		public void SetupImport(ImageImportWindowSetupParameters parameters)
		{
			LoadSettings(parameters);
			SetImportFilePath(parameters.ImportFilePath);
			SetOverwriteMode(parameters);
			SetProduction(parameters);
		}

		public void LoadSettings(string settingsFilePath)
		{
			// Import|Load Settings
			menuBar.SendKeys(Keys.Alt + "I");
			menuBar.SendKeys("L");
			openSettingsDialog.OpenSettingsFile(settingsFilePath);
		}

		public ProgressWindow RunImport()
		{
			// Import|Import File...
			menuBar.SendKeys(Keys.Alt + "I");
			menuBar.SendKeys("I");
			return WindowsManager.SwitchToProgressWindow(profile.ProgressWindow);
		}

		private void LoadSettings(ImageImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.SettingsFilePath))
			{
				LoadSettings(parameters.SettingsFilePath);
			}
		}

		private void SetImportFilePath(string path)
		{
			filePathEdit.Click();
			filePathEdit.SetText(path);
		}

		private void SetOverwriteMode(ImageImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.OverwriteMode))
			{
				overwriteComboBox.SelectComboBoxItem(parameters.OverwriteMode);
			}
		}

		private void SetProduction(ImageImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.ProductionName))
			{
				productionComboBox.SelectComboBoxItem(parameters.ProductionName);
			}
		}
	}
}