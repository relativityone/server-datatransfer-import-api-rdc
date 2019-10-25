using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public class ChooseLoadFileDialog : UIElement
	{
		private readonly UIElement fileNameEdit;
		private readonly UIElement openButton;

		public ChooseLoadFileDialog(Func<AppiumWebElement> create) : base(create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open", ElementClass.Button);
		}

		public void LoadDatFile(string datFile)
		{
			fileNameEdit.SendKeys(datFile);
			openButton.Click();
		}
	}
}