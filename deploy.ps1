$ErrorActionPreference = "Stop"

$Server = "root@159.89.197.103"
$RemoteDir = "/var/www/barangay-api"

$RootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ClientDir = Join-Path $RootDir "client"
$ApiDir = Join-Path $RootDir "API"
$PublishDir = Join-Path $RootDir "deploy\barangay-api"
$LogoDir = Join-Path $ClientDir "public\images\logos"
$ApiWwwRoot = Join-Path $ApiDir "wwwroot"

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

Write-Host "=== CLEAN API WWWROOT BEFORE CLIENT BUILD ==="
New-Item -ItemType Directory -Force $ApiWwwRoot | Out-Null
Remove-Item -Recurse -Force (Join-Path $ApiWwwRoot "*") -ErrorAction SilentlyContinue

Write-Host "=== BUILD CLIENT ==="
Push-Location $ClientDir
npm install
ng build --configuration production
Pop-Location

Write-Host "=== FIND ANGULAR BUILD OUTPUT ==="

$ApiWwwRootIndex = Join-Path $ApiWwwRoot "index.html"

if (Test-Path $ApiWwwRootIndex) {
  Write-Host "Angular build already outputted to API/wwwroot."
}
else {
  $AngularIndex = Get-ChildItem -Path (Join-Path $ClientDir "dist") -Recurse -Filter "index.html" -ErrorAction SilentlyContinue | Select-Object -First 1

  if ($null -eq $AngularIndex) {
    throw "Angular index.html not found in API/wwwroot or client/dist. Check angular.json outputPath."
  }

  $AngularDist = $AngularIndex.Directory.FullName

  Write-Host "Angular build found at: $AngularDist"

  Write-Host "=== COPY ANGULAR BUILD TO API WWWROOT ==="
  Copy-Item -Recurse (Join-Path $AngularDist "*") $ApiWwwRoot
}

if (!(Test-Path (Join-Path $ApiWwwRoot "index.html"))) {
  throw "API/wwwroot/index.html is still missing after Angular build."
}

if (!(Test-Path (Join-Path $ApiWwwRoot "index.html"))) {
  throw "API/wwwroot/index.html was not copied."
}

Write-Host "=== PUBLISH API OUTSIDE PROJECT ==="
Push-Location $ApiDir
dotnet publish ".\API.csproj" -c Release -o $PublishDir
Pop-Location

if (!(Test-Path (Join-Path $PublishDir "wwwroot\index.html"))) {
  throw "Publish output is missing wwwroot/index.html."
}

Write-Host "=== PREPARE SERVER AND PRESERVE UPLOADS + CREDENTIALS ==="
ssh $Server "mkdir -p $RemoteDir/uploads/certificates $RemoteDir/uploads/images/ids $RemoteDir/uploads/images/concerns $RemoteDir/uploads/images/profiles $RemoteDir/uploads/images/officials $RemoteDir/uploads/images/signatures $RemoteDir/uploads/audio $RemoteDir/credentials"

Write-Host "=== STOP SERVICE AND CLEAN OLD APP FILES, BUT KEEP UPLOADS AND CREDENTIALS ==="
ssh $Server "systemctl stop barangay-api || true; mkdir -p $RemoteDir; find $RemoteDir -mindepth 1 -maxdepth 1 ! -name uploads ! -name credentials -exec rm -rf {} +"

Write-Host "=== DEPLOY TO SERVER ==="
scp -r "$PublishDir\*" "${Server}:$RemoteDir/"

Write-Host "=== FIX PERMISSIONS AND RESTART ==="
ssh $Server "chown -R www-data:www-data $RemoteDir; chmod -R 755 $RemoteDir/wwwroot || true; chmod -R 750 $RemoteDir/credentials || true; chmod 640 $RemoteDir/credentials/google-tts.json || true; systemctl restart barangay-api; systemctl reload nginx"

Write-Host "=== VERIFY SERVER WWWROOT ==="
ssh $Server "ls -la $RemoteDir/wwwroot/index.html"

Write-Host "=== DONE ==="