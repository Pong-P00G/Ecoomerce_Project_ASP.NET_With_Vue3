# Test Script for Small E-commerce API
# This script tests the main endpoints of the API

param(
    [string]$BaseUrl = "https://localhost:5001"
)

Write-Host "Testing Small E-commerce API at $BaseUrl" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""

# Function to make HTTP requests
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [object]$Body = $null,
        [string]$Token = $null
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    $params = @{
        Method = $Method
        Uri = "$BaseUrl$Endpoint"
        Headers = $headers
    }
    
    if ($Body) {
        $params["Body"] = ($Body | ConvertTo-Json)
    }
    
    # Skip certificate validation for localhost testing
    if ($PSVersionTable.PSVersion.Major -ge 6) {
        $params["SkipCertificateCheck"] = $true
    }
    
    try {
        $response = Invoke-RestMethod @params
        return $response
    } catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Response: $($_.ErrorDetails.Message)" -ForegroundColor Red
        return $null
    }
}

# Test 1: Register a new user
Write-Host "1. Testing User Registration..." -ForegroundColor Cyan
$registerData = @{
    username = "testuser_$(Get-Random -Maximum 10000)"
    email = "test$(Get-Random -Maximum 10000)@example.com"
    password = "Test123!@#"
    firstName = "Test"
    lastName = "User"
    roleId = 2
}

$registerResponse = Invoke-ApiRequest -Method "POST" -Endpoint "/api/auth/register" -Body $registerData

if ($registerResponse) {
    Write-Host "✓ Registration successful!" -ForegroundColor Green
    Write-Host "  Access Token: $($registerResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
} else {
    Write-Host "✗ Registration failed!" -ForegroundColor Red
}
Write-Host ""

# Test 2: Login
Write-Host "2. Testing User Login..." -ForegroundColor Cyan
$loginData = @{
    usernameOrEmail = $registerData.username
    password = $registerData.password
}

$loginResponse = Invoke-ApiRequest -Method "POST" -Endpoint "/api/auth/login" -Body $loginData

if ($loginResponse) {
    Write-Host "✓ Login successful!" -ForegroundColor Green
    $accessToken = $loginResponse.accessToken
    $refreshToken = $loginResponse.refreshToken
    Write-Host "  Access Token: $($accessToken.Substring(0, 20))..." -ForegroundColor Gray
    Write-Host "  Refresh Token: $($refreshToken.Substring(0, 20))..." -ForegroundColor Gray
} else {
    Write-Host "✗ Login failed!" -ForegroundColor Red
    exit
}
Write-Host ""

# Test 3: Get current user
Write-Host "3. Testing Get Current User (Protected Endpoint)..." -ForegroundColor Cyan
$meResponse = Invoke-ApiRequest -Method "GET" -Endpoint "/api/auth/me" -Token $accessToken

if ($meResponse) {
    Write-Host "✓ Successfully retrieved user info!" -ForegroundColor Green
    Write-Host "  Username: $($meResponse.username)" -ForegroundColor Gray
    Write-Host "  Email: $($meResponse.email)" -ForegroundColor Gray
    Write-Host "  Role: $($meResponse.role)" -ForegroundColor Gray
} else {
    Write-Host "✗ Failed to retrieve user info!" -ForegroundColor Red
}
Write-Host ""

# Test 4: Refresh token
Write-Host "4. Testing Token Refresh..." -ForegroundColor Cyan
Start-Sleep -Seconds 2  # Wait a bit before refreshing

$refreshData = @{
    refreshToken = $refreshToken
}

$refreshResponse = Invoke-ApiRequest -Method "POST" -Endpoint "/api/auth/refresh" -Body $refreshData

if ($refreshResponse) {
    Write-Host "✓ Token refresh successful!" -ForegroundColor Green
    Write-Host "  New Access Token: $($refreshResponse.accessToken.Substring(0, 20))..." -ForegroundColor Gray
    $accessToken = $refreshResponse.accessToken
} else {
    Write-Host "✗ Token refresh failed!" -ForegroundColor Red
}
Write-Host ""

# Test 5: Logout
Write-Host "5. Testing Logout..." -ForegroundColor Cyan
$logoutResponse = Invoke-ApiRequest -Method "POST" -Endpoint "/api/auth/logout" -Token $accessToken

if ($logoutResponse) {
    Write-Host "✓ Logout successful!" -ForegroundColor Green
    Write-Host "  Message: $($logoutResponse.message)" -ForegroundColor Gray
} else {
    Write-Host "✗ Logout failed!" -ForegroundColor Red
}
Write-Host ""

# Test 6: Try to access protected endpoint after logout
Write-Host "6. Testing Access After Logout (Should Fail)..." -ForegroundColor Cyan
$afterLogoutResponse = Invoke-ApiRequest -Method "GET" -Endpoint "/api/auth/me" -Token $accessToken

if ($afterLogoutResponse) {
    Write-Host "✗ Unexpected success - token should be invalid!" -ForegroundColor Red
} else {
    Write-Host "✓ Correctly rejected - token is invalid after logout!" -ForegroundColor Green
}
Write-Host ""

Write-Host "=============================================" -ForegroundColor Green
Write-Host "API Testing Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Open Scalar UI: $BaseUrl/scalar/v1" -ForegroundColor Yellow
Write-Host "2. View OpenAPI spec: $BaseUrl/openapi/v1.json" -ForegroundColor Yellow
Write-Host "3. Check the README.md for more testing examples" -ForegroundColor Yellow
