using NUnit.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public sealed class CaptureScreenshot
	{
		private int imageCounter;
		private static CaptureScreenshot _instance = null;
		private readonly string testResultsFolder;

		private CaptureScreenshot()
		{
			this.testResultsFolder = String.Empty;
			_instance = this;
			this.imageCounter = 0;
		}

		private CaptureScreenshot(string testResultFolder)
		{
			this.testResultsFolder = testResultFolder;
			_instance = this;
			this.imageCounter = 0;
		}

		public static void SetUpScreenshotTool(string logFolderPath)
		{
			_instance = new CaptureScreenshot(logFolderPath);

			if (logFolderPath != String.Empty)
			{
				if (!Directory.Exists(logFolderPath))
				{
					Directory.CreateDirectory(logFolderPath);
				}

				string[] picturesFromPreviousTests = Directory.GetFiles(logFolderPath, $"*{TestContext.CurrentContext.Test.Name}*");
				foreach (string filePath in picturesFromPreviousTests)
				{
					File.Delete(filePath);
				}
			}
			else
			{
				throw new Exception("Folder for store images not set");
			}
		}

		public static CaptureScreenshot GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CaptureScreenshot(String.Empty);
			}
			return _instance;
		}

		public void CaptureWindowScreenshot(int leftUpperX, int leftUpperY, Size imageSize, string caption, ILog logger)
		{
			try
			{
				using (Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height))
				{
					using (Graphics graphic = Graphics.FromImage(bitmap))
					{
						graphic.CopyFromScreen(leftUpperX, leftUpperY, 0, 0, imageSize);
					}

					imageCounter += 1;
					string fileName = $"{TestContext.CurrentContext.Test.ID}_{TestContext.CurrentContext.Test.Name}_{imageCounter}_{caption}.png";
					string filePath = Path.Combine(this.testResultsFolder, fileName);
					
					bitmap.Save(filePath, ImageFormat.Png);

					if (logger != null)
					{
						logger.LogInformation($"CaptureWindowScreenshot: '{fileName}'");
					}
					else
					{
						Console.WriteLine($"CaptureWindowScreenshot: '{fileName}'");
					}

					TestContext.AddTestAttachment(filePath, caption.ToString());
				}
			}
			catch (Exception e)
			{
				if (logger != null)
				{
					logger.LogError("CaptureWindow '" + caption + "' error: " + e.Message);
				}
				else
				{
					Console.WriteLine("CaptureWindow '" + caption + "' error: " + e.Message);
				}
				
			}
		}

		public void CaptureDesktopScreenshot(ILog logger)
		{
			CaptureWindowScreenshot(0, 0, new Size(Screen.PrimaryScreen.Bounds.Width,
				Screen.PrimaryScreen.Bounds.Height), "Desktop screenshot", logger);
		}
	}
}
