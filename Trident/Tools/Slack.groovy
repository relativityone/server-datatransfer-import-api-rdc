def SendSlackNotification(String serverUnderTestName, String version, String branch, String buildType, String slackChannel, int numberOfFailedTests, int numberOfPassedTests, int numberOfSkippedTests, String message)
{
    echo "Send Slack Notification"

    def script = this
    def String email = "slack_svc@relativity.com"

    echo "*************************************************" +
        "\n" +
        "\n" + "sendCDSlackNotification Parameters: " +
        "\n" +
        "\n" + "script: " + script +
        "\n" + "serverUnderTestName: " + serverUnderTestName +
        "\n" + "version: " + version +
        "\n" + "branch: " + branch +
        "\n" + "buildType: " + buildType +
        "\n" + "slackChannel: " + slackChannel +
        "\n" + "email: " + email +
        "\n" + "numberOfFailedTests: " + numberOfFailedTests +
        "\n" + "numberOfPassedTests: " + numberOfPassedTests +
        "\n" + "numberOfSkippedTests: " + numberOfSkippedTests +
        "\n" + "message: " + message +
        "\n" +
        "\n*************************************************"
    try
    {
        sendCDSlackNotification(script, serverUnderTestName, version, branch, buildType, slackChannel, email, ['tests': ['passed': numberOfPassedTests, 'failed': numberOfFailedTests, 'skipped': numberOfSkippedTests]], message, "CD" )
    }
    catch(err)
    {
        echo "Send slack notification failed"
        echo err.toString()
    }
}

return this;