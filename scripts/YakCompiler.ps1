param (
    [Parameter(Mandatory=$true, HelpMessage="The input directory")]
    [Alias("Path")]
    [string]$inputDir,

    [Parameter(Mandatory=$false, HelpMessage="The output location")]
    [Alias("DestinationPath")]
    [string]$outputDir = $inputDir,

    [Parameter(Mandatory=$false, HelpMessage="What Config was Project built for?")]
    [Alias("Config")]
    [string]$configuration="Debug",

    [Parameter(Mandatory=$false, HelpMessage="What OS is current running")]
    [string]$os="win",

    [Parameter(Mandatory=$false, HelpMessage="Minimum required version of Rhino")]
    [Alias("Yak")]
    [string]$yakExe="C:\Program Files\Rhino 7\System\Yak.exe",

    [Parameter(Mandatory=$false, HelpMessage="Push to yak server?")]
    [bool]$publish=$false
)

$crashVersion = "1.0.0"

foreach($yakFile in Get-ChildItem "$outputDir\*.yak")
{
    Remove-Item $yakFile
}

$originalLocation = Get-Location
Set-Location $inputDir

& $yakExe build --platform $os

if ($inputDir -ne $outputDir)
{
    Copy-Item -Path "$inputDir\*.yak" -DestinationPath $outputDir
}

<#
Compress-Archive -Path "$inputDir\*.*" -DestinationPath $zipFile -CompressionLevel Optimal
Compress-Archive -Path "$inputDir\Server" -DestinationPath $zipFile -CompressionLevel Optimal -Update
Rename-Item $zipFile $yakFile
#>

Set-Location $originalLocation