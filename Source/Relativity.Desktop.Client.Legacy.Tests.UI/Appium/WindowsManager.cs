using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Appium.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal class WindowsManager
	{
		private readonly WindowsDriver<WindowsElement> session;
		private readonly List<WindowDetails> windows = new List<WindowDetails>();

		public WindowsManager(WindowsDriver<WindowsElement> session)
		{
			this.session = session;
		}

		public bool TryGetWindow(WindowName name, out WindowDetails window)
		{
			return TryGetWindow(name, x => true, out window);
		}

		public bool TryGetWindow(WindowName name, Func<WindowDetails, bool> predicate, out WindowDetails window)
		{
			return TryFindOrCreateWindow(name, predicate, out window);
		}

		public bool TryGetWindow(WindowName name, TimeSpan timeout, out WindowDetails window)
		{
			return TryGetWindow(name, x => true, timeout, out window);
		}

		public bool TryGetWindow(WindowName name, Func<WindowDetails, bool> predicate, TimeSpan timeout,
			out WindowDetails window)
		{
			WindowDetails foundWindow = null;
			var found = Wait.For(() => TryGetWindow(name, predicate, out foundWindow), timeout);
			window = foundWindow;
			return found;
		}

		public void SwitchToWindow(string windowHandle)
		{
			session.SwitchTo().Window(windowHandle);
		}

		public bool IsOpen(string windowHandle)
		{
			return session.WindowHandles.Any(x => x == windowHandle);
		}

		private bool TryFindOrCreateWindow(WindowName name, Func<WindowDetails, bool> predicate, out WindowDetails window)
		{
			RefreshSessionWindows();

			window = FindWindow(name, predicate);
			if (window == null)
			{
				return TryCreateWindow(name, predicate, out window);
			}

			SwitchToWindow(window);
			return true;
		}

		private WindowDetails FindWindow(WindowName name, Func<WindowDetails, bool> predicate)
		{
			return windows.FirstOrDefault(x => string.Equals(x.Title, name) && predicate(x));
		}

		private bool TryCreateWindow(WindowName name, Func<WindowDetails, bool> predicate, out WindowDetails window)
		{
			for (var i = 0; i < windows.Count; i++)
			{
				if (windows[i].Element == null)
				{
					SwitchToWindow(windows[i]);
					windows[i] = CreateWindowFromSession();

					if (string.Equals(windows[i].Title, name) && predicate(windows[i]))
					{
						window = windows[i];
						return true;
					}
				}
			}

			window = null;
			return false;
		}

		private void SwitchToWindow(WindowDetails window)
		{
			session.SwitchTo().Window(window.Handle);
		}

		private void RefreshSessionWindows()
		{
			var currentHandles = session.WindowHandles.ToList();

			RemoveNonExistingWindows(currentHandles);

			var newWindows = GetNewWindows(currentHandles).ToList();

			windows.AddRange(newWindows);
		}

		private IEnumerable<WindowDetails> GetNewWindows(IEnumerable<string> currentHandles)
		{
			foreach (var windowHandle in currentHandles)
			{
				if (!windows.Any(x => string.Equals(x.Handle, windowHandle)))
				{
					var window = session.CurrentWindowHandle == windowHandle
						? CreateWindowFromSession()
						: new WindowDetails(windowHandle);
					yield return window;
				}
			}
		}

		private WindowDetails CreateWindowFromSession()
		{
			return new WindowDetails(
				session.CurrentWindowHandle,
				session.Title,
				session.FindElementByName(session.Title));
		}

		private void RemoveNonExistingWindows(List<string> currentHandles)
		{
			for (var i = windows.Count - 1; i >= 0; i--)
			{
				var windowData = windows[i];
				if (!currentHandles.Contains(windowData.Handle))
				{
					windows.RemoveAt(i);
				}
			}
		}
	}
}