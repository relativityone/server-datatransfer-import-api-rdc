namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using OpenQA.Selenium.Appium.Windows;

	internal class WindowsManager
	{
		private readonly WindowsDriver<WindowsElement> session;

		private readonly List<WindowDetails> windows = new List<WindowDetails>();

		public WindowsManager(WindowsDriver<WindowsElement> session)
		{
			this.session = session;
		}

		public WindowDetails GetWindow(string title, Func<WindowDetails, bool> predicate)
		{
			RefreshSessionWindows();

			var window = this.windows.FirstOrDefault(x => string.Equals(x.Title, title) && predicate(x));

			if (window != null)
			{
				SwitchToWindow(window);
				return window;
			}

			return CreateWindow(title, predicate);
		}

		public WindowDetails GetWindow(string title)
		{
			return GetWindow(title, x => true);
		}

		private WindowDetails CreateWindow(string name, Func<WindowDetails, bool> predicate)
		{
			for (int i = 0; i < this.windows.Count; i++)
			{
				if (windows[i].WindowsElement == null)
				{
					SwitchToWindow(this.windows[i]);
					windows[i] = this.CreateWindowFromSession();

					if (string.Equals(this.windows[i].Title, name) && predicate(this.windows[i]))
					{
						return windows[i];
					}
				}
			}

			throw new Exception($"Window \"{name}\" doesn't exits");
		}

		private void SwitchToWindow(WindowDetails window)
		{
			session.SwitchTo().Window(window.WindowHandle);
		}

		private void RefreshSessionWindows()
		{
			var currentHandles = this.session.WindowHandles.ToList();

			RemoveNonExistingWindows(currentHandles);

			foreach (var windowHandle in currentHandles)
			{
				if (!windows.Any(x => string.Equals(x.WindowHandle, windowHandle)))
				{
					var window = this.session.CurrentWindowHandle == windowHandle
						             ? this.CreateWindowFromSession()
						             : new WindowDetails(windowHandle);
					windows.Add(window);
				}
			}
		}

		private WindowDetails CreateWindowFromSession()
		{
			return new WindowDetails(
				session.CurrentWindowHandle,
				session.Title,
				session.FindElementByName(this.session.Title));
		}

		private void RemoveNonExistingWindows(List<string> currentHandles)
		{
			for (int i = this.windows.Count - 1; i >= 0; i--)
			{
				var windowData = windows[i];
				if (!currentHandles.Contains(windowData.WindowHandle))
				{
					windows.RemoveAt(i);
				}
			}
		}

		internal class WindowDetails
		{
			public WindowDetails(string windowHandle)
			{
				WindowHandle = windowHandle;
			}

			public WindowDetails(string windowHandle, string title, WindowsElement windowsElement)
				: this(windowHandle)
			{
				Title = title;
				WindowsElement = windowsElement;
			}

			public string WindowHandle { get; }

			public WindowsElement WindowsElement { get; }

			public string Title { get; }
		}
	}
}