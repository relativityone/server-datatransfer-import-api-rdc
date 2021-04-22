def sendEmailAboutFailureToAuthor(String branchName)
{
    def commiterDetails = bat (
        script: 'git --no-pager show -s --format=%%ae', 
        returnStdout: true
    )

    def recipients = extractCommiterEmail(commiterDetails)
    sendEmailAboutFailure(recipients, branchName)
}

def sendEmailAboutFailureToTeam(String branchName)
{
   def recipients = 'thegoodthebadandtheugly@relativity.com'
   sendEmailAboutFailure(recipients, branchName)
}

def sendEmailAboutFailure(String recipients, String branchName)
{
    echo "Sending ${branchName} build failure to ${recipients}"
    def subject = "[GBU-Pipeline] ${env.JOB_NAME} - Build ${env.BUILD_DISPLAY_NAME} - Failed! On branch ${branchName}"
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
				\$serverHasTag = \$(git tag -l "$tag")
				if(\$serverHasTag)
				{
					\$variableAsJson = ConvertTo-Json -\$serverHasTag
					Write-Host "$tag already exists. \$variableAsJson"
				}
				else
				{
					Write-Host "$tag does not exist."
					\$GitServer = "https://git.kcura.com"
					\$Headers = @{"Authorization" = "Basic \$([System.Convert]::ToBase64String(([System.Text.Encoding]::UTF8.GetBytes(""$username:$password""))))"}
					\$URI = "\$GitServer/rest/api/1.0/projects/DTX/repos/import-api-rdc/tags"
					Invoke-RestMethod -Method POST -URI \$URI -Headers \$Headers -ContentType "application/json" -Body (@{"name" = "$tag"; "startPoint" = "$commitHash"} | ConvertTo-Json)
					if(!\$?) {throw "An error ocurred while tagging git. Please check the logs."}
					Write-Host "$tag was added to the remote repository."
				}
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
    
    echo "Build owner: ${buildOwner}."

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
	
	// workaround for REL-469686 - First request to Relativity.Distributed/Download.aspx fails due to FileLoadException
	echo "Sending a request to the Relativity.Distributed/Download.aspx - workaround for REL-469686"
	def relativityUrl = vmInfo.Url // format: http://xxx.yyy/Relativity/
	def ditributedDownloadUrl = relativityUrl.replaceAll("Relativity/", "Relativity.Distributed/Download.aspx")
	echo "Distributed download URL: ${ditributedDownloadUrl}"
	def httpConnection = new URL(ditributedDownloadUrl).openConnection()
	def responseCode = httpConnection.getResponseCode()
	echo "Relativity.Distributed/Download.aspx returned ${responseCode} response code."
	// end of workaround for REL-469686
	
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

def transferHopper(Map vmInfo)
{
	try
	{
		echo "Transferring Hopper instance to ${utils.getBuildOwner()}."
		utils.transferHopper("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id, utils.getBuildOwner())
		utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)
	}
	catch (err)
	{
		echo err.toString()
	}
}

def renewHopperInstanceLease(Map vmInfo)
{
    try
    {
        echo "Renewing Hopper Instance lease."
        utils.renewInstanceLease("https://api.hopper.relativity.com/", "homeimprovement@relativity.com", vmInfo.Id)
    }
 	catch (err)
	{
		echo err.toString()
	}   
}
return this;