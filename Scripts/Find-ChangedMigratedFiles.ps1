<#
.SYNOPSIS
Identify source file changes since a given date.

.DESCRIPTION
This script identifies relevant changes made to migration source code in the Relativity repo.

.EXAMPLE
.\Find-ChangedMigratedFiles.ps1 -SinceDate "today.0.00am"
Searches for changes made today.

.EXAMPLE
.\Find-ChangedMigratedFiles.ps1 -SinceDate "2019-03-01"
Searches for changes made since Match 1 2019.

.PARAMETER Branch
The branch to inspect.

.PARAMETER RepoPath
The full path to the Relativity repo.

.PARAMETER SinceDate
The date used as a search filter.
#>

#Requires -Version 5.0
#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [Parameter()]
    [String]$Branch = "develop",
    [Parameter()]
    [String]$RepoPath = "S:\SourceCode\Relativity\relativity",
    [Parameter()]
    [String]$SinceDate = "2019-03-01"
)

Push-Location -Path $RepoPath
$commits = (git log --since="$sinceDate" --branches=$Branch* --reverse --format=%h)
if ($commits.Length -gt 0)
{
    $files = & git diff --name-only $commit $Branch
    $patterns = @(
        "kCura/kCura/"
        "kCura/kCura.CommandLine/",
        "kCura/kCura.ImageValidator/",
        "kCura/kCura.OI.FileID/",
        "kCura/kCura.Utility/",
        "kCura/kCura.Utility.NUnit/",
        "kCura/kCura.Windows.Forms/",
        "kCura/kCura.Windows.Forms.NUnit/",
        "kCura/kCura.Windows.Process/",
        "EDDS/kCura.EDDS.WinForm/",
        "EDDS/kCura.EDDS.WinForm.NUnit.Automation/",
        "EDDS/kCura.Relativity.ImportAPI.IntegrationTests/",
        "EDDS/kCura.WinEDDS/",
        "EDDS/kCura.WinEDDS.Core/",
        "EDDS/kCura.WinEDDS.Core.NUnit/",
        "EDDS/kCura.WinEDDS.ImportExtension/",
        "EDDS/kCura.WinEDDS.ImportExtension.NUnit/",
        "EDDS/kCura.WinEDDS.NUnit/",
        "EDDS/kCura.WinEDDS.TApi/",
        "EDDS/kCura.WinEDDS.TApi.NUnit.Integration/",
        "EDDS/kCura.WinEDDS.UIControls/",
        "EDDS/kCura.Relativity.DataReader/",
        "EDDS/kCura.Relativity.ImportAPI/",
        "EDDS/kCura.Relativity.ImportAPI.Extension/",
        "EDDS/Relativity/",
        "EDDS/Relativity.Applications.Serialization"
    )

    $totalImpactedFiles = 0
    foreach ($pattern in $patterns) {
		# Filtering out package/project changes since these happen frequently via teams making mass package updates.
        $filteredFiles = $files | Select-String -SimpleMatch $pattern | Select-String -Pattern ".nupkg|packages.config|.csproj|.vbproj" -NotMatch
        if ($filteredFiles) {
            $totalImpactedFiles += $filteredFiles.Length 
            Write-Host "Found $($filteredFiles.Length) change(s) in $pattern"
            foreach ($filteredFile in $filteredFiles) {
                Write-Host $filteredFile
            }

            Write-Host ""
        }
    }

    Pop-Location
    Write-Host "Total impacted files: $totalImpactedFiles"
}