#!/bin/bash

# Test Script for Small E-commerce API
# This script tests the main endpoints of the API

BASE_URL="${1:-https://localhost:5001}"

echo -e "\033[0;32mTesting Small E-commerce API at $BASE_URL\033[0m"
echo -e "\033[0;32m=============================================\033[0m"
echo ""

# Generate random numbers for unique users
RANDOM_NUM=$RANDOM

# Test 1: Register a new user
echo -e "\033[0;36m1. Testing User Registration...\033[0m"

REGISTER_DATA=$(cat <<EOF
{
  "username": "testuser_$RANDOM_NUM",
  "email": "test${RANDOM_NUM}@example.com",
  "password": "Test123!@#",
  "firstName": "Test",
  "lastName": "User"
}
EOF
)

REGISTER_RESPONSE=$(curl -k -s -X POST "$BASE_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "$REGISTER_DATA")

if echo "$REGISTER_RESPONSE" | grep -q "accessToken"; then
    echo -e "\033[0;32m✓ Registration successful!\033[0m"
    ACCESS_TOKEN=$(echo "$REGISTER_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
    echo -e "\033[0;90m  Access Token: ${ACCESS_TOKEN:0:20}...\033[0m"
else
    echo -e "\033[0;31m✗ Registration failed!\033[0m"
    echo "$REGISTER_RESPONSE"
fi
echo ""

# Test 2: Login
echo -e "\033[0;36m2. Testing User Login...\033[0m"

USERNAME=$(echo "$REGISTER_DATA" | grep -o '"username":"[^"]*' | cut -d'"' -f4)

LOGIN_DATA=$(cat <<EOF
{
  "usernameOrEmail": "$USERNAME",
  "password": "Test123!@#"
}
EOF
)

LOGIN_RESPONSE=$(curl -k -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "$LOGIN_DATA")

if echo "$LOGIN_RESPONSE" | grep -q "accessToken"; then
    echo -e "\033[0;32m✓ Login successful!\033[0m"
    ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
    REFRESH_TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"refreshToken":"[^"]*' | cut -d'"' -f4)
    echo -e "\033[0;90m  Access Token: ${ACCESS_TOKEN:0:20}...\033[0m"
    echo -e "\033[0;90m  Refresh Token: ${REFRESH_TOKEN:0:20}...\033[0m"
else
    echo -e "\033[0;31m✗ Login failed!\033[0m"
    echo "$LOGIN_RESPONSE"
    exit 1
fi
echo ""

# Test 3: Get current user
echo -e "\033[0;36m3. Testing Get Current User (Protected Endpoint)...\033[0m"

ME_RESPONSE=$(curl -k -s -X GET "$BASE_URL/api/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

if echo "$ME_RESPONSE" | grep -q "username"; then
    echo -e "\033[0;32m✓ Successfully retrieved user info!\033[0m"
    echo -e "\033[0;90m  Username: $(echo "$ME_RESPONSE" | grep -o '"username":"[^"]*' | cut -d'"' -f4)\033[0m"
    echo -e "\033[0;90m  Email: $(echo "$ME_RESPONSE" | grep -o '"email":"[^"]*' | cut -d'"' -f4)\033[0m"
    echo -e "\033[0;90m  Role: $(echo "$ME_RESPONSE" | grep -o '"role":"[^"]*' | cut -d'"' -f4)\033[0m"
else
    echo -e "\033[0;31m✗ Failed to retrieve user info!\033[0m"
    echo "$ME_RESPONSE"
fi
echo ""

# Test 4: Refresh token
echo -e "\033[0;36m4. Testing Token Refresh...\033[0m"
sleep 2  # Wait a bit before refreshing

REFRESH_DATA=$(cat <<EOF
{
  "refreshToken": "$REFRESH_TOKEN"
}
EOF
)

REFRESH_RESPONSE=$(curl -k -s -X POST "$BASE_URL/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d "$REFRESH_DATA")

if echo "$REFRESH_RESPONSE" | grep -q "accessToken"; then
    echo -e "\033[0;32m✓ Token refresh successful!\033[0m"
    NEW_ACCESS_TOKEN=$(echo "$REFRESH_RESPONSE" | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
    echo -e "\033[0;90m  New Access Token: ${NEW_ACCESS_TOKEN:0:20}...\033[0m"
    ACCESS_TOKEN=$NEW_ACCESS_TOKEN
else
    echo -e "\033[0;31m✗ Token refresh failed!\033[0m"
    echo "$REFRESH_RESPONSE"
fi
echo ""

# Test 5: Logout
echo -e "\033[0;36m5. Testing Logout...\033[0m"

LOGOUT_RESPONSE=$(curl -k -s -X POST "$BASE_URL/api/auth/logout" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

if echo "$LOGOUT_RESPONSE" | grep -q "message"; then
    echo -e "\033[0;32m✓ Logout successful!\033[0m"
    echo -e "\033[0;90m  Message: $(echo "$LOGOUT_RESPONSE" | grep -o '"message":"[^"]*' | cut -d'"' -f4)\033[0m"
else
    echo -e "\033[0;31m✗ Logout failed!\033[0m"
    echo "$LOGOUT_RESPONSE"
fi
echo ""

# Test 6: Try to access protected endpoint after logout
echo -e "\033[0;36m6. Testing Access After Logout (Should Fail)...\033[0m"

AFTER_LOGOUT_RESPONSE=$(curl -k -s -X GET "$BASE_URL/api/auth/me" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

if echo "$AFTER_LOGOUT_RESPONSE" | grep -q "username"; then
    echo -e "\033[0;31m✗ Unexpected success - token should be invalid!\033[0m"
else
    echo -e "\033[0;32m✓ Correctly rejected - token is invalid after logout!\033[0m"
fi
echo ""

echo -e "\033[0;32m=============================================\033[0m"
echo -e "\033[0;32mAPI Testing Complete!\033[0m"
echo ""
echo -e "\033[0;33mNext Steps:\033[0m"
echo -e "\033[0;33m1. Open Scalar UI: $BASE_URL/scalar/v1\033[0m"
echo -e "\033[0;33m2. View OpenAPI spec: $BASE_URL/openapi/v1.json\033[0m"
echo -e "\033[0;33m3. Check the README.md for more testing examples\033[0m"
