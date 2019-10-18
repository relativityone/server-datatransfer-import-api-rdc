namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	using OpenQA.Selenium.Appium;
	using OpenQA.Selenium.Appium.Windows;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

	public class LoginWindow
	{
		private readonly WindowsElement window;

		public LoginWindow(WindowsElement window)
		{
			this.window = window;
		}

		public void Login(string email, string password)
		{
			this.EnterEmail(email);
			this.ClickContinueButton();
			this.EnterPassword(password);
			this.ClickLoginButton();
		}

		public AppiumWebElement GetEmailEdit()
		{
			return this.window.FindEditWithAutomationId("_email");
		}

		public AppiumWebElement GetContinueButton()
		{
			return this.window.FindButton("Continue");
		}

		public AppiumWebElement GetLoginButton()
		{
			return this.window.FindElementByAccessibilityId("_login");
		}

		public AppiumWebElement GetPasswordEdit()
		{
			return this.window.FindElementByAccessibilityId("_password__password_TextBox");
		}

		public void EnterEmail(string email)
		{
			var emailTextBox = this.GetEmailEdit();
			emailTextBox.SendKeys(email);
		}

		public void ClickContinueButton()
		{
			var continueButton = this.GetContinueButton();
			continueButton.Click();
		}

		public void ClickLoginButton()
		{
			var loginButton = this.window.FindElementByAccessibilityId("_login");
			loginButton.Click();
		}

		public void EnterPassword(string password)
		{
			var passwordTextBox = this.GetPasswordEdit();
			passwordTextBox.SendKeys(password);
		}
	}
}