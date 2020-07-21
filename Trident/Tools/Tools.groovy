def sendEmailAboutFailureToAuthor()
{
    def commiterDetails = bat (
        script: 'git --no-pager show -s --format=%ae', 
        returnStdout: true
    )

    def recipients = extractCommiterEmail(commiterDetails)
    sendEmailAboutFailure(recipients)
}

def sendEmailAboutFailureToTeam()
{
   def recipients = 'thegoodthebadandtheugly@relativity.com'
   sendEmailAboutFailure(recipients)
}

def sendEmailAboutFailure(String recipients)
{
    echo "Sending ${branchName} build failure to $recipients"
    def subject = "${env.JOB_NAME} - Build ${env.BUILD_DISPLAY_NAME} - Failed! On branch ${branchName}"
    def body = """${env.JOB_NAME} - Build - Failed:

	Check console output at ${env.BUILD_URL} to view the results."""
    sendEmail(body, subject, recipients)
}

def sendEmail(String body, String subject, String recipients)
{
    emailext attachLog: true, attachmentsPattern: 'TestResults/**/*.*', body: body, subject: subject, to: recipients
}

def extractCommiterEmail(details) {
    
    def arr = details.tokenize('\n')
    def email = arr[2].trim()
    return email
}

def extractValue(String value, String output)
{
    if (value == null || value.isEmpty())
    {
        return ""
    }

    def matcher = output =~ "$value=(.*)"
    $result =  matcher[0][0].split("=")[1]
    matcher = null
    return $result
}

def runCommandWithOutput(String command)
{
    def outputString = powershell(returnStdout: true, script: command).trim()
    if (!outputString) 
    {
        error("$command command returned empty output!")
    }

    return outputString
}

def tagGitCommit(String commitHash, String tag, String username, String password) {
    try {
        powershell """
            \$GitServer = "https://git.kcura.com"
            \$Headers = @{"Authorization" = "Basic \$([System.Convert]::ToBase64String(([System.Text.Encoding]::UTF8.GetBytes(""$username:$password""))))"}
            \$URI = "\$GitServer/rest/api/1.0/projects/DTX/repos/import-api-rdc/tags"
            Invoke-RestMethod -Method POST -URI \$URI -Headers \$Headers -ContentType "application/json" -Body (@{"name" = "$tag"; "startPoint" = "$commitHash"} | ConvertTo-Json)
            if(!\$?) {throw "An error ocurred while tagging git. Please check the logs."}
        """
    } catch (InterruptedException err) {
        error (err.toString())
    } catch (err) {
        error (err.toString())
    }

}

def createHopperInstance(String sutTemplate, String relativityBranch)
{
    def vmInfo = null
    String productName = utils.retrieveProductNameFromGitURL(scm.getUserRemoteConfigs()[0].getUrl())
    String buildOwner = ""
	String vmName = "$productName-${UUID.randomUUID().toString()}"
        
	buildOwner = utils.getBuildOwner()

	String vmDescription = "$productName - ${env.BRANCH_NAME} - ${currentBuild.displayName}"
	vmInfo = utils.createHopper("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", sutTemplate, vmName, vmDescription, buildOwner, productName)
	utils.createHopperTestSettings(vmInfo)
	// The hopper we created self destructs in a certain time period. This increases that timer by a bit. 
	// Therefore we call this function twice to make sure the hopper outlasts testing.
	utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)
	utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)

	
	if(relativityBranch != null && !relativityBranch.isEmpty()) 
	{
		try{
			stage('Install Relativity') {
				utils.getRelativityInstaller(relativityBranch)
				utils.deployRelativityToHopper(vmInfo, vmName)
				utils.waitForRelativityWorkspaceUpgrade(vmInfo)

			}
			utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)
			utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)
			utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)

		}
		catch(ex)
		{
			echo ex.toString()
		}
	}
	
    return vmInfo
}

def deleteHopperInstance(Integer vmId)
{
	try
	{
		echo "Deleting the hopper instance for ${vmId}"
		utils.removeHopper("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmId)								
	}
	catch (err)
	{
		echo err.toString()
	}
}

return this;