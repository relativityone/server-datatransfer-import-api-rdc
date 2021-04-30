<#
 .Synopsis
 Checks is RDC prerequisites (C++ redistributables) are valid.
#>
$fileWithPrerequisitesDefinition = '.\Source\Relativity.Desktop.Client.Bundle\Prerequisites\VCpp.wxs'
Write-Host "Validating RDC prerequisites defined in $fileWithPrerequisitesDefinition"

[xml] $redistributablesConfig = Get-Content $fileWithPrerequisitesDefinition
$redistributables = $redistributablesConfig.Wix.Fragment.PackageGroup

$invalidPackages = @()
foreach($redistributable in $redistributables) {
	$packageId = $redistributable.ExePackage.Id
	$downloadUrl = $redistributable.ExePackage.DownloadUrl
	$expectedHash = $redistributable.ExePackage.RemotePayload.Hash
	
	Write-Host "Validating $packageId"
	
	$outFile = '.\' + $expectedHash
	
	Invoke-WebRequest -Uri $downloadUrl -OutFile $outFile
	$actualHash = Get-FileHash -Path $outFile -Algorithm SHA1 | Select-Object -ExpandProperty Hash
	Remove-Item $outFile
	$actualHashValue = 
	if($actualHash -ne $expectedHash)
	{
		$invalidPackages += $packageId
		Write-Host "Invalid hash for $packageId. Expected: '$expectedHash', Actual: '$actualHash'"
	} else {
		Write-Host  "$packageId has valid definition"
	}
}

if($invalidPackages.count -gt 0) {
	Throw ("RDC installer requires invalid prerequisites: " + $invalidPackages)
}