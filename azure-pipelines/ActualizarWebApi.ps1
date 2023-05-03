$server = $env:SERVER
$userName = $env:USERNAME
if (-not ($userName.StartsWith("BANCOLOMBIA") -or $userName.StartsWith("BANCOAGRICOLA")))
{
    $userName = "$($server)\$($userName)"
}
$securePassword = ConvertTo-SecureString -AsPlainText -String $env:PASSWORD -Force
$credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $userName, $securePassword

$appPoolName = $env:APPPOOL
$appPath = $env:APPPATH
$tempPath = $env:TEMPPATH

Write-Host "Iniciando sesion en servidor $($server ) con usuario $($userName)"
$iisSession = New-PSSession $server -Credential $credential -ConfigurationName AzurePipelines
$copySession = New-PSSession $server -Credential $credential

Write-Host "Deteniendo app pool $($appPoolName)"
Invoke-Command -Session $iisSession -ScriptBlock {
    $state = Get-WebAppPoolState -Name $using:appPoolName
    if (($state.Value -eq 'Started') -or ($state.Value -eq 'Starting'))
    {
        Stop-WebAppPool -Name $using:appPoolName
    }
    Start-Sleep -Seconds 10
}

Write-Host "Copiando archivos hacia $($appPath)"
Invoke-Command -Session $copySession -ScriptBlock {
    Copy-Item -Path "$($using:tempPath)\*" -Destination $using:appPath -Exclude 'appsettings*.json' -Recurse -Force -ErrorAction:SilentlyContinue -ErrorVariable e
    if ($e) {
        Start-Sleep -Seconds 10
        Copy-Item -Path "$($using:tempPath)\*" -Destination $using:appPath -Exclude 'appsettings*.json' -Recurse -Force
    }
}

Write-Host "Iniciando app pool $($appPoolName)"
Invoke-Command -Session $iisSession -ScriptBlock {
    Start-WebAppPool -Name $using:appPoolName -ErrorAction:SilentlyContinue -ErrorVariable e
    if ($e) {
        Start-WebAppPool -Name $using:appPoolName
    }
}