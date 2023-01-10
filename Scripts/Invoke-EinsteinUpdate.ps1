<#
.DESCRIPTION
  Invoke-EinsteinUpdate will create a new einstein page for the software that was build.

.PARAMETER Secret
  The api-key for Einstein.

.PARAMETER SdkVersion
  The SDK version of the software to create a page for.

.PARAMETER RdcVersion
  The RDC version of the software to create a page for.
  
.PARAMETER Branch
  The branch name as in GIT.
  
.PARAMETER BuildPackagesDir
  Filepath to the package directory for develop builds.
  
.PARAMETER BuildPackagesDirGold
  Filepath to the package directory for gold builds.
  
.PARAMETER PathToLocalRdcExe
  Filepath to the local RDC installer exe.
#>


param(
	[Parameter(Mandatory)]
    [object]$Secret,

	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$SdkVersion,
	
    [Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$RdcVersion,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$Branch,
		
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$BuildPackagesDir,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$BuildPackagesDirGold,
	
	[Parameter(Mandatory)]
	[ValidateNotNullOrEmpty()]
    [string]$PathToLocalRdcExe
)
Function Get-DependencyList{
	$results = ""
	#fetch versions in dataexchange.client.sdk
	$seenLineWithDependencies = $False
	[xml] $nuspec = Get-Content '.\.paket\Relativity.DataExchange.Client.SDK.nuspec'
	$dependencies = $nuspec.package.metadata.dependencies.group.dependency
	foreach($dependency in $dependencies) {
		$toAdd = ($dependency.id + ": " + $dependency.version + " <br />")
		$results = $results + $toAdd
	}
	return $results
}

Function Replace-VariablesInTemplate{
	$LinkToSetupExeOnSharedDisk = Join-Path $BuildPackagesDirGold "\Releases\$SdkVersion"
	$MessageToUse = Get-Content -path .\Scripts\Template_einstein.txt -Raw -Encoding UTF8
	if($Branch.StartsWith("release-", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		# This is a release branch check to see if the name of the relativity version is mentioned in the template. This is done because we need to manually opdate the template for each version, 
		# and if you forget we want the pipeline to warn us by throwing an exception, so you actually fix it. $Branch is something like "release-1.11-mayapple"
		# the documentation just needs 2 more lines of HTML if this throws.
		$RelVersion = $Branch.Split('-')[2]
		write-host $RelVersion
		if($MessageToUse.IndexOf($RelVersion, [System.StringComparison]::InvariantCultureIgnoreCase) -le 0)
		{
			Throw "This operation cannot be performed because the message on einstein does not have a green check for '$RelVersion'. Add this check in .\Scripts\Template_einstein.txt, so our documentation is updated."
		}
	}
	elseif($Branch.StartsWith("master", [System.StringComparison]::InvariantCultureIgnoreCase))
	{
		# This is a master branch (used for releases) check to see if the name "Monthly Releases" is mentioned in the template. This is done because we need to manually opdate the template for each version,
		# and if you forget we want the pipeline to warn us by throwing an exception, so you actually fix it. 
		# the documentation just needs 2 more lines of HTML if this throws.
		if($MessageToUse.IndexOf("Monthly Releases", [System.StringComparison]::InvariantCultureIgnoreCase) -le 0)
		{
			Throw "This operation cannot be performed because the message on einstein does not have a green check for '$Branch'. Add this check in .\Scripts\Template_einstein.txt, so our documentation is updated."
		}
	}
	
	$MessageToUse = $MessageToUse -replace '<sdk_version_in_build>', $SdkVersion
	$MessageToUse = $MessageToUse -replace '<rdc_version_in_build>', $RdcVersion
	$MessageToUse = $MessageToUse -replace '<creation_date_of_build>', (Get-Date -UFormat "%B %d %Y").ToString()
	$MessageToUse = $MessageToUse -replace '<link_to_sdk_nuget>', "https://relativity.jfrog.io/relativity/nuget-local/Relativity.DataExchange.Client.SDK.$SdkVersion.nupkg"
	$MessageToUse = $MessageToUse -replace '<link_to_rdc_nuget>', "https://relativity.jfrog.io/relativity/webapp/#/artifacts/browse/tree/General/nuget-local/Relativity.Desktop.Client.$RdcVersion.nupkg"
	$fileHash = (Get-FileHash $PathToLocalRdcExe).Hash
	$MessageToUse = $MessageToUse -replace '<SHA256_of_build>', $fileHash
	$MessageToUse = $MessageToUse -replace '<link_to_setup_exe_in_bld_pkg>', $LinkToSetupExeOnSharedDisk
	$MessageToUse = $MessageToUse -replace '<section_with_dependencies>', (Get-DependencyList).ToString()
	

	return $MessageToUse
}

Function Get-ParentPageId{
	# 153613395 is the id of the Data Transfer SDK - Releases page -> https://einstein.kcura.com/x/U-QnCQ
	$idToUse = 153613395
	$UrlToGetChildren = "https://einstein.kcura.com/rest/api/content/search?cql=parent=$idToUse"

	$response = Invoke-WebRequest -Uri $UrlToGetChildren -Method Get -Headers $Headers -UseBasicParsing
	$jsonObj = ConvertFrom-Json $response.Content
	[string]$namesToSearchFor = $Branch, "Monthly Releases"
	$objectToPostUnder = $jsonObj | Select-Object -ExpandProperty results |	Where-Object { $namesToSearchFor.IndexOf($_.title, [System.StringComparison]::InvariantCultureIgnoreCase) -ge 0 }
	$objectToPostUnderId = 0	
	if($objectToPostUnder.id -gt 1)
	{
		$objectToPostUnderId = $objectToPostUnder.id
		Write-Host "Object id to create post under = $objectToPostUnderId"
	}
	else
	{
		$objectToPostUnderId = $idToUse
		Write-Host "Object id to create post under not found, posting under the root ($objectToPostUnderId)"
	}
	return $objectToPostUnderId
}

try{
	$pair = "svc_conf_gbu:$Secret"
	$BasicAuthValue = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))

	$Headers = @{
	Authorization = "Basic $BasicAuthValue"
	}
	$TitleToUse = "Relativity.DataExchange.Client.SDK $SdkVersion Release Notes" 


	$PublishNewPageURL = "https://einstein.kcura.com/rest/api/content/"
	$BodyJSON = '{
	"type":"page",
	"ancestors":[
		{
			"id": 0
		}
	],
	"title":"",
	"space":{
		"key":"DTV"
	},
	"body":{
		"storage":{
			"value":"",
			"representation":"storage"
		}
	}
	}'
	$MessageToUse = Replace-VariablesInTemplate
	$ObjectToPostUnderId = Get-ParentPageId
	$BodyJSONParsed = $BodyJSON | ConvertFrom-Json 
	$BodyJSONParsed.ancestors[0].id = $objectToPostUnderId
	$BodyJSONParsed.title = $TitleToUse
	$BodyJSONParsed.body.storage.value = $MessageToUse
	$BodyJSON = ConvertTo-Json $BodyJSONParsed
	$Returned = Invoke-WebRequest -Uri $PublishNewPageURL -Method Post -Headers $Headers  -body $BodyJSON -ContentType 'application/json' -UseBasicParsing
	Write-Host $Returned.Content
}
catch [System.Net.WebException] {   
		$respStream = $_.Exception.Response.GetResponseStream()
		$reader = New-Object System.IO.StreamReader($respStream)
		$respBody = $reader.ReadToEnd() | ConvertFrom-Json
		$respBody
		throw
 }







