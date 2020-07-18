function ProcessDirectory {
    param ($Path)

    if (-not (Test-Path $Path -PathType Container)) { New-Item -Path $Path -ItemType Container }
}

# Attach to the project folder
Set-Location -Path "$PSScriptRoot\..\"

$DeployPath = Join-Path -Path (Get-Location) -ChildPath "deploy"

Invoke-Expression 'dotnet restore'

Invoke-Expression 'dotnet build -c release'

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
    }
}
