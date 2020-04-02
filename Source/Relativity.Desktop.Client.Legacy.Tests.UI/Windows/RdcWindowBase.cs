using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class RdcWindowBase<T> : WindowBase<T> where T : UIElement<T>
	{
		protected readonly RdcWindowsManager WindowsManager;
		private readonly WindowDetails windowsDetails;

		protected RdcWindowBase(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(logger, window)
		{
			WindowsManager = windowsManager;
			windowsDetails = window;
		}

		public void SwitchToWindow()
		{
			WindowsManager.SwitchToWindow(Handle);
		}

		private Size GetWindowSize()
		{
			if (this.windowsDetails != null && this.windowsDetails.Element != null)
			{
				return this.windowsDetails.Element.Size;
			}

			return new Size(0, 0);
		}

		private string GetWindowTitle()
		{
			if (this.windowsDetails != null)
			{
				return this.windowsDetails.Title;
			}
			return String.Empty;
		}

		public void CaptureWindowScreenshot()
		{

			CaptureScreenshot screenshotTool = CaptureScreenshot.GetInstance();
			Size windowSize = this.GetWindowSize();

			if (windowSize.Width !=0 && windowSize.Height != 0)
			{
				int leftUpperX, leftUpperY;
				Size imageSize;
				string windowCaption = this.GetWindowTitle();

				if (windowCaption.Contains("| Export") || windowCaption.Contains("Progress"))
				{
					// Window screenshot with pixels around
					leftUpperX = 0;
					leftUpperY = 0;
					imageSize = new Size(Screen.PrimaryScreen.Bounds.Width / 2 + windowSize.Width / 2,
						Screen.PrimaryScreen.Bounds.Height / 2 + windowSize.Height / 2 + 25);
				}
				else
				{
					// Window screenshot
					leftUpperX = Screen.PrimaryScreen.Bounds.Width / 2 - windowSize.Width / 2;
					leftUpperY = Screen.PrimaryScreen.Bounds.Height / 2 - windowSize.Height / 2 - 25;
					imageSize = new Size(windowSize.Width + 25, windowSize.Height + 25);
				}

				if (windowCaption != String.Empty)
				{
					windowCaption = windowCaption.Replace("|", "").Replace("  ", " ").Replace("...", "").Replace(" .", ".");
				}

				screenshotTool.CaptureWindowScreenshot(leftUpperX, leftUpperY, imageSize, windowCaption, Logger);
			}
			else
			{
				screenshotTool.CaptureDesktopScreenshot(Logger);
			}
		}
	}
}