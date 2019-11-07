using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class ChooseLoadFileDialog : UIElement<ChooseLoadFileDialog>
	{
		private readonly EditUIElement fileNameEdit;
		private readonly ButtonUIElement openButton;

		public ChooseLoadFileDialog(Func<AppiumWebElement> create) : base(create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open");
		}

		public void LoadDatFile(string datFile)
		{
			fileNameEdit.SendKeys(datFile);
			openButton.Click();
		}
	}
}