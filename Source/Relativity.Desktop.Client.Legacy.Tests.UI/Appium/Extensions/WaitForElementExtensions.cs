using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class WaitForElementExtensions
	{
		private static readonly TimeSpan WaitForChildTimeout = TimeSpan.FromSeconds(1);
		private static readonly TimeSpan WaitForProperty = TimeSpan.FromSeconds(1);

		public static AppiumWebElement WaitForChild(this AppiumWebElement element,
			Func<AppiumWebElement, IReadOnlyList<AppiumWebElement>> getChildren)
		{
			var children = getChildren(element);

			if (children.Any()) return children.First();

			Wait.For(() =>
			{
				children = getChildren(element);
				return children.Any();
			}, WaitForChildTimeout);

			return children.FirstOrDefault();
		}

		public static AppiumWebElement WaitToBeVisible(this AppiumWebElement element)
		{
			return element.WaitFor(x => x.Displayed);
		}

		public static AppiumWebElement WaitToBeEnabled(this AppiumWebElement element)
		{
			return element.WaitFor(x => x.Enabled);
		}

		public static AppiumWebElement WaitToBeSelected(this AppiumWebElement element)
		{
			return element.WaitFor(x => x.Selected);
		}

		public static AppiumWebElement WaitFor(this AppiumWebElement element, Func<AppiumWebElement, bool> condition)
		{
			Wait.For(() => condition(element), WaitForProperty);
			return element;
		}
	}
}