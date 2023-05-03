$server = $env:SERVER
$userName = $env:USERNAME
if (-not ($userName.StartsWith("BANCOLOMBIA") -or $userName.StartsWith("BANCOAGRICOLA")))
{
    $userName = "$($server)\$($userName)"
}
$securePassword = ConvertTo-SecureString -AsPlainText -String $env:PASSWORD -Force
$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $userName, $securePassword

$path = $env:ARTIFACTPATH
$tempBasePath = $env:TEMP_BASEPATH
$tempPath = $env:TEMPPATH
$artifactName = $env:ARTIFACTNAME

if (-NOT (Test-Path -Path $path)) {
    Write-Error "No se encontro el compilado en la ruta $($path)"
    throw "FileNotFound"
}

Write-Host "Iniciando sesion en servidor $($server) con usuario $($userName)"
$session = New-PSSession $server -Credential $credential

Write-Host 'Limpiando archivos en servidor'
Invoke-Command -Session $session -ScriptBlock {
    if (Test-Path -Path $using:tempPath) {
        Remove-Item -Path "$($using:tempPath)\*" -Recurse -Verbose
    }
}

Write-Host 'Copiando archivos a servidor'
Copy-Item $path -Destination $($tempBasePath) -ToSession $session -Recurse -Force

Write-Host 'Extrayendo archivos'
Invoke-Command -Session $session -ScriptBlock {
    Expand-Archive -Path "$($using:tempBasePath)\$($using:artifactName).zip" -DestinationPath $using:tempPath;
    Remove-Item -Path "$($using:tempBasePath)\$($using:artifactName).zip"
}