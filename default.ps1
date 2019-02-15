FormatTaskName "------- Executing Task: {0} -------"

properties {
    $LogsDir = Join-Path $Root "Logs"    
    $MasterSolution = Join-Path $root "Source/Relativity.ImportAPI-RDC.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value

    # Properties below this line are defined in build.ps1
    $Target = $Null
    $Configuration = $Null
    $AssemblyVersion = $Null
    $Verbosity = $Null
    $TestTimeoutInMS = $Null
    $UnitTests = $Null
    $IntegrationTests = $Null
    $SkipBuild = $Null
    $TestParametersFile = $Null
}

task Build -Description "Builds the source code" -Depends UpdateAssemblyInfo, CompileMasterSolution -Precondition { -not $SkipBuild } {
}

task CompileMasterSolution {
    $LogFilePath = Join-Path $LogsDir "buildsummary.log"
    $ErrorFilePath = Join-Path $LogsDir "builderrors.log"
    $MSBuildTarget = $Target
    if ($Target -eq "clean") {
        Initialize-Folder $LogsDir
        Write-Verbose "Running Clean target on $MasterSolution"
        exec { msbuild $MasterSolution `
            "/t:$MSBuildTarget" `
            "/verbosity:$Verbosity" `
            "/property:Configuration=$Configuration" `
            "/nologo" `
        }
    }
    else {        
        Initialize-Folder $LogsDir -Safe
        exec { msbuild $MasterSolution `
            "/t:$MSBuildTarget" `
            "/verbosity:$Verbosity" `
            "/property:Configuration=$Configuration" `
            "/property:PublishWebProjects=True" `
            "/clp:Summary"`
            "/nodeReuse:False" `
            "/nologo" `
            "/maxcpucount" `
            "/flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity" `
            "/flp2:errorsonly;LogFile=`"$ErrorFilePath`""
        }
    }
}

task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

task Test -Description "Run NUnit on Master solution" {
    $NUnit3 = Join-Path $PackagesDir "NUnit.ConsoleRunner\tools\nunit3-console.exe"
    $ResultsFile = Join-Path $LogsDir "Test_Results.xml"
    $OutputFile = Join-Path $LogsDir "Test_Output.txt"
    Initialize-Folder $LogsDir -Safe    
    if (Test-Path $ResultsFile)
    {
        Remove-Item $ResultsFile
    }

    if (Test-Path $OutputFile)
    {
        Remove-Item $OutputFile
    }
    
    $testCategoryFilter = $Null
    if ($IntegrationTests -and -not $UnitTests) {
        $testCategoryFilter = "--where=`"cat==Integration`""
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "false", "Process")    
    }
    elseif ($UnitTests -and -not $IntegrationTests) {
        $testCategoryFilter = "--where=`"cat!=Integration`""
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "true", "Process")
    }

    if ($TestParametersFile) {
        if (-Not (Test-Path $TestParametersFile -PathType Leaf)) {
            Throw "The test parameters file '$TestParametersFile' was specified but doesn't exist."
        }

        $json = Get-Content -Raw -Path $TestParametersFile | ConvertFrom-Json
        foreach ($property in $json.PSObject.Properties) {
            $name = $property.Name
            $value = $property.Value

            # Ensure the parameters are in env var format.
            if (-Not ($name.StartsWith("IAPI_INTEGRATION_"))) {
                $name = "IAPI_INTEGRATION_" + $name.ToUpper()
            }

            [Environment]::SetEnvironmentVariable($name, $value , "Process")
        }
    }

    exec { & $NUnit3 $MasterSolution `
        "--labels=All" `
        "--domain=Multiple" `
        "--process=Multiple" `
        "--agents=$NumberOfProcessors" `
        "--skipnontestassemblies" `
        "--timeout=$TestTimeoutInMS" `
        "--result=$ResultsFile" `
        "--out=$OutputFile" `
        $testCategoryFilter `
     }
}

task UpdateAssemblyInfo -Precondition { $AssemblyVersion -ne "1.0.0.0" } -Description "Update the AssemblyInfo files in \Version\" {
    $VersionPath = Join-Path $Root "Version"
    $ScriptPath = Join-Path $VersionPath "Update-AssemblyInfo.ps1"
    exec { & $ScriptPath -assembly_version $AssemblyVersion -path_to_version_folder $VersionPath }
}

Function Initialize-Folder {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [String] $Path,
        [Parameter()]
        [switch] $Safe
    )

    if ((Test-Path $Path) -and $Safe)
    {
        Return
    }

    if (Test-Path $Path)
    {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop -Verbose:$VerbosePreference
}