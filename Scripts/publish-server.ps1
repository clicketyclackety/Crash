param (
    [Parameter(Mandatory=$false, HelpMessage="What Config was Project built for?")]
    [string]$configuration="Debug"
)

$script_dir = $PSScriptRoot
$base_dir = (get-item $script_dir).parent.FullName
$buildDir = "$base_dir\Crash\bin\x64\$configuration\net48\"

$loc = Get-Location
Set-Location $buildDir

& dotnet publish -c $configuration -r win-x64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True
& dotnet publish -c $configuration -r osx-x64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True
& dotnet publish -c $configuration -r osx-arm64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True

Set-Location $loc