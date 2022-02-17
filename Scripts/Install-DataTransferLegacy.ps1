<#
.DESCRIPTION
  Downloads latest version of the DataTransfer.Legacy app and installs it in the application library.

.PARAMETER RelativityHost
  Url of the Relativity instance.
.PARAMETER RelativityUsername
  Username.
.PARAMETER RelativityPassword
  Password.  
.PARAMETER NugetExe
  Filepath to the nuget exe.
#>


param(
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$RelativityHost,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$RelativityUsername,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$RelativityPassword,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
	[string]$NugetExe
)

Write-Host "RelativityHost: $RelativityHost"
Write-Host "NugetExe: $NugetExe"

[securestring]$securePassword = ConvertTo-SecureString -String $RelativityPassword -AsPlainText -Force
[pscredential]$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $RelativityUsername, $securePassword

try{
	exec { 
		& $NugetExe install DataTransfer.Legacy -ExcludeVersion -OutputDirectory "$PSScriptRoot"
	} -errorMessage "Failed to download latest DataTransfer.Legacy app."

	$rapPath = "$PSScriptRoot\DataTransfer.Legacy\lib\DataTransfer.Legacy.rap"

	if(-not (Get-Module -ListAvailable -Name RAPTools))
	{
		Install-Module RAPTools -Force
	}
	Import-Module RAPTools -Force
	Install-RelativityLibraryApplication -HostName "$RelativityHost" -FilePath "$rapPath" -RelativityCredential $credential
	Get-Module -ListAvailable -Name R1Ops.Relativity
	if(-not (Get-Module -ListAvailable -Name R1Ops.Relativity))
	{
		Install-Module R1Ops.Relativity -MaximumVersion 1.4.4 -Force
	}
	Import-Module R1Ops.Relativity -MaximumVersion 1.4.4 -Force 
	Set-R1OpsInstanceSetting -Section 'DataTransfer.Legacy' -Name 'IAPICommunicationMode' -Value 'Kepler' -ValueType 'Text' -Uri "https://$RelativityHost" -Credential $credential
}
finally{
	Remove-Item -Path "$PSScriptRoot\DataTransfer.Legacy\" -Recurse
}