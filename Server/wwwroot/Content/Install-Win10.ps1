<#
.SYNOPSIS
   Installs the URemote Client.
.DESCRIPTION
   Do not modify this script.  It was generated specifically for your account.
.EXAMPLE
   powershell.exe -f Install-Win10.ps1
   powershell.exe -f Install-Win10.ps1 -DeviceAlias "My Super Computer" -DeviceGroup "My Stuff"
#>

param (
	[string]$DeviceAlias,
	[string]$DeviceGroup,
	[string]$Path,
	[switch]$Uninstall
)

[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
$LogPath = "$env:TEMP\URemote_Install.txt"
[string]$HostName = $null
[string]$Organization = $null
$ConnectionInfo = $null

if ([System.Environment]::Is64BitOperatingSystem){
	$Platform = "x64"
}
else {
	$Platform = "x86"
}

$InstallPath = "$env:ProgramFiles\URemote"

function Write-Log($Message){
	Write-Host $Message
	"$((Get-Date).ToString()) - $Message" | Out-File -FilePath $LogPath -Append
}
function Do-Exit(){
	Write-Host "Exiting..."
	Start-Sleep -Seconds 3
	exit
}
function Is-Administrator() {
    $Identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $Principal = New-Object System.Security.Principal.WindowsPrincipal -ArgumentList $Identity
    return $Principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
} 

function Run-StartupChecks {

	if ($HostName -eq $null -or $Organization -eq $null) {
		Write-Log "Required parameters are missing.  Please try downloading the installer again."
		Do-Exit
	}

	if ((Is-Administrator) -eq $false) {
		Write-Log -Message "Install script requires elevation.  Attempting to self-elevate..."
		Start-Sleep -Seconds 3
		$param = "-f `"$($MyInvocation.ScriptName)`""

		Start-Process -FilePath powershell.exe -ArgumentList "-DeviceAlias $DeviceAlias -DeviceGroup $DeviceGroup -Path $Path" -Verb RunAs
		exit
	}
}

function Stop-URemote {
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc delete URemote_Service" -Wait -WindowStyle Hidden
	Stop-Process -Name URemote_Agent -Force -ErrorAction SilentlyContinue
	Stop-Process -Name URemote_Desktop -Force -ErrorAction SilentlyContinue
}

function Uninstall-URemote {
	Stop-URemote
	Remove-Item -Path $InstallPath -Force -Recurse -ErrorAction SilentlyContinue
	Remove-NetFirewallRule -Name "URemote ScreenCast" -ErrorAction SilentlyContinue
}

function Install-URemote {
	if ((Test-Path -Path "$InstallPath") -and (Test-Path -Path "$InstallPath\ConnectionInfo.json")) {
		$ConnectionInfo = Get-Content -Path "$InstallPath\ConnectionInfo.json" | ConvertFrom-Json
		if ($ConnectionInfo -ne $null) {
			$ConnectionInfo.Host = $HostName
			$ConnectionInfo.OrganizationID = $Organization
			$ConnectionInfo.ServerVerificationToken = ""
		}
	}
	else {
		New-Item -ItemType Directory -Path "$InstallPath" -Force
	}

	if ($ConnectionInfo -eq $null) {
		$ConnectionInfo = @{
			DeviceID = (New-Guid).ToString();
			Host = $HostName;
			OrganizationID = $Organization;
			ServerVerificationToken = "";
		}
	}

	if ($HostName.EndsWith("/")) {
		$HostName = $HostName.Substring(0, $HostName.LastIndexOf("/"))
	}

	if ($Path) {
		Write-Log "Copying install files..."
		Copy-Item -Path $Path -Destination "$env:TEMP\URemote-Win10-$Platform.zip"

	}
	else {
		$ProgressPreference = 'SilentlyContinue'
		Write-Log "Downloading client..."
		Invoke-WebRequest -Uri "$HostName/Content/URemote-Win10-$Platform.zip" -OutFile "$env:TEMP\URemote-Win10-$Platform.zip" 
		$ProgressPreference = 'Continue'
	}

	if (!(Test-Path -Path "$env:TEMP\URemote-Win10-$Platform.zip")) {
		Write-Log "Client files failed to download."
		Do-Exit
	}

	Stop-URemote
	Get-ChildItem -Path "C:\Program Files\URemote" | Where-Object {$_.Name -notlike "ConnectionInfo.json"} | Remove-Item -Recurse -Force

	Expand-Archive -Path "$env:TEMP\URemote-Win10-$Platform.zip" -DestinationPath "$InstallPath"  -Force

	New-Item -ItemType File -Path "$InstallPath\ConnectionInfo.json" -Value (ConvertTo-Json -InputObject $ConnectionInfo) -Force

	if ($DeviceAlias -or $DeviceGroup) {
		$DeviceSetupOptions = @{
			DeviceAlias = $DeviceAlias;
			DeviceGroup = $DeviceGroup;
			OrganizationID = $Organization;
			DeviceID = $ConnectionInfo.DeviceID;
		}

		Invoke-RestMethod -Method Post -ContentType "application/json" -Uri "$HostName/api/devices" -Body $DeviceSetupOptions -UseBasicParsing
	}

	New-Service -Name "URemote_Service" -BinaryPathName "$InstallPath\URemote_Agent.exe" -DisplayName "URemote Service" -StartupType Automatic -Description "Background service that maintains a connection to the URemote server.  The service is used for remote support and maintenance by this computer's administrators."
	Start-Process -FilePath "cmd.exe" -ArgumentList "/c sc.exe failure `"URemote_Service`" reset=5 actions=restart/5000" -Wait -WindowStyle Hidden
	Start-Service -Name URemote_Service

	New-NetFirewallRule -Name "URemote Desktop Unattended" -DisplayName "URemote Desktop Unattended" -Description "The agent that allows screen sharing and remote control for URemote." -Direction Inbound -Enabled True -Action Allow -Program "C:\Program Files\URemote\Desktop\URemote_Desktop.exe" -ErrorAction SilentlyContinue
}

try {
	Run-StartupChecks

	Write-Log "Install/uninstall logs are being written to `"$LogPath`""
    Write-Log

	if ($Uninstall) {
		Write-Log "Uninstall started."
		Uninstall-URemote
		Write-Log "Uninstall completed."
		exit
	}
	else {
		Write-Log "Install started."
        Write-Log
		Install-URemote
		Write-Log "Install completed."
		exit
	}
}
catch {
	Write-Log -Message "Error occurred: $($Error[0].InvocationInfo.PositionMessage)"
	Do-Exit
}
