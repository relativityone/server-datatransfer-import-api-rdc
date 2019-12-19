using System.Threading;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class ExportWindow : RdcWindowBase<ExportWindow>
	{
		private readonly BrowseForFolderDialog browseForFolderDialog;
		private readonly TabItemUIElement dataSourceTab;
		private readonly TabItemUIElement destinationFilesTab;
		private readonly CheckBoxUIElement exportFullTextAsFileCheckBox;
		private readonly CheckBoxUIElement exportImagesCheckBox;
		private readonly CheckBoxUIElement exportNativeFilesCheckBox;
		private readonly EditUIElement folderPathTextBox;
		private readonly ComboBoxUIElement imageFileFormatComboBox;
		private readonly ComboBoxUIElement imageFileTypeComboBox;
		private readonly MenuItemUIElement menuBar;
		private readonly ComboBoxUIElement metadataFileEncodingComboBox;
		private readonly ComboBoxUIElement metadataFileFormatComboBox;
		private readonly ButtonUIElement pickTextFieldPrecedenceButton;
		private readonly PickTextPrecedenceDialog pickTextPrecedenceDialog;
		private readonly ButtonUIElement selectFieldsSourceButton;
		private readonly SelectViewDialog selectFieldsSourceDialog;
		private readonly ExportProfile profile;
		private readonly ComboBoxUIElement textAndNativeComboBox;
		private readonly ComboBoxUIElement textFileEncodingComboBox;
		private readonly SpinnerComboBoxUIElement volumeDigitPaddingComboBox;

		public ExportWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window,
			ExportProfile profile)
			: base(logger, windowsManager, window)
		{
			this.profile = profile;
			menuBar = FindMenuBar("Application");
			var tabs = FindTabsWithAutomationId("TabControl1");
			dataSourceTab = tabs.FindTabItem("Data Source");
			destinationFilesTab = tabs.FindTabItem("Destination Files");
			selectFieldsSourceButton = FindButtonWithAutomationId("_selectFromListButton");
			folderPathTextBox = FindEditWithAutomationId("_folderPath");
			browseForFolderDialog = new BrowseForFolderDialog(logger, FindWindow("Browse For Folder"));
			selectFieldsSourceDialog = new SelectViewDialog(logger, FindWindow(profile.FieldsSourceDialogName));
			pickTextPrecedenceDialog = new PickTextPrecedenceDialog(logger, FindWindow("Pick Text Precedence"));
			var textAndNativeFileNamesGroup = FindGroupWithAutomationId("GroupBoxTextAndNativeFileNames");
			textAndNativeComboBox = textAndNativeFileNamesGroup.FindComboBoxWithAutomationId("_comboBox");
			exportImagesCheckBox = FindCheckBoxWithAutomationId("_exportImages");
			exportNativeFilesCheckBox = FindCheckBoxWithAutomationId("_exportNativeFiles");
			imageFileFormatComboBox = FindComboBoxWithAutomationId("_imageFileFormat");
			imageFileTypeComboBox = FindComboBoxWithAutomationId("_imageTypeDropdown");
			metadataFileFormatComboBox = FindComboBoxWithAutomationId("_nativeFileFormat");
			metadataFileEncodingComboBox = FindPaneWithAutomationId("_dataFileEncoding").FindComboBox();
			exportFullTextAsFileCheckBox = FindCheckBoxWithAutomationId("_exportFullTextAsFile");
			textFileEncodingComboBox = FindPaneWithAutomationId("_textFileEncoding").FindComboBox();
			volumeDigitPaddingComboBox = FindSpinnerComboBoxWithAutomationId("_volumeDigitPadding");
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
			SetImageFileFormat(parameters);
			SetImageFileType(parameters);
			SetMetadataFileFormat(parameters);
			SetMetadataFileEncoding(parameters);
			SetExportFullTextAsFile(parameters);
			SetTextFileEncoding(parameters);
			SetTextFieldPrecedence(parameters);
		}

		public ProgressWindow RunExport()
		{
			menuBar.ClickMenuItem("File").ClickMenuItem("Run");
			return WindowsManager.SwitchToProgressWindow(profile.ProgressWindow);
		}

		private void SetExportPath(string path)
		{
			folderPathTextBox.Click();
			browseForFolderDialog.ClickCancelButton();
			folderPathTextBox.SetText(path);
		}

		private void SetExportImages(ExportWindowSetupParameters parameters)
		{
			if (exportImagesCheckBox.Exists)
			{
				exportImagesCheckBox.SetValue(parameters.ExportImages);
			}
		}

		private void SetExportNativeFiles(ExportWindowSetupParameters parameters)
		{
			if (exportNativeFilesCheckBox.Exists)
			{
				exportNativeFilesCheckBox.SetValue(parameters.ExportNativeFiles);
			}
		}

		private void SetExportFullTextAsFile(ExportWindowSetupParameters parameters)
		{
			exportFullTextAsFileCheckBox.SetValue(parameters.ExportFullTextAsFile);
		}

		private void SetMetadataFileFormat(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.MetadataFileFormat))
				metadataFileFormatComboBox.SelectComboBoxItem(parameters.MetadataFileFormat);
		}

		private void SetMetadataFileEncoding(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.MetadataFileEncoding))
				metadataFileEncodingComboBox.SelectComboBoxItem(parameters.MetadataFileEncoding);
		}

		private void SetImageFileFormat(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.ImageFileFormat))
				imageFileFormatComboBox.SelectComboBoxItem(parameters.ImageFileFormat);
		}

		private void SetImageFileType(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.ImageFileType))
				imageFileTypeComboBox.SelectComboBoxItem(parameters.ImageFileType);
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
				pickTextPrecedenceDialog.SelectFieldAndClose(parameters.TextFieldPrecedence);
			}
		}

		private void SetVolumeInformationDigitPadding(ExportWindowSetupParameters parameters)
		{
			if (parameters.VolumeInformationDigitPadding > 0)
				volumeDigitPaddingComboBox.SetValue(parameters.VolumeInformationDigitPadding);
		}

		private void SelectView(ExportWindowSetupParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.FieldSourceName))
			{
				selectFieldsSourceButton.Click();
				selectFieldsSourceDialog.SelectView(parameters.FieldSourceName);
			}
		}
	}
}