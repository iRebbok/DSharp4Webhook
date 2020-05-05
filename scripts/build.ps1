# Processing incoming arguments
param (
    # the only thing that keeps me from using boolean directly - github actions
    [string]$IsMonoArg = "",
    [string]$VersionPrefix = ""
)

$IsMono = $false
if ($IsMonoArg -eq "true") { $IsMono = $true }

Write-Output @"
IsMono=$IsMono
VersionPrefix=$($VersionPrefix.Length -eq 0 ? "None" : $VersionPrefix)
"@

# Attach to the project folder
Set-Location -Path "$PSScriptRoot\..\"

$DeployPath = Join-Path -Path (Get-Location) -ChildPath "deploy"
$DeployFolder = Join-Path -Path $DeployPath -ChildPath ($IsMono ? "mono" : "")

Invoke-Expression 'dotnet restore'

$Expression = 'dotnet pack -c Release'

if ($IsMono) { $Expression += ' /p:DefineConstants=MONO_BUILD' }
if ($VersionPrefix.Length -ne 0) { $Expression += ' /p:VersionPrefix=$VersionPrefix' }

Invoke-Expression $Expression

if(!$?) { Exit $LASTEXITCODE }

if (-not (Test-Path $DeployPath)) { New-Item $DeployPath -ItemType "directory" }

Get-ChildItem -Path "src/bin/Release" -Directory -Recurse | ForEach-Object {
    $FolderName = Split-Path $_.FullName -Leaf
    $FinalFolder = ($IsMono ? ($DeployFolder + '-') : $DeployFolder) + $FolderName
    if (-not (Test-Path $FinalFolder)) { New-Item $FinalFolder -ItemType "directory" }
    Get-ChildItem -Path $_.FullName -File -Recurse | ForEach-Object { Copy-Item $_.FullName (Join-Path $FinalFolder $_.Name) -Force }
    # Immediately pack them
    Compress-Archive -Path ($_.FullName + "\*") -DestinationPath ($FinalFolder + ".zip") -Force
}

# Transferring the remaining nuget packages if it's not mono
if (-not $IsMono) { Get-ChildItem -Path "src/bin/Release" -File | Where-Object Name -Match ".+\.nupkg" | ForEach-Object { Copy-Item $_.FullName (Join-Path $DeployPath $_.Name) -Force } }
