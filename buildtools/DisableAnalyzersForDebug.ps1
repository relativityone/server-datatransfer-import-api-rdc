Add-Type -AssemblyName 'System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Add-Type -AssemblyName 'System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'

[string]$vs = "http://schemas.microsoft.com/developer/msbuild/2003"
$xmlNamespaceManager = New-Object -TypeName System.Xml.XmlNamespaceManager -ArgumentList (New-Object -TypeName System.Xml.NameTable)
$xmlNamespaceManager.AddNamespace("vs",$vs)
$commandPath = Split-Path -parent $PSCommandPath

foreach ($filename in Get-ChildItem -Path $commandPath\..\Source -Recurse -Filter *.??proj -File)
{
	Write-Host "Item changed :" $filename.FullName
    [System.Xml.Linq.XDocument]$document = [System.Xml.Linq.XDocument]::Load($filename.FullName, [System.Xml.Linq.LoadOptions]::PreserveWhitespace)
    foreach ($itemGroup in [System.Xml.XPath.Extensions]::XPathSelectElements($document, "/vs:Project/vs:ItemGroup[vs:Analyzer]", $xmlNamespaceManager))
    {
        [System.Xml.Linq.XAttribute]$condition = New-Object -TypeName System.Xml.Linq.XAttribute -ArgumentList "Condition", "'`$(Configuration)'!='Debug'"

        [System.Xml.Linq.XElement]$when = New-Object -TypeName System.Xml.Linq.XElement -ArgumentList ([System.Xml.Linq.XName]::Get("When",$vs), $condition, $itemGroup)

        [System.Xml.Linq.XElement]$choose = New-Object -TypeName System.Xml.Linq.XElement -ArgumentList ([System.Xml.Linq.XName]::Get("Choose",$vs), $when)

        $itemGroup.ReplaceWith($choose)
    }
    $document.Save($filename.FullName)
}
