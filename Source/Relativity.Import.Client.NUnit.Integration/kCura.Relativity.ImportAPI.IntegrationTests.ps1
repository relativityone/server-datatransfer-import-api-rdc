#requires -version 5.0

param(
    [Parameter(HelpMessage = 'Username for logging into the Relativity Instance.')]
    [String] $RelativityUserName = "relativity.admin@kcura.com",
    [Parameter(Mandatory = $true, HelpMessage = 'Password for logging into the Relativity Instance')]
    [SecureString] $RelativityPassword,
    [Parameter(HelpMessage = 'Username for logging into the Relativity Database')]
    [String] $DBUserName = "eddsdbo",
    [Parameter(HelpMessage = 'Password for logging into the Relativity Database')]
    [SecureString] $DBPassword,
    [Parameter(HelpMessage = 'Database instance name')]
    [String] $DBInstance = "EDDSINSTANCE001",
    [Parameter(Mandatory = $true, HelpMessage = 'Machine name hosting the Relativity Instance')]
    [String] $Server,
    [Parameter(HelpMessage = 'Domain that the Relativity instance is on.')]
    [String] $Domain = "kcura.corp",
    [Parameter(HelpMessage = 'Specifies if the server is not on the domain.')]
    [Switch] $OffDomain,
    [Parameter(HelpMessage = 'Whether server is using http or https.')]
    [String] $Protocol = "https",
    [Parameter(HelpMessage = 'Specifies if the test suite should report its own results.')]
    [Switch] $Report,
    [Parameter(HelpMessage = 'Credential for reading from ProGet when running in Jenkins')]
    [System.Management.Automation.PSCredential] $Credential = [System.Management.Automation.PSCredential]::Empty,
    [Parameter(HelpMessage = 'TestCategory specified for IAPI')]
    [String] $TestCategory
)

$params = @{
    RequiredVersion = "0.19.3"
    Path = ".\buildtools"
}

if($Credential)
{
    $params.add('Credential', $Credential)
}

Save-Module "TestSuite" @params
Import-Module ".\buildtools\TestSuite\0.19.3\TestSuite.psd1"
$ServerUnderTest = If ($OffDomain) {$Server} Else {"$Server.$Domain"}
$DBPass = [System.Runtime.InteropServices.marshal]::PtrToStringAuto([System.Runtime.InteropServices.marshal]::SecureStringToBSTR($DBPassword))
$RelPass = [System.Runtime.InteropServices.marshal]::PtrToStringAuto([System.Runtime.InteropServices.marshal]::SecureStringToBSTR($RelativityPassword))
$NUnitSuite = Initialize-NUnitSuite -Name "kCura.Relativity.ImportAPI.IntegrationTests" -SuitePath $PSScriptRoot
$NUnitSuite.Bootstrap()
$NUnitSuite.Configure(@{
    AdminUsername = $RelativityUserName
    AdminPassword = $RelPass
    SQLUsername = $DBUserName
    SQLPassword = $DBPass
    ServerBindingType = $Protocol
    RESTServerAddress = $ServerUnderTest
    RSAPIServerAddress = $ServerUnderTest
    SQLServerAddress = "$ServerUnderTest\$DBInstance"
})

$NUnitSuite.RunTests("--where cat==$TestCategory")

if($Report)
{
    $NUnitSuite.Report()
}
