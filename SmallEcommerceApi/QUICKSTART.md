# Quick Start Guide - Small E-commerce API

## Prerequisites Check

Before you begin, ensure you have:

- [ ] **.NET 10.0 SDK** installed
- [ ] **SQL Server** (LocalDB, Express, or Full) installed
- [ ] **Text editor** (Visual Studio, VS Code, or any IDE)
- [ ] **Terminal/Command Prompt** access

### Installing Prerequisites

#### Install .NET 10.0 SDK

**Windows / Mac / Linux**:
1. Visit: https://dotnet.microsoft.com/download/dotnet/10.0
2. Download and install the SDK (not just the runtime)
3. Verify installation:
   ```bash
   dotnet --version
   ```
   You should see: `10.0.x` (where x is the patch version)

#### Install SQL Server

**Option 1: SQL Server LocalDB (Recommended for Development - Windows only)**
- Already included with Visual Studio 2022
- Or download: https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb

**Option 2: SQL Server Express (Cross-platform)**
- Windows: https://www.microsoft.com/sql-server/sql-server-downloads
- Linux: https://learn.microsoft.com/sql/linux/quickstart-install-connect-ubuntu
- Mac: Use Docker: `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

**Option 3: Docker (All platforms)**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrongPassword123!" \
  -p 1433:1433 --name sql1 -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## 5-Minute Setup

### Step 1: Extract the Project
```bash
# Extract the corrected SmallEcommerceApi to a folder
cd path/to/SmallEcommerceApi
```

### Step 2: Review Configuration
Open `appsettings.json` and verify/update the connection string if needed:

```json
{
  "ConnectionStrings": {
    "DbConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcommerceProject;..."
  }
}
```

**Connection String Examples:**

**For LocalDB (Windows - Default)**:
```
Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcommerceProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False
```

**For SQL Express (Windows)**:
```
Server=localhost\\SQLEXPRESS;Database=EcommerceProject;Trusted_Connection=True;TrustServerCertificate=True;
```

**For Docker / Remote SQL Server**:
```
Server=localhost,1433;Database=EcommerceProject;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;
```

### Step 3: Restore Dependencies
```bash
dotnet restore
```

You should see:
```
Restore succeeded.
```

### Step 4: Build the Project
```bash
dotnet build
```

You should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Step 5: Run the Application
```bash
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

**✅ If you see this, your API is running!**

---

## Testing the API

### Method 1: Using Scalar UI (Easiest - Recommended)

1. Keep the application running
2. Open your browser
3. Navigate to: **https://localhost:5001/scalar/v1**
4. You'll see the Scalar API Documentation interface
5. Click on any endpoint to test it
6. Use the "Try it out" button to make requests

**Testing Authentication Flow**:
1. Find **POST /api/auth/register** endpoint
2. Click "Try it out"
3. Use this example data:
   ```json
   {
     "username": "testuser",
     "email": "test@example.com",
     "password": "Test123!@#",
     "firstName": "Test",
     "lastName": "User"
   }
   ```
4. Click "Execute"
5. Copy the `accessToken` from the response
6. Click the "Authorize" button at the top
7. Enter: `Bearer <your-access-token>`
8. Now you can test protected endpoints!

### Method 2: Using Automated Test Scripts

**Windows (PowerShell)**:
```powershell
.\test-api.ps1
```

**Linux/Mac (Bash)**:
```bash
chmod +x test-api.sh
./test-api.sh
```

These scripts will automatically:
- Register a new user
- Login and get tokens
- Access protected endpoints
- Test token refresh
- Test logout

### Method 3: Using cURL

**Register a user**:
```bash
curl -k -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

**Login**:
```bash
curl -k -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "johndoe",
    "password": "SecurePass123!"
  }'
```

**Get current user** (replace YOUR_TOKEN):
```bash
curl -k -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Common Issues & Solutions

### Issue 1: "dotnet: command not found"
**Solution**: Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

### Issue 2: "Cannot connect to database"
**Solutions**:
1. Verify SQL Server is running:
   - LocalDB: `sqllocaldb info`
   - SQL Express: Check Services (services.msc)
   - Docker: `docker ps | grep sql`

2. Test connection string:
   ```bash
   # For LocalDB
   sqllocaldb start MSSQLLocalDB
   ```

3. Update `appsettings.json` with correct connection string

### Issue 3: "Port 5001 already in use"
**Solutions**:

**Option A**: Change the port in `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:7000"
}
```

**Option B**: Kill the process using the port:
```bash
# Windows
netstat -ano | findstr :5001
taskkill /F /PID <process-id>

# Linux/Mac
lsof -ti:5001 | xargs kill
```

### Issue 4: Build Errors
**Solution**: Clean and rebuild:
```bash
dotnet clean
dotnet restore --force
dotnet build
```

### Issue 5: SSL Certificate Issues in Browser
**Solution**: 
1. Trust the development certificate:
   ```bash
   dotnet dev-certs https --trust
   ```
2. Or use HTTP instead: `http://localhost:5000/scalar/v1`

### Issue 6: "Unauthorized" when testing endpoints
**Solutions**:
1. Ensure you registered/logged in first
2. Copy the access token correctly
3. Add "Bearer " prefix: `Bearer eyJhbGciOiJ...`
4. Check token hasn't expired (15 min default)
5. Use refresh token to get new access token

---

## Next Steps

### 1. Explore the API
- Open Scalar UI: `https://localhost:5001/scalar/v1`
- View all available endpoints
- Test different authentication scenarios
- Try creating products (requires admin role)

### 2. Read the Documentation
- `README.md` - Complete API documentation
- `FIXES.md` - See what was fixed in the code
- Review the code structure in different folders

### 3. Customize the Project
- Modify JWT settings (token expiration, secret key)
- Add new endpoints
- Customize the database schema
- Add validation rules

### 4. Development Workflow

**Making Changes**:
1. Stop the app (Ctrl+C)
2. Make your code changes
3. Run: `dotnet build`
4. Fix any errors
5. Run: `dotnet run`
6. Test your changes

**Database Changes**:
1. Modify models in `Models/` folder
2. Create migration: `dotnet ef migrations add YourMigrationName`
3. Apply migration: `dotnet ef database update`

---

## Project Structure Overview

```
SmallEcommerceApi/
├── Controllers/              # API Endpoints
│   ├── Auth/                # Login, Register, etc.
│   └── Products/            # Product management
├── Services/                # Business Logic
│   ├── AuthService.cs      # Auth logic
│   ├── UserService.cs      # User management
│   └── Interfaces/         # Service contracts
├── Models/                  # Database entities
│   ├── Users/              # User models
│   ├── Products/           # Product models
│   └── Orders/             # Order models
├── DTOs/                    # Data Transfer Objects
├── Db/                      # Database context
├── Security/                # Auth helpers
└── Program.cs               # Application entry
```

---

## Useful Commands

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Run with auto-reload on file changes
dotnet watch run

# Create a migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove

# View project info
dotnet --info

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore
```

---

## Getting Help

If you encounter issues:

1. Check `FIXES.md` for known issues and solutions
2. Review `README.md` for detailed documentation
3. Enable detailed logging in `appsettings.json`:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft.AspNetCore": "Information"
       }
     }
   }
   ```
4. Check the console output for error messages

---

## Production Deployment

Before deploying to production:

1. ✅ Change JWT secret key to a strong random value
2. ✅ Update connection string to production database
3. ✅ Set environment to Production
4. ✅ Enable proper logging
5. ✅ Set up HTTPS with valid certificates
6. ✅ Configure CORS for your frontend domain
7. ✅ Enable rate limiting
8. ✅ Set up monitoring and alerts

---

**🎉 Congratulations! You're ready to use the Small E-commerce API!**

For detailed API documentation, endpoints, and examples, see `README.md`.
