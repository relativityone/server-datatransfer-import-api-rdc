using System;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal abstract class UIElement<T> where T : UIElement<T>
	{
		private readonly Func<AppiumWebElement> create;
		private AppiumWebElement appiumWebElement;
		private TimeSpan waitForTimeout = TimeSpan.Zero;

		protected UIElement(Func<AppiumWebElement> create)
		{
			this.create = create;
		}

		private AppiumWebElement Element => appiumWebElement ?? (appiumWebElement = CreateElement());
		public string Text => Element.Text;
		public bool Selected => Element.Selected;

		public bool Exists
		{
			get
			{
				try
				{
					return Element != null;
				}
				catch (InvalidOperationException)
				{
					return false;
				}
			}
		}

		public T WaitFor()
		{
			waitForTimeout = DefaultTimeouts.FindElement;
			return (T)this;
		}

		public T WaitFor(TimeSpan timeout)
		{
			waitForTimeout = timeout;
			return (T) this;
		}

		public void Click()
		{
			Element.Click();
		}

		public void SendKeys(string text)
		{
			Element.SendKeys(text);
		}

		public T WaitToBeVisible(TimeSpan timeout)
		{
			return WaitFor(x => x.Displayed, timeout);
		}

		public T WaitToBeVisible()
		{
			return WaitToBeVisible(DefaultTimeouts.WaitForProperty);
		}

		public T WaitToBeEnabled(TimeSpan timeout)
		{
			return WaitFor(x => x.Enabled, timeout);
		}

		public T WaitToBeEnabled()
		{
			return WaitToBeEnabled(DefaultTimeouts.WaitForProperty);
		}

		public T WaitToBeSelected(TimeSpan timeout)
		{
			return WaitFor(x => x.Selected, timeout);
		}

		public T WaitToBeSelected()
		{
			return WaitToBeSelected(DefaultTimeouts.WaitForProperty);
		}

		protected EditUIElement FindEdit(string name)
		{
			return new EditUIElement(FindChild(ElementType.Edit, name));
		}

		protected EditUIElement FindEdit()
		{
			return new EditUIElement(FindChild(ElementType.Edit));
		}

		public EditUIElement FindEditWithAutomationId(string automationId)
		{
			return new EditUIElement(FindChildWithAutomationId(ElementType.Edit, automationId));
		}

		protected ButtonUIElement FindButton(string name)
		{
			return new ButtonUIElement(FindChild(ElementType.Button, name));
		}

		protected ButtonUIElement FindButton()
		{
			return new ButtonUIElement(FindChild(ElementType.Button));
		}

		public ButtonUIElement FindButtonWithAutomationId(string automationId)
		{
			return new ButtonUIElement(FindChildWithAutomationId(ElementType.Button, automationId));
		}

		protected ButtonUIElement FindButtonWithClass(string name)
		{
			return new ButtonUIElement(FindChildWithClass(ElementType.Button, name, ElementClass.Button));
		}

		public CheckBoxUIElement FindCheckBoxWithAutomationId(string automationId)
		{
			return new CheckBoxUIElement(FindChildWithAutomationId(ElementType.CheckBox, automationId));
		}

		protected TextUIElement FindText()
		{
			return new TextUIElement(FindChild(ElementType.Text));
		}

		protected TextUIElement FindTextWithAutomationId(string automationId)
		{
			return new TextUIElement(FindChildWithAutomationId(ElementType.Text, automationId));
		}

		public ComboBoxUIElement FindComboBoxWithAutomationId(string automationId)
		{
			return new ComboBoxUIElement(FindChildWithAutomationId(ElementType.ComboBox, automationId));
		}

		public SpinnerComboBoxUIElement FindSpinnerComboBoxWithAutomationId(string automationId)
		{
			return new SpinnerComboBoxUIElement(FindChildWithAutomationId(ElementType.ComboBox, automationId));
		}

		public ComboBoxUIElement FindComboBox()
		{
			return new ComboBoxUIElement(FindChild(ElementType.ComboBox));
		}

		protected TabsUIElement FindTabsWithAutomationId(string automationId)
		{
			return new TabsUIElement(FindChildWithAutomationId(ElementType.Tab, automationId));
		}

		protected PaneUIElement FindPaneWithAutomationId(string automationId)
		{
			return new PaneUIElement(FindChildWithAutomationId(ElementType.Pane, automationId));
		}

		protected MenuItemUIElement FindMenuBar(string name)
		{
			return new MenuItemUIElement(FindChild(ElementType.MenuBar, name));
		}

		protected GroupUIElement FindGroupWithAutomationId(string automationId)
		{
			return new GroupUIElement(FindChildWithAutomationId(ElementType.Group, automationId));
		}

		public TreeUIElement FindTree()
		{
			return new TreeUIElement(FindChild(ElementType.Tree));
		}

		protected TreeUIElement FindTreeWithAutomationId(string automationId)
		{
			return new TreeUIElement(FindChildWithAutomationId(ElementType.Tree, automationId));
		}

		public ListUIElement FindListWithAutomationId(string automationId)
		{
			return new ListUIElement(FindChildWithAutomationId(ElementType.List, automationId));
		}

		public ListUIElement FindList()
		{
			return new ListUIElement(FindChild(ElementType.List));
		}

		protected Func<AppiumWebElement> FindWindow(string name)
		{
			return () => Element.FindChild(ElementType.Window, name);
		}

		protected Func<AppiumWebElement> FindChild(string elementType, string name)
		{
			return () => Element.FindChild(elementType, name);
		}

		private Func<AppiumWebElement> FindChild(string elementType)
		{
			return () => Element.FindChild(elementType);
		}

		private Func<AppiumWebElement> FindChildWithAutomationId(
			string elementType,
			string automationId)
		{
			return () => Element.FindChildWithAutomationId(elementType, automationId);
		}

		private Func<AppiumWebElement> FindChildWithClass(
			string elementType,
			string name,
			string className)
		{
			return () => Element.FindChildWithClass(elementType, name, className);
		}

		protected IReadOnlyList<AppiumWebElement> FindChildren(string elementType)
		{
			return Element.FindChildren(elementType);
		}

		private AppiumWebElement CreateElement()
		{
			return waitForTimeout > TimeSpan.Zero ? create.WaitFor(waitForTimeout)() : create();
		}

		private T WaitFor(Func<AppiumWebElement, bool> condition, TimeSpan timeout)
		{
			Wait.For(() => condition(Element), timeout);
			return (T)this;
		}
	}
}