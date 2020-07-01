Import-Module -Name ".\DevelopmentScripts\branches.psm1" -Global

Function Update-AssemblyInfo {
    param(
    [string]$NewVersion
    )
    Set-Location .\Version

    Get-ChildItem -Include AssemblySharedInfo.cs, AssemblySharedInfo.vb -Recurse | 
        ForEach-Object {
            $_.IsReadOnly = $false
            (Get-Content -Path $_) -replace '(?<=Assembly(?:(File|Informational))?Version\(")[^"]*(?="\))', $NewVersion |
                Set-Content -Path $_
        }
    Set-Location ..
}

Function Get-RdcVersion {
    param(
        [string] $rdcVersionWixFile,
        [string] $branch
    )

	$majorMinorPatchVersion = Get-RdcWixVersion -rdcVersionWixFile $rdcVersionWixFile
    $postFix = Get-ReleaseVersion "$branch" -postFixOnly

    [BranchType]$typeOfBranch = branches\Get-CurrentBranchType "$branch"
    # Means its not a release branch
    if(-Not ($typeOfBranch -eq [BranchType]::HotfixRelease -or $typeOfBranch -eq [BranchType]::Release))
    {
        Write-Host "PostFix: $postFix"
        $commitsSince = Get-ReleaseVersion "$branch" -returnCommitsSinceOnly
        Write-Host "commitsSince: $commitsSince"
        $postFix = ".$commitsSince$postFix"
        Write-Host "PostFix: $postFix"
    }
    $packageVersion = "$majorMinorPatchVersion$postFix"
	return $packageVersion
}

Function Get-RdcWixVersion {
    param(
        [string] $rdcVersionWixFile
    )
    
    if (-Not (Test-Path $rdcVersionWixFile -PathType Leaf)) {
        Throw "The RDC version cannot be determined because the WIX RCD version source file '$rdcVersionWixFile' doesn't exist."
    }

    [xml]$xml = Get-Content $rdcVersionWixFile
    $selector = "//processing-instruction('define')[starts-with(., 'ProductVersion')]"
    $node = $xml.SelectSingleNode($selector)
    if (!$node) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' doesn't define the expected ProductVersion variable."
    }

    $options = [Text.RegularExpressions.RegexOptions]::IgnoreCase
    $regexMatch = [regex]::Match($node.InnerText, "ProductVersion[ ]*=[ ]*""(?<value>.*)\""", $options)
    if (!$regexMatch.Success) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the regular expression match failed."
    }

    if (!$regexMatch.Groups["value"]) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the value cannot be determined from the regular expresssion match."
    }

    [string]$productVersion = $regexMatch.Groups["value"].Value
    if (!$productVersion -or $productVersion.Length -le 0) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the version is empty."
    }

    return $productVersion.Trim()
}

Function Get-ReleaseVersion {
    param(
        [string]$branchNameJenkins,
        [switch]$postFixOnly = $false,
        [switch]$omitPostFix = $false,
        [switch]$returnCommitsSinceOnly = $false
    )
	Write-Host $branchNameJenkins
    $host.UI.RawUI.WindowTitle = "Getting release version"

    function gitBranchName {
        $branchName = git rev-parse --abbrev-ref HEAD
        If($branchName -eq 'HEAD')
        {
            Write-Host "The branchname is not given, it is currently HEAD (meaning the code is checked out at a commit, not at a branch)"
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
    $version = $gitVersionSplit[0] # 1.9.0
    $commitsSinceLastTag = $gitVersionSplit[1] # 95
    # git describe does not give the commits since tag if the numer of commits since tag is null.
    if("$commitsSinceLastTag" -eq "")
    {
        $commitsSinceLastTag = "0"
    }
	$commitsSince = [int]$commitsSinceLastTag + [int]$version.Split('.')[2]
	
    Write-Host "Version = $version"
    $major = $version.Split('.')[0] # 1
    $minor = $version.Split('.')[1] # 9
    

    Write-Host "Commits since version was created = $commitsSince"
    if($returnCommitsSinceOnly)
    {
        return $commitsSince
    }
    $currentBranch = gitBranchName
    Write-Host "Current branch is $currentBranch"
    
    [BranchType]$typeOfBranch = branches\Get-CurrentBranchType "$currentBranch"
    if (($typeOfBranch -eq [BranchType]::Release) -or ($typeOfBranch -eq [BranchType]::HotfixRelease)) {
        if(-Not ($currentBranch.Contains("$major.$minor")))
        {
            $(Throw New-Object System.ArgumentException "Current branch should contain the latest tag : currentbranch = $currentBranch, last tag = $version, string to find = $major.$minor", "tag not found")
        }
    }
    
    Write-Host "Type of branch = $typeOfBranch"
    
    # Different branches get different postfixes
    switch ($typeOfBranch) {
        ([BranchType]::Develop) {$postfix = "-dev"}
        ([BranchType]::FeatureBranch) {$postfix = "-$currentBranch"}
        ([BranchType]::Release) {$postfix = ""}
        ([BranchType]::HotfixRelease) {
            $numbersAtTheEnd = $currentBranch | Foreach {if ($_ -match '(\d+)$') {$matches[1]}}
            $postfix = "-Hotfix-$numbersAtTheEnd"
        }
        ([BranchType]::Trident) {$postfix = "-testing"}
        ([BranchType]::PerformancePipeline) {$postfix = "-performance"}
		([BranchType]::ComplexCases) {$postfix = "-testing"}
        default { Throw "Branch type is unknown" }
    }
    
    #Escape as version numbers should not contain anything special, like underscores. Dashes are fine tough
    $pattern = '[^a-zA-Z0-9]'
    $postfix = $postfix -replace $pattern ,"-"

    If($omitPostFix)
    {
        $majorMinorCommits = "$major.$minor.$commitsSince"
        Write-Host "MajorMinorCommitsSince = $majorMinorCommits"
        Write-Output $majorMinorCommits    
    }
    elseif($postFixOnly)
    {
        $newVersion = "$postfix"
        Write-Host "Postfix = $newVersion"
        Write-Output $newVersion
    }    
    else
    {
        $newVersion = "$major.$minor.$commitsSince$postfix"
        Write-Host "New complete version should be = $newVersion"
        Write-Output $newVersion
    }
}