using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class SpinnerComboBoxUIElement : UIElement
	{
		private readonly EditUIElement edit;

		public SpinnerComboBoxUIElement(Func<AppiumWebElement> create) : base(create)
		{
			edit = new EditUIElement(FindEdit());
		}

		public void SetValue<T>(T value)
		{
			edit.SetText(value.ToString());
		}
	}
}