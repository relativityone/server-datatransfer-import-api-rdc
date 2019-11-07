using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class LoginWindow : RdcWindowBase
	{
		private readonly UIElement continueButton;
		private readonly EditUIElement emailEdit;
		private readonly UIElement loginButton;
		private readonly EditUIElement passwordEdit;
		private readonly SecurityAlertDialog securityAlertDialog;

		public LoginWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			emailEdit = FindEditWithAutomationId("_email");
			continueButton = FindButton("Continue");
			loginButton = FindButtonWithAutomationId("_login");
			passwordEdit = FindEditWithAutomationId("_password__password_TextBox");
			securityAlertDialog = new SecurityAlertDialog(WaitForWindow("Security Alert"));
		}

		public SelectWorkspaceWindow Login(string email, string password)
		{
			if (securityAlertDialog.Exists)
			{
				securityAlertDialog.ClickYesButton();
			}

			EnterEmail(email);
			ClickContinueButton();
			EnterPassword(password);
			ClickLoginButton();
			return WindowsManager.SwitchToSelectWorkspaceWindow();
		}

		public void EnterEmail(string email)
		{
			emailEdit.SendKeys(email);
		}

		public void ClickContinueButton()
		{
			continueButton.Click();
		}

		public void ClickLoginButton()
		{
			loginButton.Click();
		}

		public void EnterPassword(string password)
		{
			passwordEdit.SendKeys(password);
		}
	}
}