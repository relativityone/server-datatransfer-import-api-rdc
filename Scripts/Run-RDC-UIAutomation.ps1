$rootPath = Split-Path $PSScriptRoot -Parent
$nunitRunnerPath = Join-Path -Path $rootPath -ChildPath "\packages\NUnit.ConsoleRunner\tools\nunit3-console.exe"
$testDllPath = Join-Path -Path $rootPath -ChildPath "\Source\Relativity.Desktop.Client.Legacy.Tests.UI\bin\Release\Relativity.Desktop.Client.Legacy.Tests.UI.dll"
& $nunitRunnerPath $testDllPath