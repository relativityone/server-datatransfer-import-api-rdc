using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal abstract class UIElement<T> where T : UIElement<T>
	{
		private readonly Func<AppiumWebElement> create;
		private AppiumWebElement appiumWebElement;
		private TimeSpan waitForTimeout = TimeSpan.Zero;
		private string elementDescription = "Unknown";
		protected readonly ILog Logger;

		protected UIElement(ILog logger, Func<AppiumWebElement> create)
		{
			this.Logger = logger;
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

		public bool WaitToNotExist(TimeSpan timeout)
		{
			var elementDetails = ToString();
			Logger.LogDebug("Waiting for element: {0} to not exist", elementDetails);
			var originalWaitForTimeout = waitForTimeout;

			var notExist = Wait.For(() =>
			{
				appiumWebElement = null;
				return !Exists;
			}, timeout);

			waitForTimeout = originalWaitForTimeout;
			Logger.LogDebug("Waiting on element: {0} to not exist {1}", elementDetails, GetResultMessage(notExist));
			return notExist;
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

		public virtual void Click()
		{
			Logger.LogDebug("Clicking on element: {0}", ToString());
			Element.Click();
		}

		public void SendKeys(string text)
		{
			Element.SendKeys(text);
		}

		public T WaitToBeVisible(TimeSpan timeout)
		{
			string parentDetails = ToString();
			Logger.LogDebug("Waiting for element: {0} to be visible. Timeout {1}", parentDetails, timeout);
			return WaitFor(x => x.Displayed, timeout);
		}

		public T WaitToBeVisible()
		{
			return WaitToBeVisible(DefaultTimeouts.WaitForProperty);
		}

		public T WaitToBeEnabled(TimeSpan timeout)
		{
			string parentDetails = ToString();
			Logger.LogDebug("Waiting for element: {0} to be enabled. Timeout {1}", parentDetails, timeout);
			return WaitFor(x => x.Enabled, timeout);
		}

		public T WaitToBeEnabled()
		{
			return WaitToBeEnabled(DefaultTimeouts.WaitForProperty);
		}

		public T WaitToBeSelected(TimeSpan timeout)
		{
			string parentDetails = ToString();
			Logger.LogDebug("Waiting for element: {0} to be selected. Timeout {1}", parentDetails, timeout);
			return WaitFor(x => x.Selected, timeout);
		}

		public T WaitToBeSelected()
		{
			return WaitToBeSelected(DefaultTimeouts.WaitForProperty);
		}

		protected EditUIElement FindEdit(string name)
		{
			return new EditUIElement(Logger, FindChild(ElementType.Edit, name));
		}

		protected EditUIElement FindEdit()
		{
			return new EditUIElement(Logger, FindChild(ElementType.Edit));
		}

		public EditUIElement FindEditWithAutomationId(string automationId)
		{
			return new EditUIElement(Logger, FindChildWithAutomationId(ElementType.Edit, automationId));
		}

		protected ButtonUIElement FindButton(string name)
		{
			return new ButtonUIElement(Logger, FindChild(ElementType.Button, name));
		}

		protected ButtonUIElement FindButton()
		{
			return new ButtonUIElement(Logger, FindChild(ElementType.Button));
		}

		public ButtonUIElement FindButtonWithAutomationId(string automationId)
		{
			return new ButtonUIElement(Logger, FindChildWithAutomationId(ElementType.Button, automationId));
		}

		protected ButtonUIElement FindButtonWithClass(string name)
		{
			return new ButtonUIElement(Logger, FindChildWithClass(ElementType.Button, name, ElementClass.Button));
		}

		public CheckBoxUIElement FindCheckBoxWithAutomationId(string automationId)
		{
			return new CheckBoxUIElement(Logger, FindChildWithAutomationId(ElementType.CheckBox, automationId));
		}

		public TextUIElement FindText()
		{
			return new TextUIElement(Logger, FindChild(ElementType.Text));
		}

		protected TextUIElement FindTextWithAutomationId(string automationId)
		{
			return new TextUIElement(Logger, FindChildWithAutomationId(ElementType.Text, automationId));
		}

		public ComboBoxUIElement FindComboBoxWithAutomationId(string automationId)
		{
			return new ComboBoxUIElement(Logger, FindChildWithAutomationId(ElementType.ComboBox, automationId));
		}

		public SpinnerComboBoxUIElement FindSpinnerComboBoxWithAutomationId(string automationId)
		{
			return new SpinnerComboBoxUIElement(Logger, FindChildWithAutomationId(ElementType.ComboBox, automationId));
		}

		public ComboBoxUIElement FindComboBox()
		{
			return new ComboBoxUIElement(Logger, FindChild(ElementType.ComboBox));
		}

		protected TabsUIElement FindTabsWithAutomationId(string automationId)
		{
			return new TabsUIElement(Logger, FindChildWithAutomationId(ElementType.Tab, automationId));
		}

		protected PaneUIElement FindPaneWithAutomationId(string automationId)
		{
			return new PaneUIElement(Logger, FindChildWithAutomationId(ElementType.Pane, automationId));
		}

		protected MenuItemUIElement FindMenuBar(string name)
		{
			return new MenuItemUIElement(Logger, FindChild(ElementType.MenuBar, name));
		}

		protected GroupUIElement FindGroupWithAutomationId(string automationId)
		{
			return new GroupUIElement(Logger, FindChildWithAutomationId(ElementType.Group, automationId));
		}

		public TreeUIElement FindTree()
		{
			return new TreeUIElement(Logger, FindChild(ElementType.Tree));
		}

		protected TreeUIElement FindTreeWithAutomationId(string automationId)
		{
			return new TreeUIElement(Logger, FindChildWithAutomationId(ElementType.Tree, automationId));
		}

		public ListUIElement FindListWithAutomationId(string automationId)
		{
			return new ListUIElement(Logger, FindChildWithAutomationId(ElementType.List, automationId));
		}

		public StatusBarUIElement FindStatusBarWithAutomationId(string automationId)
		{
			return new StatusBarUIElement(Logger, FindChildWithAutomationId(ElementType.StatusBar, automationId));
		}

		public ListUIElement FindList()
		{
			return new ListUIElement(Logger, FindChild(ElementType.List));
		}

		protected Func<AppiumWebElement> FindWindow(string name)
		{
			return () => FindElement(ElementType.Window, name);
		}

		protected Func<AppiumWebElement> FindChild(string elementType, string name)
		{
			return () => FindElement(elementType, name);
		}

		protected Func<AppiumWebElement> FindChild(string elementType)
		{
			return () => FindElementByXPath($"*//{elementType}");
		}

		private Func<AppiumWebElement> FindChildWithAutomationId(
			string elementType,
			string automationId)
		{
			return () => FindElementByXPath($"*//{elementType}[@AutomationId=\"{automationId}\"]");
		}

		private Func<AppiumWebElement> FindChildWithClass(
			string elementType,
			string name,
			string className)
		{
			return () => FindElementByXPath($"*//{elementType}[@ClassName=\"{className}\"][@Name=\"{name}\"]");
		}

		protected IReadOnlyList<AppiumWebElement> FindChildren(string elementType)
		{
			return FindElementsByXPath($"*//{elementType}");
		}

		private AppiumWebElement CreateElement()
		{
			return waitForTimeout > TimeSpan.Zero ? create.WaitFor(waitForTimeout)() : create();
		}

		private T WaitFor(Func<AppiumWebElement, bool> condition, TimeSpan timeout)
		{
			bool success = Wait.For(() => condition(Element), timeout);
			Logger.LogDebug("Waiting for element status {0}", GetResultMessage(success));
			return (T)this;
		}

		private static string GetResultMessage(bool success)
		{
			return success ? "SUCCEED" : "FAILED";
		}

		private AppiumWebElement FindElement(string elementType, string name)
		{
			return FindElementByXPath($"*//{elementType}[@Name=\"{name}\"]");
		}

		private AppiumWebElement FindElementByXPath(string xPath)
		{
			string parentDetails = ToString();
			Logger.LogDebug("Finding element on parent: {0} by XPath: {1}", parentDetails, xPath);
			var child = Element.FindElementByXPath(xPath);
			Logger.LogDebug("Element found on parent: {0} by XPath: {1}", parentDetails, xPath);
			return child;
		}

		private ReadOnlyCollection<AppiumWebElement> FindElementsByXPath(string xPath)
		{
			string parentDetails = ToString();
			Logger.LogDebug("Finding elements on parent: {0} by XPath: {1}", parentDetails, xPath);
			var children = Element.FindElementsByXPath(xPath);
			Logger.LogDebug("{0} elements found on parent: {1} by XPath: {2}", children.Count, parentDetails, xPath);
			return children;
		}

		public override string ToString()
		{
			try
			{
				elementDescription = $"[TagName: {Element.TagName}, Text: {Element.Text}]";
			}
			catch (InvalidOperationException)
			{
				Logger.LogDebug("Failed to get element description. Previous element description is: {0}", elementDescription);
			}

			return elementDescription;
		}
	}
}