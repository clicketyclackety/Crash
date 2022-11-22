param (
    [Parameter(Mandatory=$true, HelpMessage="The input directory")]
    [Alias("Path")]
    [string]$inputDir,

    [Parameter(Mandatory=$false, HelpMessage="The output location")]
    [Alias("DestinationPth")]
    [string]$outputDir = $inputDir,

    [Parameter(Mandatory=$false, HelpMessage="What Config was Project built for?")]
    [Alias("Config")]
    [string]$configuration="Debug",

    [Parameter(Mandatory=$false, HelpMessage="What OS is current running")]
    [string]$os="win",

    [Parameter(Mandatory=$false, HelpMessage="Minimum required version of Rhino")]
    [Alias("Version")]
    [string]$rhinoVersion="7.21",

    [Parameter(Mandatory=$false, HelpMessage="Push to yak server?")]
    [bool]$publish=$false
)

$formattedVersion = $rhinoVersion.Replace('.','_')
$crashVersion = "1.0.0"
$formattedOS = "error"
if ($os.ToLower().Contains(("win"))
{
    $formattedOS = "win"
}
else if ($os.ToLower().Contains(("mac"))
{
    $formattedOS = "mac"
}

$fileName = "Crash"
$yakFile = "$outputDir\\crash-$crashVersion-rh$formattedVersion-$formattedOS.yak"

if (Test-Path $yakFile)
{
    Remove-Item $yakFile
}

Compress-Archive -Path "$inputDir\*.*" -DestinationPath $yakFile -CompressionLevel Optimal
Compress-Archive -Path "$inputDir\Server" -DestinationPath $yakFile -CompressionLevel Optimal -Update
