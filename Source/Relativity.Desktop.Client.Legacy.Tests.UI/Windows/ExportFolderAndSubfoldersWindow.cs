using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.SetupParameters;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ExportFolderAndSubfoldersWindow : RdcWindowBase
	{
		private readonly BrowseForFolderDialog browseForFolderDialog;
		private readonly ComboBoxUIElement dataFileEncodingComboBox;
		private readonly UIElement dataSourceTab;
		private readonly UIElement destinationFilesTab;
		private readonly CheckBoxUIElement exportFullTextAsFileCheckBox;
		private readonly CheckBoxUIElement exportImagesCheckBox;
		private readonly CheckBoxUIElement exportNativeFilesCheckBox;
		private readonly EditUIElement folderPathTextBox;
		private readonly MenuItemUIElement menuBar;
		private readonly ComboBoxUIElement nativeFileFormatComboBox;
		private readonly UIElement pickTextFieldPrecedenceButton;
		private readonly PickTextPrecedenceDialog pickTextPrecedenceDialog;
		private readonly UIElement selectViewButton;
		private readonly SelectViewDialog selectViewDialog;
		private readonly ComboBoxUIElement textAndNativeComboBox;
		private readonly ComboBoxUIElement textFileEncodingComboBox;
		private readonly SpinnerComboBoxUIElement volumeDigitPaddingComboBox;

		public ExportFolderAndSubfoldersWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = new MenuItemUIElement(FindMenuBar("Application"));
			var tabs = new TabsUIElement(FindTabWithAutomationId("TabControl1"));
			dataSourceTab = tabs.FindTabItem("Data Source");
			destinationFilesTab = tabs.FindTabItem("Destination Files");
			selectViewButton = FindButtonWithAutomationId("_selectFromListButton");
			folderPathTextBox = FindEditWithAutomationId("_folderPath");
			browseForFolderDialog = new BrowseForFolderDialog(WaitForWindow("Browse For Folder"));
			selectViewDialog = new SelectViewDialog(WaitForWindow("Select View"));
			pickTextPrecedenceDialog = new PickTextPrecedenceDialog(WaitForWindow("Pick Text Precedence"));
			var textAndNativeFileNamesGroup = FindGroupWithAutomationId("GroupBoxTextAndNativeFileNames");
			textAndNativeComboBox =
				new ComboBoxUIElement(textAndNativeFileNamesGroup.FindComboBoxWithAutomationId("_comboBox"));
			exportImagesCheckBox = new CheckBoxUIElement(FindCheckBoxWithAutomationId("_exportImages"));
			exportNativeFilesCheckBox = new CheckBoxUIElement(FindCheckBoxWithAutomationId("_exportNativeFiles"));
			nativeFileFormatComboBox = new ComboBoxUIElement(FindComboBoxWithAutomationId("_nativeFileFormat"));
			dataFileEncodingComboBox =
				new ComboBoxUIElement(FindPaneWithAutomationId("_dataFileEncoding").FindComboBox());
			exportFullTextAsFileCheckBox = new CheckBoxUIElement(FindCheckBoxWithAutomationId("_exportFullTextAsFile"));
			textFileEncodingComboBox =
				new ComboBoxUIElement(FindPaneWithAutomationId("_textFileEncoding").FindComboBox());
			volumeDigitPaddingComboBox =
				new SpinnerComboBoxUIElement(FindComboBoxWithAutomationId("_volumeDigitPadding"));
			pickTextFieldPrecedenceButton = FindButtonWithAutomationId("_pickTextFieldPrecedenceButton");
		}

		public void SetupExport(ExportWindowSetupParameters parameters)
		{
			dataSourceTab.Click();
			SelectView(parameters);

			destinationFilesTab.Click();
			SetExportPath(parameters.ExportPath);
			SetVolumeInformationDigitPadding(parameters);
			SetFilesNamedAfter(parameters);
			SetExportImages(parameters);
			SetExportNativeFiles(parameters);
			SetNativeFileFormat(parameters);
			SetDataFileEncoding(parameters);
			SetExportFullTextAsFile(parameters);
			SetTextFileEncoding(parameters);
			SetTextFieldPrecedence(parameters);
		}

		public ProgressWindow RunExport()
		{
			menuBar.ClickMenuItem("File").ClickMenuItem("Run");
			return WindowsManager.SwitchToExportFoldersAndSubfoldersProgress();
		}

		private void SetExportPath(string path)
		{
			folderPathTextBox.Click();
			browseForFolderDialog.ClickCancelButton();
			folderPathTextBox.SetText(path);
		}

		private void SetExportImages(ExportWindowSetupParameters parameters)
		{
			exportImagesCheckBox.SetValue(parameters.ExportImages);
		}

		private void SetExportNativeFiles(ExportWindowSetupParameters parameters)
		{
			exportNativeFilesCheckBox.SetValue(parameters.ExportNativeFiles);
		}

		private void SetExportFullTextAsFile(ExportWindowSetupParameters parameters)
		{
			exportFullTextAsFileCheckBox.SetValue(parameters.ExportFullTextAsFile);
		}

		private void SetNativeFileFormat(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.NativeFileFormat))
				nativeFileFormatComboBox.SelectComboBoxItem(parameters.NativeFileFormat);
		}

		private void SetDataFileEncoding(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.DataFileEncoding))
				dataFileEncodingComboBox.SelectComboBoxItem(parameters.DataFileEncoding);
		}

		private void SetFilesNamedAfter(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.FilesNamedAfter))
				textAndNativeComboBox.SelectComboBoxItem(parameters.FilesNamedAfter);
		}

		private void SetTextFileEncoding(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.TextFileEncoding))
				textFileEncodingComboBox.SelectComboBoxItem(parameters.TextFileEncoding);
		}

		private void SetTextFieldPrecedence(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.TextFieldPrecedence))
			{
				pickTextFieldPrecedenceButton.Click();
				pickTextPrecedenceDialog.SelectFieldsAndClose(parameters.TextFieldPrecedence);
			}
		}

		private void SetVolumeInformationDigitPadding(ExportWindowSetupParameters parameters)
		{
			if (parameters.VolumeInformationDigitPadding > 0)
				volumeDigitPaddingComboBox.SetValue(parameters.VolumeInformationDigitPadding);
		}

		private void SelectView(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.View))
			{
				selectViewButton.Click();
				selectViewDialog.SelectView(parameters.View);
			}
		}
	}
}