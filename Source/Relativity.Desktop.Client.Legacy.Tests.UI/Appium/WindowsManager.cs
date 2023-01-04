using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using Polly;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal class WindowsManager
	{
		private readonly WindowsDriver<WindowsElement> session;
		private readonly List<WindowDetails> windows = new List<WindowDetails>();

		public IReadOnlyCollection<string> OpenedWindowsHandles => Policy<ReadOnlyCollection<string>>
			.Handle<WebDriverException>()
			.Retry(5)
			.Execute(() => session.WindowHandles);

		public WindowsManager(WindowsDriver<WindowsElement> session)
		{
			this.session = session;
		}

		public bool TryGetWindow(WindowName name, Func<WindowDetails, bool> predicate, out WindowDetails window)
		{
			return TryFindOrCreateWindow(name, predicate, out window);
		}

		public bool TryGetWindow(WindowName name,
			Func<WindowDetails, bool> predicate,
			TimeSpan timeout,
			out WindowDetails window)
		{
			var windowHandles = OpenedWindowsHandles;
			bool found = TryGetWindow(name, predicate, out WindowDetails foundWindow);
			var timeoutLeft = timeout;

			while (!found && timeoutLeft > TimeSpan.Zero)
			{
				var waitStartedTime = DateTime.Now;

				if (!WaitForOpeningNewWindow(windowHandles, timeoutLeft))
				{
					break;
				}

				windowHandles = OpenedWindowsHandles;
				found = TryGetWindow(name, predicate, out foundWindow);
				timeoutLeft -= DateTime.Now - waitStartedTime;
			}

			window = foundWindow;
			return found;
		}

		private bool WaitForOpeningNewWindow(IReadOnlyCollection<string> alreadyOpenedWindowsHandles, TimeSpan timeout)
		{
			return Wait.For(() => IsNewWindowOpened(alreadyOpenedWindowsHandles), TimeSpan.FromSeconds(2), timeout);
		}

		private bool IsNewWindowOpened(IReadOnlyCollection<string> previousWindowHandles)
		{
			return OpenedWindowsHandles.Any(x => previousWindowHandles.All(w => w != x));
		}

		public void SwitchToWindow(string windowHandle)
		{
			session.SwitchTo().Window(windowHandle);
		}

		private bool TryFindOrCreateWindow(WindowName name, Func<WindowDetails, bool> predicate, out WindowDetails window)
		{
			RefreshSessionWindows();

			window = FindWindow(name, predicate);
			if (window == null)
			{
				return TryCreateWindow(name, predicate, out window);
			}

			SwitchToWindow(window.Handle);
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
					SwitchToWindow(windows[i].Handle);
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

		private void RefreshSessionWindows()
		{
			var currentHandles = OpenedWindowsHandles;

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

		private void RemoveNonExistingWindows(IReadOnlyCollection<string> currentHandles)
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