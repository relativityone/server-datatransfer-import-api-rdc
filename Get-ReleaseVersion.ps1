param(
[string]$branchNameJenkins
)


function gitBranchName {
    $branchName = git rev-parse --abbrev-ref HEAD
    If($branchName -eq 'HEAD')
    {
        return $branchNameJenkins
    }
    else
    {
        return $branchName
    }
}



$gitVersion = git describe --tags --always
Write-Host $gitVersion
$gitVersionSplit = $gitVersion.ToString().Split('-')
$version = $gitVersionSplit[0]
$commitsSince = $gitVersionSplit[1]

Write-Host "Version = $version"
Write-Host "Commits since version was created = $commitsSince"
$major = $version.Split('.')[0]
$minor = $version.Split('.')[1]
if("$commitsSince" -eq "")
{
   $commitsSince = "0"
}
$currentBranch = gitBranchName
Write-Host "current branch is $currentBranch"
$postfix = ""
If ($currentBranch.ToString() -eq "develop" ) 
{
   $postfix = "-dev"
}
elseif ($currentBranch.ToString().StartsWith("bugfix/")) 
{
   $currentBranch = $currentBranch.Replace("bugfix/","") 
   $postfix = "-bugfix-$currentBranch"
}
elseif ($currentBranch.ToString().StartsWith("feature/")) 
{
	 $currentBranch = $currentBranch.Replace("feature/","") 
    $postfix = "-feature-$currentBranch"
}
elseif ($currentBranch.ToString().StartsWith("release-")) 
{
    if(-Not ($currentBranch.Contains($version)))
    {
	    $(Throw New-Object System.ArgumentException "current branch should contain the latest tag : currentbranch = $currentBranch, last tag = $version ", "tag not found")
    }
    $postfix = ""
}
else
{
	$(Throw New-Object System.ArgumentException "Branch must start with 'feature' or 'bugfix' (case sensitive), or be equal to 'develop', current branch is '$currentBranch'","branch name")
}



$newVersion = "$major.$minor.$commitsSince$postfix"
Write-Host "New version should be = $newVersion"
Write-Output $newVersion	



