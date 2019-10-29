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
			return Create(() => Element.FindChild(ElementType.Edit, name));
		}

		protected Func<AppiumWebElement> FindEdit()
		{
			return () => Element.FindChild(ElementType.Edit);
		}

		public UIElement FindEditWithAutomationId(string automationId)
		{
			return Create(() => Element.FindEditWithAutomationId(automationId));
		}

		protected Func<AppiumWebElement> FindEditWithAutomationId2(string automationId)
		{
			return () => Element.FindEditWithAutomationId(automationId);
		}

		protected UIElement FindButton(string name)
		{
			return Create(() => Element.FindButton(name));
		}

		protected UIElement FindButton()
		{
			return Create(() => Element.FindChild(ElementType.Button));
		}

		public UIElement FindButtonWithAutomationId(string automationId)
		{
			return Create(() => Element.FindButtonWithAutomationId(automationId));
		}

		protected UIElement FindButtonWithClass(string name, string className)
		{
			return Create(() => Element.FindButtonWithClass(name, className));
		}

		public Func<AppiumWebElement> FindCheckBoxWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.CheckBox, automationId);
		}

		protected UIElement FindText()
		{
			return Create(() => Element.FindChild(ElementType.Text));
		}

		protected UIElement FindTextWithAutomationId(string automationId)
		{
			return Create(() => Element.FindChildWithAutomationId(ElementType.Text, automationId));
		}

		public Func<AppiumWebElement> FindComboBoxWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.ComboBox, automationId);
		}

		public Func<AppiumWebElement> FindComboBox()
		{
			return () => Element.FindChild(ElementType.ComboBox);
		}

		protected Func<AppiumWebElement> FindTabWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.Tab, automationId);
		}

		protected UIElement FindPaneWithAutomationId(string automationId)
		{
			return Create(() => Element.FindChildWithAutomationId(ElementType.Pane, automationId));
		}

		protected Func<AppiumWebElement> FindMenuBar(string name)
		{
			return () => Element.FindChild(ElementType.MenuBar, name);
		}

		public UIElement FindGroupWithAutomationId(string automationId)
		{
			return Create(() => Element.FindChildWithAutomationId(ElementType.Group, automationId));
		}

		public UIElement FindTree()
		{
			return Create(() => Element.FindTree());
		}

		protected UIElement FindTreeWithAutomationId(string automationId)
		{
			return Create(() => Element.FindTreeWithAutomationId(automationId));
		}

		public Func<AppiumWebElement> FindListWithAutomationId(string automationId)
		{
			return () => Element.FindChildWithAutomationId(ElementType.List, automationId);
		}

		public Func<AppiumWebElement> FindList()
		{
			return () => Element.FindChild(ElementType.List);
		}

		protected Func<AppiumWebElement> WaitForListWithAutomationId(string automationId)
		{
			return () => WaitForChild(x => x.FindChildrenWithAutomationId(ElementType.List, automationId));
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

			var child = children.FirstOrDefault();

			if (child != null) return child;

			throw new Exception($"Child cannot be found in given timeout: {timeout}");
		}
	}
}