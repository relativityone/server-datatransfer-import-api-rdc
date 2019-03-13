﻿FormatTaskName "------- Executing Task: {0} -------"
Framework "4.6" #.NET framework version

properties {
    $LogsDir = Join-Path $Root "Logs"
    $MasterSolution = Join-Path $Root "Source/Relativity.ImportAPI-RDC.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $BuildArtifactsDir = Join-Path $Root "Artifacts"
    $BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
    $ScriptsDir = Join-Path $Root "Scripts"

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
    $TestVMName = $Null
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

task ExtendedCodeAnalysis -Description "Perform extended code analysis checks." {

    & "$ScriptsDir\Invoke-ExtendedCodeAnalysis.ps1" -SolutionFile $MasterSolution
}

task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

task TestVMSetup -Description "Setup the test parameters for TestVM" {
    try
    {
        $testVM = $null
        if ($TestVMName) {
            $testVM = (Get-Testvm) | Where-Object { $_.BoxName -eq $TestVMName } | Select-Object
            if (-Not $testVM) {
                Throw "This operation cannot be performed because the TestVM $TestVMName doesn't exist."
            }
        }
        else {
            $testVM = (Get-Testvm) | Select-Object -First 1
            if (-Not $testVM) {
                Throw "This operation cannot be performed because there must be at least 1 TestVM available."
            }
        }

	    $hostname = $testVM.BoxName 
	    If ((Get-Content (Join-Path $testVM.Directory box.json) | ConvertFrom-Json).parameters.joinDomain.value -eq 0) { 
		    $hostname = "$($testVM.BoxName).kcura.corp"
	    } 

        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYURL", "https://$hostname", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYRESTURL", "https://$hostname/relativity.rest/api", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYSERVICESURL", "https://$hostname/relativity.services", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYWEBAPIURL", "https://$hostname/relativitywebapi", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYUSERNAME", "relativity.admin@kcura.com", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYPASSWORD", "Test1234!", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPASPERAMODETESTS", "true", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPDIRECTMODETESTS", "false", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "false", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLDROPWORKSPACEDATABASE", "true", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLINSTANCENAME", "$hostname\\EDDSINSTANCE001", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLADMINUSERNAME", "sa", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLADMINPASSWORD", "P@ssw0rd@1", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_WORKSPACETEMPLATE", "Relativity Starter Template", "Process")
        Write-Host "The test environment is setup with the $hostname TestVM."
    }
    catch
    {
        $errorMessage = $_.Exception.Message
        Write-Error "Failed to setup the TestVM for integration tests. Error: $errorMessage"
        throw
    }
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
    exec { & $ScriptPath -Version $AssemblyVersion -VersionFolderPath $VersionPath }
}

task DigitallySignBinaries -Description "Digitally sign all binaries"   {
    $sites = @("http://timestamp.comodoca.com/authenticode",
               "http://timestamp.verisign.com/scripts/timstamp.dll",
               "http://tsa.starfieldtech.com")
    $signtool = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, "Microsoft SDKs", "Windows", "v7.1A", "Bin", "signtool.exe")
    $retryAttempts = 3
    Write-Output "Signing all assemblies in $BinariesArtifactsDir"
    $directoriesToSign = Get-ChildItem -Path $BinariesArtifactsDir
    $dllNamePrefix = "Relativity"
    foreach($directory in $directoriesToSign)
    {
        Write-Output "Signing assemblies in $directory"
        $filesToSign = Get-ChildItem -Path $directory.FullName -Recurse -Include @("*.dll","*.exe","*.msi") | Where-Object { $_.Name.StartsWith($dllNamePrefix) }
        foreach($fileToSign in $filesToSign)
        {
            & $signtool verify /pa /q $fileToSign
            $signed = $?

            if (-not $signed) {

                For ($i =0; $i -lt $retryAttempts; $i++) {
                    ForEach ($site in $sites){
                        Write-Host "Attempting to sign" $dll "using" $site "..."
                        & $signtool sign /a /t $site /d "Relativity" /du "http://www.kcura.com" $dll
                        $signed = $?                    
                        if ($signed) {
                            Write-Host "Signed" $dll "Successfully!"
                            break
                        }
                    }  
                    
                    if ($signed) {
                        break
                    }
                }
        
                if (-not $signed) {
                    Write-Error "Failed to sign the dlls. See the error above.";
                    exit 1
                }
            }
            else {
                Write-Host $dll "is already signed!"
            }
        }
    }
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