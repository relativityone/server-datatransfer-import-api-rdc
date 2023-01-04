<#
 .Synopsis
 Checks if the references in \.paket\Relativity.DataExchange.Client.SDK.nuspec can be found in \paket.dependencies.

 .Parameter SolutionDir
 The directory of the solution.
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True,Position=0)]
    [string]$SolutionDir
)

$dict = New-Object 'system.collections.generic.dictionary[string,string]'

#fetch versions in dataexchange.client.sdk
[xml] $nuspec = Get-Content '.\.paket\Relativity.DataExchange.Client.SDK.nuspec'
$dependencies = $nuspec.package.metadata.dependencies.group.dependency
foreach($dependency in $dependencies) {
    $toAdd = ($dependency.id + ": " + $dependency.version + " <br />")
    $version = $dependency.version
    Write-Host $dependency.id $version
    $versionRangeMatch = Select-String '^\[(.*),.*\)$' -inputobject $version
    if($versionRangeMatch) {
        Write-Host "Found version range for:" $dependency.id "validating version:" $version
        $version = $versionRangeMatch.Matches.Groups[1].value
    }
    $dict.Add($dependency.id, $version)
}

#Look at paket.dependencies and find all references that where in dataexchange.client.sdk
$target = Get-Content ($SolutionDir + '\paket.dependencies')

foreach($packageAndVesion in $dict.GetEnumerator()) {
   Write-Host ("looking for " + $packageAndVesion.Key + " " + $packageAndVesion.Value.ToString())
   $found = $false
   foreach($line in $target) {
        if(-not $line.StartsWith("nuget"))
        {
            continue
        }
        $lineContainsExpectedName = $line -like ("* " + $packageAndVesion.Key.ToString() + " *")
        $lineContainsExpectedVersion = $line -like ("*" + $packageAndVesion.Value.ToString() + "*")
        if($lineContainsExpectedName -and $lineContainsExpectedVersion)
        {
            $found = $true
            Write-Host ("found in :" + $line)
            continue
        }
        if($lineContainsExpectedName -and -not $lineContainsExpectedVersion)
        {
            Throw ("Can't find " + $packageAndVesion.Key.ToString() + " " + $packageAndVesion.Value.ToString() + " in file .\paket.dependencies, closest match is " + $line + " You see this message because .\.paket\paket.template.relativity.dataexchange.client.sdk has an reference to this nuget file.")
        }
    }
    if(-not $found)
    {
        Throw ("Can't find " + $packageAndVesion.Key.ToString() + " " + $packageAndVesion.Value.ToString() + " in file, no close matches either, You see this message because .\.paket\paket.template.relativity.dataexchange.client.sdk has an reference to this nuget file." )
    }
}
Write-Host "All dependencies are found and up to date!" -ForegroundColor Green