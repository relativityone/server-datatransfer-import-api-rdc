Add-Type -TypeDefinition @'
public enum BranchType {
    Release,
    HotfixRelease,
    Develop,
    FeatureBranch,
    Trident,
	PerformancePipeline,
	ReleaseBranches,
    PipelineTest,
	Master
}
'@ -Language CSharp

Function Get-CurrentBranchType{
    param(
        [string]$currentBranch
    )
    
    $jiraVersionNumber = Get-JiraTicketNumberFromBranchName -branchName $currentBranch
    If ($currentBranch.ToString() -eq "master" )
    {
       return [BranchType]::Master
    }  
    If ($currentBranch.ToString() -eq "develop" ) 
    {
       return [BranchType]::Develop
    }  
    If ($currentBranch.ToString() -eq "Performance-pipeline" ) 
    {
       return [BranchType]::PerformancePipeline
    } 
	elseif ($currentBranch.ToString() -eq "TestReleaseBranch")
	{
        return [BranchType]::ReleaseBranches
	}
    elseif ($currentBranch.ToString().StartsWith("release-")) 
    {
        if($currentBranch -like "*hotfix*")
        {
             return [BranchType]::HotfixRelease
        }
        return [BranchType]::Release
    }
	elseif ($currentBranch.ToString().StartsWith("pipeline-test-")) 
    {
        return [BranchType]::PipelineTest
    }
    elseif (![string]::IsNullOrEmpty($jiraVersionNumber)) 
    {
        if(!$currentBranch.StartsWith($jiraVersionNumber))
        {
            throw "Branch should start with the jira version number, detected jira version number = '$jiraVersionNumber', and branch = '$Branch'"
        }
        return [BranchType]::FeatureBranch
    }
    elseif ($currentBranch.ToString() -eq "Trident" ) 
    {
        return [BranchType]::Trident
    }
    else
    {
        $(Throw New-Object System.ArgumentException "The branch name is not 'develop', or starts with 'release-' or starts with a Jira number (like 'REL-123456'). The branchname supplied is '$currentBranch'", "branch name")
    }
}

Function Get-JiraTicketNumberFromBranchName {
    param(
        [String] $branchName
    )
    # Remove the REL number to reduce the version length.
    $options = [Text.RegularExpressions.RegexOptions]::IgnoreCase
    $regexMatch = [regex]::Match($branchName, "(?<jira>REL-\d+).*", $options)
    if (!$regexMatch.Success) {
        return $null
    }

    if (!$regexMatch.Groups["jira"]) {
        return $null
    }

    $jiraTicketNumber = $regexMatch.Groups["jira"].Value
    if ($jiraTicketNumber -and $jiraTicketNumber.Length -gt 0) {
        return $jiraTicketNumber
    }

    return $null
}

Export-ModuleMember -Function Get-CurrentBranchType