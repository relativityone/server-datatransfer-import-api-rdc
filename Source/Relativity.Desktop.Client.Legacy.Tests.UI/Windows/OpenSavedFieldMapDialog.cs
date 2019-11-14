using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class OpenSavedFieldMapDialog : UIElement<OpenSavedFieldMapDialog>
	{
		private readonly ChooseLoadFileDialog chooseLoadFileDialog;
		private readonly RelativityConfirmationDialog confirmationDialog;
		private readonly EditUIElement fileNameEdit;
		private readonly ButtonUIElement openButton;

		public OpenSavedFieldMapDialog(Func<AppiumWebElement> create) : base(create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open");
			chooseLoadFileDialog = new ChooseLoadFileDialog(FindWindow("Choose Load File")).WaitFor();
			confirmationDialog = new RelativityConfirmationDialog(FindWindow("Relativity.Desktop.Client")).WaitFor();
		}

		public void LoadKweFile(string kweFile, string datFile)
		{
			fileNameEdit.SendKeys(kweFile);
			openButton.Click();
			confirmationDialog.ClickOkButton();
			chooseLoadFileDialog.LoadDatFile(datFile);
		}
	}
}