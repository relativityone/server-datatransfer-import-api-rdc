<#
.SYNOPSIS
This will update the product version in the centralized version database.

.DESCRIPTION
This script is meant to be used on build servers. It can run on 
devlopment machines but it should be noted that you will incrementing 
the version used for production versions.

.PARAMETER Version
The current version to increment. (i.e. param value of 9.6.2 will 
increment the next patch version, returning a value similar to 9.6.2.23).

.PARAMETER VersionDatabaseServerName
The version database server name.

.PARAMETER VersionDatabaseName
The name of the database on the version database server. 

.PARAMETER VersionDatabaseUsername
The username for the version database.

.PARAMETER VersionDatabasePassword
The password for the version database user.

.PARAMETER Force
Will surpress the prompt warning the user if the script is run on a 
devleopment computer.

.Example
.\Increment-ProductVersion.ps1 -Version (Get-Version ..\Version\version.txt)

Increments and return the value of the version in the version.txt
#>

#Requires -Version 5.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [version] $Version,
    [string] $VersionDatabaseServerName = "BLD-MSTR-01.kcura.corp",
    [string] $VersionDatabaseName = "TCBuildVersion",
    [string] $VersionDatabaseUsername = "StoryboardUser",
    [string] $VersionDatabasePassword = "Test1234!",
    [switch] $Force
)

begin {
    if (-not $Force.IsPresent -and $env:COMPUTERNAME.StartsWith("P-DV-"))
    {
        $options = [System.Management.Automation.Host.ChoiceDescription[]] @("&No", "&Yes")
        [int]$defaultchoice = 0
        $opt = $host.UI.PromptForChoice("Warning!" , "This script will update the production version of the product version. This should not be done on a development computer. Are you sure you want to proceed?" , $Options,$defaultchoice)
        switch($opt)
        {
            0 
            { 
                Write-Verbose "User was warned and chose not to update the version."
                exit 
            }

            1 
            { 
                Write-Verbose "User was warned and chose to update the version."
            }
        }
    }
}

process {
    $project = "Development"
    $productNameForVersioning = "IAPI"
    $Conn = New-Object System.Data.SqlClient.SqlConnection
    $Conn.ConnectionString = "server='$VersionDatabaseServerName';Database='$VersionDatabaseName';user=$VersionDatabaseUsername;password=$VersionDatabasePassword;"

    try { 
        $Conn.Open()
    
        $Comm = New-Object System.Data.SqlClient.SqlCommand
        $Comm.Connection = $Conn
        $Comm.CommandText = "
MERGE TCBuildSemanticVersion as Target
USING (SELECT
		@productNameParam AS ProductName, 
		@projectNameParam AS ProjectName, 
		@majorVersionParam AS Major, 
		@minorVersionParam AS Minor, 
		@patchVersionParam AS Patch) 
	AS Source
ON (Target.ProductName = Source.ProductName
	and Target.ProjectName = Source.ProjectName
	and Target.Major = Source.Major
	and Target.Minor = Source.Minor
	and Target.Patch = Source.Patch)
WHEN MATCHED THEN
	UPDATE SET Target.Build = (Target.Build + 1)
WHEN NOT MATCHED BY Target THEN
	INSERT (ProductName, ProjectName, Major, Minor, Patch, Build)
    VALUES (Source.ProductName, Source.ProjectName, Source.Major, Source.Minor, Source.Patch, 1)
OUTPUT inserted.Build;
"
        $productNameParam = New-Object System.Data.SqlClient.SqlParameter "@productNameParam", System.Data.SqlDbType.VarChar
        $productNameParam.Value = "IAPI"
        $projectNameParam = New-Object System.Data.SqlClient.SqlParameter "@projectNameParam", System.Data.SqlDbType.VarChar
        $projectNameParam.Value = "Development"
        $majorVersionParam = New-Object System.Data.SqlClient.SqlParameter "@majorVersionParam", System.Data.SqlDbType.Int
        $majorVersionParam.Value = $Version.Major
        $minorVersionParam = New-Object System.Data.SqlClient.SqlParameter "@minorVersionParam", System.Data.SqlDbType.Int
        $minorVersionParam.Value = $Version.Minor
        $patchVersionParam = New-Object System.Data.SqlClient.SqlParameter "@patchVersionParam", System.Data.SqlDbType.Int
        $patchVersionParam.Value = $Version.Build
        
        $Comm.Parameters.Clear()
        $Comm.Parameters.Add($productNameParam) | Out-Null
        $Comm.Parameters.Add($projectNameParam) | Out-Null
        $Comm.Parameters.Add($majorVersionParam) | Out-Null
        $Comm.Parameters.Add($minorVersionParam) | Out-Null
        $Comm.Parameters.Add($patchVersionParam) | Out-Null
        
        $RevisionVersion = $Comm.ExecuteScalar()    
        $Conn.close()
        $NewVersion = New-Object System.Version($Version.Major, $Version.Minor, $Version.Build, $RevisionVersion)

        Return $NewVersion
    }
    catch {
        Throw (New-Object System.Exception "Failed to update the version in the production version database with message: $($_.Exception.Message)", $_.Exception)
    }
}

end {

}
