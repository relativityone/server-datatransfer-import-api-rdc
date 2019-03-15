FormatTaskName "------- Executing Task: {0} -------"
Framework "4.6" #.NET framework version

properties {
    $LogsDir = Join-Path $Root "Logs"
    $PackagesDir = Join-Path $Root "packages"
    $MasterSolution = Join-Path $Root "Source/Relativity.ImportAPI-RDC.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $BuildArtifactsDir = Join-Path $Root "Artifacts"
    $BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
    $ScriptsDir = Join-Path $Root "Scripts"
    $BuildPackagesDir = "\\bld-pkgs\Packages\Import-Api-RDC\"
    $TestResultsDir = Join-Path $Root "TestResults"
    $ExtentCliExe  = Join-Path $PackagesDir "extent\tools\extent.exe"
    $GitVersionExe = Join-Path $PackagesDir "GitVersion.CommandLine\tools\GitVersion.exe"

    # Properties below this line are defined in build.ps1
    $Target = $Null
    $Configuration = $Null
    $BuildPlatform = $Null
    $BuildUrl = $Null
    $Version = $Null
    $Branch = $Null
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

task CompileMasterSolution -Description "Compile the solution" {

    if (!$BuildPlatform) {
        $BuildPlatform = "Any CPU"
    }

    Write-Output "Solution: $MasterSolution"
    Write-Output "Configuration: $Configuration"
    Write-Output "Build platform: $BuildPlatform"
    Write-Output "Verbosity: $Verbosity"

    $LogFilePath = Join-Path $LogsDir "buildsummary.log"
    $ErrorFilePath = Join-Path $LogsDir "builderrors.log"
    $MSBuildTarget = $Target
    if ($Target -eq "clean") {
        Initialize-Folder $LogsDir
        Write-Verbose "Running Clean target on $MasterSolution"
        exec { msbuild $MasterSolution `
            "/t:$MSBuildTarget" `
            "/verbosity:$Verbosity" `
            "/p:Configuration=$Configuration" `
            "/nologo" `
        }
    }
    else {        
        Initialize-Folder $LogsDir -Safe
        exec { msbuild $MasterSolution `
            "/t:$MSBuildTarget" `
            "/verbosity:$Verbosity" `
            "/p:Platform=$BuildPlatform" `
            "/p:Configuration=$Configuration" `
            "/clp:Summary"`
            "/nodeReuse:false" `
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
    Initialize-Folder $TestResultsDir
    $TestResultsXmlFile = Join-Path $TestResultsDir "Test_Results.xml"
    $OutputFile = Join-Path $TestResultsDir "Test_Output.txt"
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
        "--result=$TestResultsXmlFile" `
        "--out=$OutputFile" `
        $testCategoryFilter `
    }

    # This will generate index.html within the test results directory.
    exec { & $ExtentCliExe -i $TestResultsXmlFile -o $TestResultsDir -r v3html } -errorMessage "There was an error generating the test report."
}

task UpdateAssemblyInfo -Precondition { $Version -ne "1.0.0.0" } -Description "Update the AssemblyInfo files in \Version\" {
    $VersionPath = Join-Path $Root "Version"
    $ScriptPath = Join-Path $VersionPath "Update-AssemblyInfo.ps1"
    exec { & $ScriptPath -Version $Version -VersionFolderPath $VersionPath }
}

task DigitallySign -Description "Digitally sign all binaries"   {
    $sites = @("http://timestamp.comodoca.com/authenticode",
               "http://timestamp.verisign.com/scripts/timstamp.dll",
               "http://tsa.starfieldtech.com")
    $signtool = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, "Microsoft SDKs", "Windows", "v7.1A", "Bin", "signtool.exe")
    $retryAttempts = 3
    $directoriesToSign = New-Object System.Collections.ArrayList($null)
    $directoriesToSign.Add((Join-Path $BinariesArtifactsDir "Relativity.Import.Client"))
    $dllNamePrefix = "Relativity"
    foreach($directory in $directoriesToSign)
    {
        Write-Output "Signing assemblies in $directory"
        $filesToSign = Get-ChildItem -Path $directory | Where-Object { $_.Name.StartsWith($dllNamePrefix) }
        if ($filesToSign.Length -eq 0)
        {
            Throw "The $directory contains zero files to sign. Verify the build script and project files are in agreement."
        }

        foreach($fileToSign in $filesToSign)
        {
            if (-not ($fileToSign.Extension -eq ".dll") -and
                -not ($fileToSign.Extension -eq ".exe") -and
                -not ($fileToSign.Extension -eq ".msi")) {
                continue;
            }

            $file = $fileToSign.FullName
            & $signtool verify /pa /q $file
            $signed = $?

            if (-not $signed) {

                For ($i =0; $i -lt $retryAttempts; $i++) {
                    ForEach ($site in $sites){
                        Write-Host "Attempting to sign" $file "using" $site "..."
                        & $signtool sign /a /t $site /d "Relativity" /du "http://www.kcura.com" $file
                        $signed = $?                    
                        if ($signed) {
                            Write-Host "Signed" $file "Successfully!"
                            break
                        }
                    }  
                    
                    if ($signed) {
                        break
                    }
                }
        
                if (-not $signed) {
                    Throw "Failed to sign the dlls. See the error above."
                }
            }
            else {
                Write-Host $file "is already signed!"
            }
        }
    }
}

task PublishBuildArtifacts -Description "Publish build artifacts"  {
    Assert ($Branch -ne "") "Branch is a required argument for saving build artifacts."
    Assert ($Version -ne "") "Version is a required argument for saving build artifacts."    
    $targetDir = "$BuildPackagesDir\$Branch\$Version"
    Copy-Folder -SourceDir $LogsDir -TargetDir "$targetDir\logs"
    Copy-Folder -SourceDir $BinariesArtifactsDir -TargetDir "$targetDir\binaries"
    Copy-Folder -SourceDir $TestResultsDir -TargetDir "$targetDir\test-results"
}

task GitVersion -Depends BuildVersion, PackageVersion -Description "Retrieves incremented build and package version from GitVersion" {
    Assert ($BuildUrl -ne $null -and $BuildUrl -ne "") "BuildUrl must be provided"
}

task BuildVersion -Description "Retrieves build version from GitVersion" {
    Write-Output "Importing GitVersion properties.."

    $buildVersionMajor = & $GitVersionExe /output json /showvariable Major
    $buildVersionMinor = & $GitVersionExe /output json /showvariable Minor
    $buildVersionPatch = & $GitVersionExe /output json /showvariable Patch
    $buildVersionCommitNumber = & $GitVersionExe /output json /showvariable CommitsSinceVersionSource

    Write-Output "Build Url: $BuildUrl"
    Write-Output "Version major: $buildVersionMajor"
    Write-Output "Version minor: $buildVersionMinor"
    Write-Output "Version patch: $buildVersionPatch"
    Write-Output "Version commits number: $buildVersionCommitNumber"

    $version = "$buildVersionMajor.$buildVersionMinor.$buildVersionPatch.$buildVersionCommitNumber"
    $global:BuildVersion = $version

    # So Jenkins can get the version number
    Write-Output "buildVersion=$version"
}

task PackageVersion -Description "Retrieves package version from GitVersion" {	
    $version = & $GitVersionExe /output json /showvariable NuGetVersion
    $global:PackageVersion = $version

    # So Jenkins can get the package version number
    Write-Output "packageVersion=$version"
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
        Write-Host "The directory '$Path' already exists."
        Return
    }

    if (Test-Path $Path)
    {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
        Write-Host "Deleted the '$Path' directory."
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop | Out-Null
    Write-Host "Created the '$Path' directory."
}

Function Copy-Folder {
    param(
        [String] $SourceDir,
        [String] $TargetDir
    )

    $robocopy = "robocopy.exe"
    Write-Output "Copying the build artifacts from $SourceDir to $TargetDir"
    & $robocopy "$SourceDir" "$TargetDir" /MIR /is
    if ($LASTEXITCODE -ne 1) 
    {
	    Throw "An error occured while copying the build artifacts from $SourceDir to $TargetDir"
    }
}