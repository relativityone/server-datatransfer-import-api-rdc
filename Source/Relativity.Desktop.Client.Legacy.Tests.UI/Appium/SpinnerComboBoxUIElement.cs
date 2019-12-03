using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class SpinnerComboBoxUIElement : UIElement<SpinnerComboBoxUIElement>
	{
		private readonly EditUIElement edit;

		public SpinnerComboBoxUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			edit = FindEdit();
		}

		public void SetValue<T>(T value)
		{
			edit.SetText(value.ToString());
		}
	}
}