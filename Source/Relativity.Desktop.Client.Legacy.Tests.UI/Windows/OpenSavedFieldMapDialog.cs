using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public class OpenSavedFieldMapDialog : UIElement
	{
		private readonly ChooseLoadFileDialog chooseLoadFileDialog;
		private readonly RelativityConfirmationDialog confirmationDialog;
		private readonly UIElement fileNameEdit;
		private readonly UIElement openButton;

		public OpenSavedFieldMapDialog(Func<AppiumWebElement> create) : base(create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open", ElementClass.Button);
			chooseLoadFileDialog = new ChooseLoadFileDialog(WaitForWindow("Choose Load File"));
			confirmationDialog = new RelativityConfirmationDialog(WaitForWindow("Relativity.Desktop.Client"));
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