<#
 .Synopsis
 Checks if the references in \.paket\paket.template.relativity.dataexchange.client.sdk can be found in \paket.dependencies.

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
$seenLineWithDependencies = $False

foreach($line in Get-Content ($SolutionDir + '\.paket\paket.template.relativity.dataexchange.client.sdk')) {

    if($line -like "*framework: *")
    {
        $seenLineWithDependencies = $True
        continue
    }
    if(($line -like "*frameworkAssemblies*")){
       break
    }
    if(-not $seenLineWithDependencies)
    {
        continue
    }
    Write-Host $line
    $lineElements = $line.split(($Null) -as [string],[System.StringSplitOptions]::RemoveEmptyEntries)
    
    Write-Host $lineElements[0]
    Write-Host $lineElements[2]

    $dict.Add($lineElements[0],$lineElements[2])
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