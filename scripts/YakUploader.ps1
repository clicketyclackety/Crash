param (
    [Parameter(Mandatory=$false, HelpMessage="What Config was Project built for?")]
    [string]$configuration="Debug",

    [Parameter(Mandatory=$false, HelpMessage="Push to yak server?")]
    [bool]$publish=$False
)

$script_dir = $PSScriptRoot
$base_dir = (get-item $script_dir).parent.FullName
$buildDir = "$base_dir\src\Crash\bin\$configuration\net48\"

$loc = Get-Location
Set-Location $buildDir

# Build plugin
& dotnet build -c $configuration "$base_dir/src/Crash/Crash.csproj"

# Build servers
& dotnet publish -c $configuration -r win-x64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True
# & dotnet publish -c $configuration -r osx-x64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True
# & dotnet publish -c $configuration -r osx-arm64 "$base_dir/Crash.Server/Crash.Server.csproj" /p:Publish=True

$yakexe = "C:\Program Files\Rhino 7\System\Yak.exe"
& $yakexe build --platform win

if($publish -eq $True) {
    & $yakexe push "$buildDir\crash-1.0.0-rh7_21-win.yak"
}

Set-Location $loc