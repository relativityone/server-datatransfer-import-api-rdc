using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class CheckBoxUIElement : UIElement<CheckBoxUIElement>
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
			if (check && !Selected || !check && Selected)
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