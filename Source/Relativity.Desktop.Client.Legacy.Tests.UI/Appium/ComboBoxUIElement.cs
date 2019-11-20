using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ComboBoxUIElement : UIElement<ComboBoxUIElement>
	{
		private readonly ButtonUIElement button;
		private readonly TextUIElement textBox;

		public ComboBoxUIElement(Func<AppiumWebElement> create) : base(create)
		{
			textBox = FindText();
			button = FindButton();
		}

		public void SelectComboBoxItem(string value)
		{
			if (textBox.Text == value) return;

			button.Click();

			var selected = SelectItemByValueInDirection(value, Keys.Up);
			if (!selected)
			{
				selected = SelectItemByValueInDirection(value, Keys.Down);
			}

			if (!selected)
			{
				throw new Exception($"ComboBoxItem with value: {value} doesn't exists");
			}

			SendKeys(Keys.Enter);
		}

		private bool SelectItemByValueInDirection(string value, string directionKey)
		{
			string previousText = null;
			while (textBox.Text != value && textBox.Text != previousText)
			{
				previousText = textBox.Text;
				SendKeys(directionKey);
			}

			return textBox.Text == value;
		}
	}
}