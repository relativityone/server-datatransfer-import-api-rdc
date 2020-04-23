<#
 .Synopsis
 Use the Resharper CLI tools and custom extensions to perform extended code analysis checks.

 .Parameter SolutionFile
 The solution file to run code analysis on.

 .Parameter CliDownloadUrl
 The Resharper CLI package download URL.

 .Parameter NuGetPackageUrl
 Download source of the ConfigureAwaitChecker nuget package
 
 .Parameter severity
 The severity level of the resharper issues to output. HINT|SUGGESTION|WARNING|ERROR
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True,Position=0)]
    [string]$SolutionFile,
    [Parameter(Mandatory=$False,Position=1)]
    [string]$CliDownloadUrl = "https://relativity.jfrog.io/relativity/api/nuget/nuget-anthology/Download/JetBrains.ReSharper.CommandLineTools/2019.3.1",
    [Parameter(Mandatory=$False,Position=2)]
    [string]$NuGetPackageUrl = "https://relativity.jfrog.io/relativity/nuget-local/ConfigureAwaitChecker.v9.0.15.0.nupkg",
    [Parameter(Mandatory=$False,Position=3)]
    [string]$Severity = "SUGGESTION"
)

$rootResharperDir = ".\buildtools\Resharper-CA"
$logFile = ".\Logs\AsyncAwait-Errors.xml"

if (-Not (Test-Path $rootResharperDir)) {
    New-Item -ItemType Directory -Path $rootResharperDir
}

if (Test-Path $logFile) {
    Remove-Item $logFile
}

$cliDownloadFile = "$rootResharperDir\Resharper-Cli.zip"
$cliExtractedtDir = "$rootResharperDir\Extracted"

if (-Not (Test-Path $cliDownloadFile)) {
    Write-Output "Downloading package $CliDownloadUrl to $cliDownloadFile..."
	[Net.ServicePointManager]::SecurityProtocol = ([Net.SecurityProtocolType]::Tls12)
    Invoke-WebRequest -Uri $CliDownloadUrl -OutFile $cliDownloadFile
}

# Unzip only if it's not already unzipped
if (-Not (Test-Path $cliExtractedtDir)) {
    Write-Output "Extracting ZIP package $cliDownloadFile to $cliExtractedtDir..."
    Expand-Archive $cliDownloadFile -DestinationPath $cliExtractedtDir
}

# Download only if we don't have it already
$nugetPackageFile = "$rootResharperDir\Extracted\tools\ConfigureAwaitChecker.v9.nupkg"
if (-Not (Test-Path $nugetPackageFile)) {
    Write-Output "Downloading package $NuGetPackageUrl to $nugetPackageFile..."
    try {
		[Net.ServicePointManager]::SecurityProtocol = ([Net.SecurityProtocolType]::Tls12)
        Invoke-WebRequest -Uri $NuGetPackageUrl -OutFile $nugetPackageFile
    }
    catch {
        $errorMessage = $_.Exception.Message
        Write-Error "Failed to download the $NuGetPackageUrl package. Error: $errorMessage"
        throw
    }
}

# Note: explicitly defining a temp cache to work around misses that occur due to the Resharper cache implementation.
$tempPath = [System.IO.Path]::GetTempPath()
$uniqueFolder = "RelativityTmpDir_" + [System.DateTime]::Now.Ticks + "_" + [System.Guid]::NewGuid()
$cacheHome = Join-Path $tempPath $uniqueFolder
New-Item -ItemType Directory -Path $cacheHome

try {
    Write-Output "Inspecting code to ensure async/await usage is correctly implemented..."
    & $cliExtractedtDir\tools\inspectcode.exe $SolutionFile --output="$logFile" --s="$Severity" --caches-home="$cacheHome"
}
finally {
    Remove-Item -Recurse -Force -Path $cacheHome
}

# Get errors
[xml]$xmlErrors = Get-Content -path $logFile

# Find issue types
If ($xmlErrors.Report.IssueTypes | Select-XML -XPath "//IssueType[@Id='ConsiderUsingConfigureAwait']" -ErrorAction Ignore) {
    Write-Error "The async/await code analysis check contains 1 or more violations.
Review $logFile for complete details."
    Write-Output ""
    Write-Output "Violation Summary"
    Try {
        Foreach($p in $xmlErrors.Report.Issues.ChildNodes) {
            Foreach($i in $p.ChildNodes) {
                if ($i.Attributes.GetNamedItem("TypeId").Value -eq "ConsiderUsingConfigureAwait") {
                    Write-Output ("{0}, Line: {1}" -f $i.Attributes.GetNamedItem("File").Value, $i.Attributes.GetNamedItem("Line").Value) -ErrorAction SilentlyContinue
                }
            }
        }
    } 
    Catch {
        $errorMessage = $_.Exception.Message
        Write-Error "Failed to retrieve violations from logfile. Error: $errorMessage"
    }

    exit 1;
} Else {
    Write-Output "The async/await code analysis check passed."
    exit 0;
}