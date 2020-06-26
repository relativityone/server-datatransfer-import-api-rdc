def SendSlackNotification(String serverUnderTestName, String version, String branch, String buildType, String slackChannel, int numberOfFailedTests, int numberOfPassedTests, int numberOfSkippedTests, String message)
{
    echo "Send Slack Notification"

    def script = this
	def email = powershell(returnStdout: true, script: "git --no-pager show -s --format='%ae'").trim()
	def channel = "iapi-pipeline-" + "${slackChannel}"
    echo "*************************************************" +
        "\n" +
        "\n" + "sendCDSlackNotification Parameters: " +
        "\n" +
        "\n" + "script: " + script +
        "\n" + "serverUnderTestName: " + serverUnderTestName +
        "\n" + "version: " + version +
        "\n" + "branch: " + branch +
        "\n" + "buildType: " + buildType +
        "\n" + "slackChannel: " + channel +
        "\n" + "email: " + email +
        "\n" + "numberOfFailedTests: " + numberOfFailedTests +
        "\n" + "numberOfPassedTests: " + numberOfPassedTests +
        "\n" + "numberOfSkippedTests: " + numberOfSkippedTests +
        "\n" + "message: " + message +
        "\n" +
        "\n*************************************************"
    try
    {
        sendCDSlackNotification(script, serverUnderTestName, version, branch, buildType, channel, email, ['tests': ['passed': numberOfPassedTests, 'failed': numberOfFailedTests, 'skipped': numberOfSkippedTests]], message, "CD" )
    }
    catch(err)
    {
        echo "Send slack notification failed"
        echo err.toString()
    }
}

return this;