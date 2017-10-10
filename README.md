# Relativity Desktop Client (WinEDDS) Installer
 
## Overview
 
The installer for the Relativity Desktop Client. WinEDDS is the legacy name of the RDC.
 
## How to Build

Through Visual Studio:
1. Open up the kCura.WinEDDS.Setup.sln under the Installers folder using Visual Studio.
1. Run 'Build Solution' in AnyCPU first to ensure all dependencies are compiled.
1. Run 'Build Solution', or build just the kCura.WinEDDS.Setup project, in x86 or x64, to build the 32- or 64-bit installer.

From the command line:

    .\build.ps1 BuildWinEDDSInstallers [-Release]

## Manual Test Cases

Notation:

V1: version of code before changes.

V2/V3: versions of code after changes.

### Relativity Installation Test Cases

1. Install V1, and check contents of [INSTALLDIR]\EDDS\WinEDDSInstaller.
    - Upgrade to V2, and verify contents are “the same” (slight size changes or version in name for installers is ok).
1. Do a clean install of V2, and verify contents are “the same” (no upgrade in this scenario).

### Relativity Web Test Cases

1. Both 32- and 64-bit RDC installers can be downloaded from the Workspace Details page of a workspace on the Primary SQL Server.
2. Both 32- and 64-bit RDC installers can be downloaded from the Workspace Details page of a workspace on a Distributed SQL Server.

### RDC Installation Scenarios (32-bit and 64-bit Windows)

Note: test uninstall and repair both through the UI, and through Add/Remove Programs.

1. Install V1, upgrade to V2, upgrade to V3, uninstall V3.
1. Install V2, upgrade to V3, uninstall V3.
1. Install V2, uninstall V2.
1. Install V2, repair.

### RDC Installation Behavior

1. For the 32-bit RDC on a 32-bit OS, the default installation directory is [ProgramFiles]\kCura Corporation\Relativity Desktop Client.
1. For the 32-bit RDC on a 64-bit OS, the default installation directory is [ProgramFiles86]\kCura Corporation]Relativity Desktop Client.
1. For the 64-bit RDC on a 64-bit OS, the default installation directory is [ProgramFiles]\kCura Corporation\Relativity Desktop Client.
1. Installation places a working shortcut on the user's desktop to launch the RDC.
1. Installation places a working shortcut in the Start menu to launch the RDC.
1. Installation creates a Windows Firewall rule named "Relativity Desktop Client TCP" that opens all incoming TCP ports.
1. Installation creates a Windows Firewall rule named "Relativity Desktop Client UDP" that opens all incoming UDP ports.
1. Installation creates an empty Registry key at the path HKCU\Software\kCura\RelativityDesktopClient.
1. Installation creates a Registry key at the path HKLM\Software\kCura\RelativityDesktopClient\Path with the value of [INSTALLDIR]\kCura.EDDS.WinForm.exe.

### RDC Installation Tests (32-bit and 64-bit Windows)

1. When installed, Add/Remove Programs lists product with correct version and branding (Relativity ODA LLC, icon).
1. 32- and 64-bit MSIs signed on GOLD builds.

### UI workflows (32- and 64-bit Windows)

1. Running the installer MSI opens a UI with a Relativity branded welcome screen.
1. Clicking `Next` navigates forward and gives the user the option of a `Typical`, `Custom`, or `Complete` install. This page has a Relativity branded banner.
1. `Typical` and `Complete` both navigate forward to the confirmation dialog, with a Relativity branded banner.
1. `Custom` navigates forward and gives the user of selecting features to install, and a non-default install directory, with a Relativity branded banner.
    - Clicking `Next` on this dialog navigates forward to the confirmation dialog.
1. Clicking `Next` on this confirmation dialog proceeds with the installation to the default (or customized) install directory. This dialog shows a progress bar and has a Relatiivty branded banner.
1. After installation completes, the UI continues to the final dialog, with a Relativity branded banner, letting the user exit.

Clicking `Cancel` at any time during the install UI sequence will prompt the user, and if confirmed, cancel the install process and proceed to a Relativity branded final dialog.

### CLI workflows (32- and 64-bit Windows)

1. Running `msiexec.exe /i kCura.WinEDDS.Setup.msi /quiet [/l*v InstallLog.txt]` will launch the installer in quiet mode (no UI), and if successful, install the RDC to the default location.
1. Running `msiexec.exe /i kCura.WinEDDS.Setup.msi /quiet [/l*v InstallLog.txt] INSTALLDIR=C:\TestPath` will launch the installer in quiet mode (no UI), and if successful, install the RDC to C:\TestPath (or any other specified path).
1. Running `msiexec.exe /x kCura.WinEDDS.Setup.msi /quiet [/l*v UninstallLog.txt]` will launch the uninstaller in quiet mode (no UI), and if successful, remove the RDC from the machine.

### Error scenarios

1. The 64-bit RDC will fails to install on a 32-bit OS. On launch, the user gets the message: "This installation package is not supported by this processor type. Contact your product vendor."
1. Attempting to downgrade (install V1 when V2 is already installed) prompts user with message: "Unable to install because a newer version of this product is already installed."
1. Attempting to install both the 32- and 64-bit RDC on the same machine fails. The user will be prompted with the message: "Unable to install because a newer version of this product is already installed."
1. Attempting to install either the 32- or 64-bit RDC on a machine without .NET Framework 4.5 fails with the error message: "This application requires .NET Framework 4.5. Please install the .NET Framework then run this installer again."
 
## Build artifacts
 
- [root]\Installer\MSI\kCura.WinEDDS.Setup.msi (32-bit)
- [root]\Installer\MSI\kCura.WinEDDS64.Setup.msi (64-bit)

## Maintainers
 
- Buena Vista Coding Club (BuenaVistaCodingClub@relativity.com)
- Tools (tools@kcura.com)
