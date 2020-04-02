using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class OpenSavedFieldMapDialog : UIElement<OpenSavedFieldMapDialog>
	{
		private readonly ChooseLoadFileDialog chooseLoadFileDialog;
		private readonly RelativityConfirmationDialog confirmationDialog;
		private readonly EditUIElement fileNameEdit;
		private readonly ButtonUIElement openButton;

		public OpenSavedFieldMapDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open");
			chooseLoadFileDialog = new ChooseLoadFileDialog(logger, FindWindow("Choose Load File")).WaitFor(TimeSpan.FromSeconds(10));
			confirmationDialog = new RelativityConfirmationDialog(logger, FindWindow("Relativity.Desktop.Client")).WaitFor(TimeSpan.FromSeconds(10));
		}

		public void LoadKweFile(string kweFile, string datFile)
		{
			fileNameEdit.SendKeys(kweFile + Keys.Enter);
			confirmationDialog.ClickOkButton();
			chooseLoadFileDialog.LoadDatFile(datFile);
			WaitToNotExist(TimeSpan.FromSeconds(10));
		}
	}
}