def SendSlackNotification(String serverUnderTestName, String version, String branch, def isPublishedBranch, def isReleaseBranch, String slackChannel, int numberOfFailedTests, int numberOfPassedTests, int numberOfSkippedTests, String message)
{
    echo "Send Slack Notification"

    def script = this
	def email = powershell(returnStdout: true, script: "git --no-pager show -s --format='%ae'").trim()
    // Url are defined in the Import API slack application https://api.slack.com/apps/A028WEQG63F
	//Name of the slack-app created: 'cicd-Import-API-RDC'
	//The new slack-app created url : https://kcura-pd.slack.com/apps/A057YAY6LK1-cicd-import-api-rdc?tab=more_info
	//Respective webhook url page: https://api.slack.com/apps/A057YAY6LK1/install-on-team?success=1
	//Webhook url: https://hooks.slack.com/services/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

    def url = "https://hooks.slack.com/services/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"

    echo "*************************************************" +
        "\n" +
        "\n" + "sendCDSlackNotification Parameters: " +
        "\n" +
        "\n" + "script: " + script +
        "\n" + "serverUnderTestName: " + serverUnderTestName +
        "\n" + "version: " + version +
        "\n" + "branch: " + branch +
        "\n" + "isPublishedBranch: " + isPublishedBranch +
        "\n" + "isReleaseBranch: " + isReleaseBranch +
        "\n" + "slackChannel: " + slackChannel +
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
				"text": "*Build result*\nImage: ${serverUnderTestName}\nBranch: ${branch}\nVersion: ${version}\nIsPublishedBranch: ${isPublishedBranch}\nIsReleaseBranch: ${isReleaseBranch}\n Email: ${email}\nMessage: ${message}"
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