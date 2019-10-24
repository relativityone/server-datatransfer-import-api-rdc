using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class LoginWindow : WindowBase
	{
		public LoginWindow(WindowDetails window)
			: base(window)
		{
		}

		public void Login(string email, string password)
		{
			EnterEmail(email);
			ClickContinueButton();
			EnterPassword(password);
			ClickLoginButton();
		}

		public AppiumWebElement GetEmailEdit()
		{
			return Element.FindEditWithAutomationId("_email");
		}

		public AppiumWebElement GetContinueButton()
		{
			return Element.FindButton("Continue");
		}

		public AppiumWebElement GetLoginButton()
		{
			return Element.FindButtonWithAutomationId("_login");
		}

		public AppiumWebElement GetPasswordEdit()
		{
			return Element.FindEditWithAutomationId("_password__password_TextBox");
		}

		public void EnterEmail(string email)
		{
			var emailTextBox = GetEmailEdit();
			emailTextBox.SendKeys(email);
		}

		public void ClickContinueButton()
		{
			var continueButton = GetContinueButton();
			continueButton.Click();
		}

		public void ClickLoginButton()
		{
			var loginButton = GetLoginButton();
			loginButton.Click();
		}

		public void EnterPassword(string password)
		{
			var passwordTextBox = GetPasswordEdit();
			passwordTextBox.SendKeys(password);
		}
	}
}