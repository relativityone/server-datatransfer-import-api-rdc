using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ComboBoxUIElement : UIElement<ComboBoxUIElement>
	{
		private readonly ButtonUIElement button;
		private readonly TextUIElement text;

		public ComboBoxUIElement(Func<AppiumWebElement> create) : base(create)
		{
			text = FindText();
			button = FindButton();
		}

		public void SelectComboBoxItem(string name)
		{
			if (text.Text == name) return;

			button.Click();

			string previousText = null;
			while (text.Text != name && text.Text != previousText)
			{
				previousText = text.Text;
				SendKeys(Keys.Down);
			}

			if (text.Text != name) throw new Exception($"ComboBoxItem with name: {name} doesn't exists");

			SendKeys(Keys.Enter);
		}
	}
}