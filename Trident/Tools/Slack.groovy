def SendSlackNotification(String serverUnderTestName, String version, String branch, String buildType, String slackChannel, int numberOfFailedTests, int numberOfPassedTests, int numberOfSkippedTests, String message)
{
    echo "Send Slack Notification"

    def script = this
	def email = powershell(returnStdout: true, script: "git --no-pager show -s --format='%ae'").trim()
	def channel = "notify-iapi-pipeline-" + "${slackChannel}"
    // Url are defined in the Import API slack application https://api.slack.com/apps/A028WEQG63F
    def url = "https://hooks.slack.com/services/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"
    switch(channel) {
        case "notify-iapi-pipeline-build":
            url = "https://hooks.slack.com/services/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"
        break;
        case "notify-iapi-pipeline-compatibility":
            url = "https://hooks.slack.com/services/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"
        break;
        case "notify-iapi-pipeline-load-tests":
            url = "https://hooks.slack.com/services/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"
        break;
        case "notify-iapi-pipeline-mass-import-verification":
            url = "https://hooks.slack.com/services/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"
        break;
        case "notify-iapi-pipeline-regression":
            url = "https://hooks.slack.com/services/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"
        break;
        case "notify-iapi-pipeline-release-branches":
            url = "https://hooks.slack.com/services/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"
        break;
    }

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
        "\n" + "buildUrl: " + env.BUILD_URL +
        "\n" + "numberOfFailedTests: " + numberOfFailedTests +
        "\n" + "numberOfPassedTests: " + numberOfPassedTests +
        "\n" + "numberOfSkippedTests: " + numberOfSkippedTests +
        "\n" + "message: " + message +
        "\n" +
        "\n*************************************************"
    try
    {
        echo 'Sending POST request to ' + url
        def post = new URL(url).openConnection();
        def body = """
                    {
	"blocks": [
		{
			"type": "section",
			"text": {
				"type": "mrkdwn",
				"text": "*Build result*\nImage: ${serverUnderTestName}\nBranch: ${branch}\nVersion: ${version}\nBuildType: ${buildType}\n Email: ${email}\nMessage: ${message}"
			}
		},
		{
			"type": "actions",
			"elements": [
				{
					"type": "button",
                    "url": "${env.BUILD_URL}",
					"value": "job",
					"action_id": "actionId-0",
					"text": {
						"type": "plain_text",
						"text": "Jenkins Job",
						"emoji": true
					},
					"style": "primary"
				}
			]
		},
		{
			"type": "divider"
		},
		{
			"type": "section",
			"text": {
				"type": "mrkdwn",
				"text": "Tests:"
			}
		},
		{
			"type": "actions",
			"elements": [
				{
					"type": "button",
                    "url": "${env.BUILD_URL}",
					"value": "passed",
					"action_id": "actionId-1",
					"text": {
						"type": "plain_text",
						"text": "Passed: ${numberOfPassedTests}",
						"emoji": true
					},
					"style": "primary"
				},
				{
					"type": "button",
                    "url": "${env.BUILD_URL}",
					"value": "failed",
					"action_id": "actionId-2",
					"text": {
						"type": "plain_text",
						"text": "Failed: ${numberOfFailedTests}",
						"emoji": true
					},
					"style": "danger"
				},
				{
					"type": "button",
                    "url": "${env.BUILD_URL}",
					"value": "skipped",
					"action_id": "actionId-3",
					"text": {
						"type": "plain_text",
						"text": "Skipped: ${numberOfSkippedTests}",
						"emoji": true
					}
				}
			]
		}
	]
}
                    """
        post.setRequestMethod("POST")
        post.setDoOutput(true)
        post.setRequestProperty("Content-Type", "application/json")
        post.getOutputStream().write(body.getBytes("UTF-8"));
        def postRC = post.getResponseCode();
        echo 'Response Code: ' + postRC
        
        if (postRC.equals(200)) {
            def response = post.getInputStream().getText();
            echo 'Response: ' + response
        }

    }
    catch(err)
    {
        echo "Send slack notification failed"
        echo err.toString()
    }
}

return this;