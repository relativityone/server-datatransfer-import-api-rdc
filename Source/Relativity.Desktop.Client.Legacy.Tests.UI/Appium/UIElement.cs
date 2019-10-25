using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class UIElement
	{
		private static readonly TimeSpan WaitForPropertyDefaultTimeout = TimeSpan.FromSeconds(1);
		private static readonly TimeSpan WaitForChildTimeout = TimeSpan.FromSeconds(1);
		private readonly Func<AppiumWebElement> create;
		private AppiumWebElement appiumWebElement;

		protected UIElement(Func<AppiumWebElement> create)
		{
			this.create = create;
		}

		protected AppiumWebElement Element => appiumWebElement ?? (appiumWebElement = create());

		public string Text => Element.Text;

		public UIElement Refresh()
		{
			appiumWebElement = null;
			return this;
		}

		public void Click()
		{
			Element.Click();
		}

		public void SendKeys(string text)
		{
			Element.SendKeys(text);
		}

		public UIElement WaitToBeVisible(TimeSpan timeout)
		{
			return WaitFor(x => x.Displayed, timeout);
		}

		public UIElement WaitToBeVisible()
		{
			return WaitToBeVisible(WaitForPropertyDefaultTimeout);
		}

		public UIElement WaitToBeEnabled(TimeSpan timeout)
		{
			return WaitFor(x => x.Enabled, timeout);
		}

		public UIElement WaitToBeEnabled()
		{
			return WaitToBeEnabled(WaitForPropertyDefaultTimeout);
		}

		public UIElement WaitToBeSelected(TimeSpan timeout)
		{
			return WaitFor(x => x.Selected, timeout);
		}

		public UIElement WaitToBeSelected()
		{
			return WaitToBeSelected(WaitForPropertyDefaultTimeout);
		}

		private UIElement WaitFor(Func<AppiumWebElement, bool> condition, TimeSpan timeout)
		{
			Wait.For(() => condition(Element), timeout);
			return this;
		}

		protected UIElement FindEdit(string name)
		{
			return Create(() => Element.FindEdit(name));
		}

		public UIElement FindEditWithAutomationId(string automationId)
		{
			return Create(() => Element.FindEditWithAutomationId(automationId));
		}

		protected UIElement FindButton(string name)
		{
			return Create(() => Element.FindButton(name));
		}

		public UIElement FindButtonWithAutomationId(string automationId)
		{
			return Create(() => Element.FindButtonWithAutomationId(automationId));
		}

		protected UIElement FindButtonWithClass(string name, string className)
		{
			return Create(() => Element.FindButtonWithClass(name, className));
		}

		protected UIElement FindTextWithAutomationId(string automationId)
		{
			return Create(() => Element.FindChildWithAutomationId(ElementType.Text, automationId));
		}

		protected Func<AppiumWebElement> FindTabWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.Tab, automationId);
		}

		protected Func<AppiumWebElement> FindMenuBar(string name)
		{
			return () => Element.FindChild(ElementType.MenuBar, name);
		}

		public UIElement FindTree()
		{
			return Create(() => Element.FindTree());
		}

		protected UIElement FindTreeWithAutomationId(string automationId)
		{
			return Create(() => Element.FindTreeWithAutomationId(automationId));
		}

		protected Func<AppiumWebElement> FindListWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.List, automationId);
		}

		protected Func<AppiumWebElement> WaitForWindow(string name)
		{
			return () => WaitForChild(x => x.FindChildren(ElementType.Window, name));
		}

		protected Func<AppiumWebElement> WaitForChild(string elementType, string name, TimeSpan timeout)
		{
			return () => { return WaitForChild(x => x.FindChildren(elementType, name), timeout); };
		}

		protected static UIElement Create(Func<AppiumWebElement> create)
		{
			return new UIElement(create);
		}

		private AppiumWebElement WaitForChild(Func<AppiumWebElement, IReadOnlyList<AppiumWebElement>> getChildren)
		{
			return WaitForChild(getChildren, WaitForChildTimeout);
		}

		private AppiumWebElement WaitForChild(Func<AppiumWebElement, IReadOnlyList<AppiumWebElement>> getChildren,
			TimeSpan timeout)
		{
			var children = getChildren(Element);

			if (children.Any()) return children.First();

			Wait.For(() =>
			{
				children = getChildren(Element);
				return children.Any();
			}, timeout);

			return children.FirstOrDefault();
		}
	}
}