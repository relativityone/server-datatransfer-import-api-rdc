using System;
using System.Linq;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TreeUIElement : UIElement<TreeUIElement>
	{
		public TreeUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}

		public TreeUIElement FindTreeItem()
		{
			return new TreeUIElement(Logger, FindTreeChild());
		}

		private Func<AppiumWebElement> FindTreeChild()
		{
			return () =>
			{
				var children = FindChildren(ElementType.Tree);

				if (children.Any())
				{
					return children.First();
				}

				return FindChild(ElementType.TreeItem)();
			};
		}
	}
}