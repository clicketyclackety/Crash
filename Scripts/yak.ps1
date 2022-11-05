param (
    [Parameter(Mandatory=$false, HelpMessage="What Config was Project built for?")]
    [string]$configuration="Debug"
)

$script_dir = $PSScriptRoot
$base_dir = (get-item $script_dir).parent.FullName
$buildDir = "$base_dir\Crash\bin\x64\$configuration\net48\"

$loc = Get-Location
Set-Location $buildDir

$yakexe = "C:\Program Files\Rhino 7\System\Yak.exe"
& $yakexe build --platform win

Set-Location $loc