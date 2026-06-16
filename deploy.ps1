$ErrorActionPreference = "Stop"

$Server = "root@159.89.197.103"
$RemoteDir = "/var/www/barangay-api"

$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ClientDir = Join-Path $RootDir "client"
$ApiDir = Join-Path $RootDir "API"
$PublishDir = Join-Path $RootDir "deploy\barangay-api"
$LogoDir = Join-Path $ClientDir "public\images\logos"

Write-Host "=== CHECK LOGOS ==="
if (!(Test-Path $LogoDir)) {
  throw "Logo folder missing: $LogoDir. Create it and paste your logos there."
}

$logoCount = (Get-ChildItem $LogoDir -File -ErrorAction SilentlyContinue | Measure-Object).Count
if ($logoCount -eq 0) {
  throw "No logo files found in $LogoDir. Paste your logo images there before deploying." 
}

Write-Host "=== CLEAN LOCAL DEPLOY OUTPUT ==="
Remove-Item -Recurse -Force $PublishDir -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force (Join-Path $ApiDir "publish") -ErrorAction SilentlyContinue

Write-Host "=== BUILD CLIENT ==="
Push-Location $ClientDir
npm install
ng build --configuration production
Pop-Location

Write-Host "=== CLEAN API WWWROOT ==="
Push-Location $ApiDir
Remove-Item -Recurse -Force ".\wwwroot\*" -ErrorAction SilentlyContinue

Write-Host "=== COPY ANGULAR BUILD TO API WWWROOT ==="
Copy-Item -Recurse "..\client\dist\browser\*" ".\wwwroot\"

Write-Host "=== PUBLISH API OUTSIDE PROJECT ==="
dotnet publish ".\API.csproj" -c Release -o $PublishDir
Pop-Location

Write-Host "=== PREPARE SERVER AND PRESERVE UPLOADS ==="
ssh $Server "mkdir -p $RemoteDir/uploads/certificates $RemoteDir/uploads/images/ids $RemoteDir/uploads/images/concerns $RemoteDir/uploads/images/profiles $RemoteDir/uploads/images/officials $RemoteDir/uploads/audio" 

Write-Host "=== STOP SERVICE AND CLEAN OLD APP FILES, BUT KEEP UPLOADS ==="
ssh $Server "systemctl stop barangay-api || true; mkdir -p $RemoteDir; find $RemoteDir -mindepth 1 -maxdepth 1 ! -name uploads -exec rm -rf {} +"

Write-Host "=== DEPLOY TO SERVER ==="
$PublishGlob = Join-Path $PublishDir "*"
scp -r $PublishGlob "${Server}:$RemoteDir/"

Write-Host "=== FIX PERMISSIONS AND RESTART ==="
ssh $Server "chown -R www-data:www-data $RemoteDir; systemctl restart barangay-api; systemctl reload nginx"

Write-Host "=== DONE ==="
