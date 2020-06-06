# Processing incoming arguments
param (
    [string]$VersionPrefix = ""
)

# Processing the error if there is one
function ProcessFailed {
    param ($Message)

    if ($LASTEXITCODE -ne 0) {
        Write-Output $Message
        Exit $LASTEXITCODE
    }
}

function ProcessDirectory {
    param ($Path)

    if (-not (Test-Path $Path -PathType Container)) { New-Item -Path $Path -ItemType Container }
}

Write-Output @"
VersionPrefix=$($VersionPrefix.Length -eq 0 ? "None" : $VersionPrefix)
"@

# Attach to the project folder
Set-Location -Path "$PSScriptRoot\..\"

$DeployPath = Join-Path -Path (Get-Location) -ChildPath "deploy"

Invoke-Expression 'dotnet restore'
ProcessFailed "Restoring failed"

$Expression = 'dotnet pack -c Release'

if ($VersionPrefix.Length -ne 0) { $Expression += ' /p:VersionPrefix=$VersionPrefix' }

Invoke-Expression $Expression
# This is how we handle errors
# $? does not work properly with the Invoke-Expression
ProcessFailed "Build failed"
Write-Output "Build is successful"

# Packing everything in a deploy folder
ProcessDirectory $DeployPath
Get-ChildItem -Path 'src\' -Directory -Recurse | Where-Object { $_.FullName.EndsWith("bin\Release") } | ForEach-Object {
    $ProjectName = $_.FullName.Split('\')[-3]
    Get-ChildItem -Path $_.FullName -Directory -Recurse | ForEach-Object {
        $FrameworkName = $_.Name
        $FinalName = "$ProjectName-$FrameworkName"
        $OutputPath = Join-Path $DeployPath $FinalName
        ProcessDirectory $OutputPath
        Copy-Item -Path ($_.FullName + "\*") -Destination $OutputPath -Recurse -Force
        Compress-7Zip -ArchiveFileName ($FinalName + '.tar') -Path $_.FullName -OutputPath $DeployPath -Format 'Tar'
        Compress-7Zip -ArchiveFileName ($FinalName + '.tar.gz') -Path ($OutputPath + '.tar') -OutputPath $DeployPath -Format 'GZip' -CompressionLevel 'High'
    }
}

Remove-Item -Path $DeployPath -Include '*.tar' -Recurse -Force
