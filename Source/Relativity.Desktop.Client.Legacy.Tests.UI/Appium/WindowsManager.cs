namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

	using OpenQA.Selenium.Appium.Windows;

	internal class WindowsManager
	{
		private static readonly TimeSpan DefaultWindowTimeout = TimeSpan.FromSeconds(30);

		private readonly WindowsDriver<WindowsElement> session;

		private readonly List<WindowDetails> windows = new List<WindowDetails>();

		public WindowsManager(WindowsDriver<WindowsElement> session)
		{
			this.session = session;
		}

		public WindowDetails GetWindow(string title)
		{
			return GetWindow(title, x => true);
		}

		public WindowDetails GetWindow(string title, Func<WindowDetails, bool> predicate)
		{
			return GetWindow(title, predicate, DefaultWindowTimeout);
		}

		public WindowDetails GetWindow(string title, Func<WindowDetails, bool> predicate, TimeSpan timeout)
		{
			var window = GetOrCreateWindow(title, predicate);

			if (window == null && (window = WaitAndGetWindow(title, predicate, timeout)) == null)
			{
				throw new Exception($"Waiting for window: \"{title}\" timed out.");
			}

			return window;
		}

		public void SwitchToWindow(string windowHandle)
		{
			session.SwitchTo().Window(windowHandle);
		}

		public bool IsOpen(string windowHandle)
		{
			return session.WindowHandles.Any(x => x == windowHandle);
		}

		private WindowDetails GetOrCreateWindow(string title, Func<WindowDetails, bool> predicate)
		{
			RefreshSessionWindows();

			WindowDetails window;
			if ((window = FindWindow(title, predicate)) == null)
			{
				return CreateWindow(title, predicate);
			}

			SwitchToWindow(window);
			return window;
		}

		private WindowDetails FindWindow(string title, Func<WindowDetails, bool> predicate)
		{
			return windows.FirstOrDefault(x => string.Equals(x.Title, title) && predicate(x));
		}

		private WindowDetails WaitAndGetWindow(string title, Func<WindowDetails, bool> predicate, TimeSpan timeout)
		{
			WindowDetails window = null;
			StringBuilder sb = new StringBuilder();

			Wait.For(
				() =>
					{
						sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " Checking for: " + title);
						window = GetOrCreateWindow(title, predicate);
						return window != null;
					},
				timeout);

			File.AppendAllText(@"C:\Adrian\Temp\spin.log", sb.ToString());
			return window;
		}

		private WindowDetails CreateWindow(string name, Func<WindowDetails, bool> predicate)
		{
			for (int i = 0; i < this.windows.Count; i++)
			{
				if (windows[i].Element == null)
				{
					SwitchToWindow(this.windows[i]);
					windows[i] = this.CreateWindowFromSession();

					if (string.Equals(this.windows[i].Title, name) && predicate(this.windows[i]))
					{
						return windows[i];
					}
				}
			}

			return null;
		}

		private void SwitchToWindow(WindowDetails window)
		{
			session.SwitchTo().Window(window.Handle);
		}

		private void RefreshSessionWindows()
		{
			var currentHandles = this.session.WindowHandles.ToList();

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
					var window = this.session.CurrentWindowHandle == windowHandle
						             ? this.CreateWindowFromSession()
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
				session.FindElementByName(this.session.Title));
		}

		private void RemoveNonExistingWindows(List<string> currentHandles)
		{
			for (int i = this.windows.Count - 1; i >= 0; i--)
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