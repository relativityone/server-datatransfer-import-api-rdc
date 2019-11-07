using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class CheckBoxUIElement : UIElement
	{
		public CheckBoxUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public void UnCheck()
		{
			SetValue(false);
		}

		public void Check()
		{
			SetValue(true);
		}

		public void SetValue(bool check)
		{
			if (check && !Element.Selected || !check && Element.Selected)
			{
				Toggle();
			}
		}

		public void Toggle()
		{
			Click();
		}
	}
}