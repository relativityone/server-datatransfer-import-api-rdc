using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class ImportLoadFileWindow : RdcWindowBase<ImportLoadFileWindow>
	{
		private readonly EditUIElement filePathEdit;
		private readonly MenuItemUIElement menuBar;
		private readonly OpenSettingsDialog openSettingsDialog;
		private readonly ComboBoxUIElement overwriteComboBox;
		private readonly ComboBoxUIElement productionComboBox;
		private readonly ImportProfile profile;

		public ImportLoadFileWindow(RdcWindowsManager windowsManager,
			WindowDetails window,
			ImportProfile profile) :
			base(windowsManager, window)
		{
			this.profile = profile;

			menuBar = FindMenuBar("Application");
			filePathEdit = FindEditWithAutomationId("_filePath");
			overwriteComboBox = FindComboBoxWithAutomationId("_overwriteDropdown");
			productionComboBox = FindComboBoxWithAutomationId("_productionDropdown");
			openSettingsDialog = new OpenSettingsDialog(FindWindow("Open"));
		}

		public void SetupImport(ImportWindowSetupParameters parameters)
		{
			LoadSettings(parameters);
			SetImportFilePath(parameters.ImportFilePath);
			SetOverwriteMode(parameters);
			SetProduction(parameters);
		}

		public void LoadSettings(string settingsFilePath)
		{
			menuBar.ClickMenuItem("Import").ClickMenuItem("Load Settings\tCtrl+O");
			openSettingsDialog.OpenSettingsFile(settingsFilePath);
		}

		public ProgressWindow RunImport()
		{
			menuBar.ClickMenuItem("Import").ClickMenuItem("Import File...\tF5");
			return WindowsManager.SwitchToProgressWindow(profile.ProgressWindow);
		}

		private void LoadSettings(ImportWindowSetupParameters parameters)
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

		private void SetOverwriteMode(ImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.OverwriteMode))
			{
				overwriteComboBox.SelectComboBoxItem(parameters.OverwriteMode);
			}
		}

		private void SetProduction(ImportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.ProductionName))
			{
				productionComboBox.SelectComboBoxItem(parameters.ProductionName);
			}
		}
	}
}