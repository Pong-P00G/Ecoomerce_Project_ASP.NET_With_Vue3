# Small E-commerce API

A RESTful API for an e-commerce platform built with ASP.NET Core 10.0, Entity Framework Core, and JWT authentication.

## Features

- User Authentication (Register, Login, Logout, Token Refresh)
- JWT Token-based Authorization
- Product Management
- Category Management
- Shopping Cart
- Order Management
- Role-based Access Control (Admin/User)
- Scalar API Documentation

## Tech Stack

- **Framework**: ASP.NET Core 10.0
- **ORM**: Entity Framework Core 10.0
- **Database**: SQL Server
- **Authentication**: JWT Bearer Tokens
- **Password Hashing**: BCrypt
- **API Documentation**: Scalar (based on OpenAPI)
- **Dependency Injection**: Built-in ASP.NET Core DI

## Prerequisites

- .NET 10.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code (optional)

## Project Structure

```
SmallEcommerceApi/
├── Controllers/          # API Controllers
│   ├── Auth/            # Authentication endpoints
│   └── Products/        # Product endpoints
├── Models/              # Data models
│   ├── Auth/           # Authentication models
│   ├── Carts/          # Shopping cart models
│   ├── Orders/         # Order models
│   ├── Payments/       # Payment models
│   ├── Products/       # Product models
│   └── Users/          # User models
├── DTOs/                # Data Transfer Objects
│   ├── Auth/           # Authentication DTOs
│   ├── Carts/          # Cart DTOs
│   └── Products/       # Product DTOs
├── Services/            # Business logic services
│   └── Interfaces/     # Service interfaces
├── Db/                  # Database context
├── Security/            # Security-related classes
├── Settings/            # Configuration settings
└── Program.cs           # Application entry point
```

## Setup Instructions

### 1. Clone or Extract the Project

Extract the SmallEcommerceApi.zip file to your desired location.

### 2. Update Connection String

Edit `appsettings.json` and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DbConnection": "YOUR_CONNECTION_STRING_HERE"
  }
}
```

**Examples:**

- **LocalDB**: `"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EcommerceProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False"`

- **SQL Server Express**: `"Data Source=localhost\\SQLEXPRESS;Initial Catalog=EcommerceProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True"`

- **Azure SQL**: `"Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=EcommerceProject;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"`

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### 5. Access API Documentation

Open your browser and navigate to:
- **Scalar UI**: `https://localhost:5001/scalar/v1`
- **OpenAPI JSON**: `https://localhost:5001/openapi/v1.json`

## Configuration

### JWT Settings

Edit `appsettings.json` to configure JWT authentication:

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "SmallEcommerceApi",
    "Audience": "SmallEcommerceClient",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

**Important**: Use a strong secret key (at least 32 characters) in production!

### CORS Settings

The API allows requests from `http://localhost:3000` (for Vue.js frontend). Edit `Program.cs` to add more origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://localhost:3000",
            "http://yourdomain.com"  // Add your domains here
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
```

## API Endpoints

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and get tokens | No |
| POST | `/api/auth/refresh` | Refresh access token | No |
| POST | `/api/auth/logout` | Logout user | Yes |
| POST | `/api/auth/revoke` | Revoke refresh token | Yes |
| GET | `/api/auth/me` | Get current user info | Yes |

### Product Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/products` | Get all products | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| POST | `/api/products` | Create product | Yes (Admin) |
| PUT | `/api/products/{id}` | Update product | Yes (Admin) |
| DELETE | `/api/products/{id}` | Delete product | Yes (Admin) |

## Testing the API

### Using Scalar UI (Recommended)

1. Start the application
2. Navigate to `https://localhost:5001/scalar/v1`
3. Use the interactive UI to test endpoints

### Using cURL

#### 1. Register a New User

```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!@#",
    "firstName": "Test",
    "lastName": "User"
  }'
```

#### 2. Login

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "testuser",
    "password": "Test123!@#"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abcd1234...",
  "expiresIn": 900
}
```

#### 3. Access Protected Endpoint

```bash
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

#### 4. Refresh Token

```bash
curl -X POST https://localhost:5001/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "YOUR_REFRESH_TOKEN_HERE"
  }'
```

### Using Postman

1. Import the OpenAPI specification from `https://localhost:5001/openapi/v1.json`
2. Create a new collection
3. Set up authentication in the collection using Bearer Token
4. Use the {{accessToken}} variable for authenticated requests

## Database

The database is automatically created when you run the application in Development mode using `EnsureCreated()`. For production, use migrations:

### Create Initial Migration

```bash
dotnet ef migrations add InitialCreate
```

### Update Database

```bash
dotnet ef database update
```

### View Database Schema

The database includes the following main tables:

- **Users**: User accounts
- **UserProfiles**: Additional user information
- **RefreshTokens**: JWT refresh tokens
- **Products**: Product catalog
- **Categories**: Product categories
- **ProductVariants**: Product variations (size, color, etc.)
- **Carts**: Shopping carts
- **CartItems**: Items in carts
- **Orders**: Customer orders
- **OrderItems**: Items in orders
- **Payments**: Payment transactions

## Security

### Password Security
- Passwords are hashed using BCrypt with salt rounds
- Minimum password requirements can be configured

### JWT Tokens
- Access tokens expire after 15 minutes (configurable)
- Refresh tokens expire after 7 days (configurable)
- Tokens are signed with HMAC-SHA256
- Refresh tokens are stored securely in the database

### HTTPS
- The application enforces HTTPS redirection
- Certificate validation can be configured in appsettings

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to database

**Solution**:
1. Verify SQL Server is running
2. Check connection string in appsettings.json
3. Ensure your SQL Server allows TCP/IP connections
4. For LocalDB, ensure it's installed: `sqllocaldb info`

### JWT Token Issues

**Problem**: "Unauthorized" error when accessing protected endpoints

**Solution**:
1. Ensure the Bearer token is included in the Authorization header
2. Check if the token has expired
3. Verify the JWT SecretKey in appsettings.json is at least 32 characters
4. Use the /api/auth/refresh endpoint to get a new token

### Package Restore Issues

**Problem**: NuGet packages won't restore

**Solution**:
```bash
dotnet clean
dotnet restore --force
dotnet build
```

### Port Already in Use

**Problem**: "Address already in use" error

**Solution**:
1. Change ports in `Properties/launchSettings.json`
2. Or kill the process using the port:
   - Windows: `netstat -ano | findstr :5001` then `taskkill /F /PID <PID>`
   - Linux/Mac: `lsof -ti:5001 | xargs kill`

## Development

### Adding New Endpoints

1. Create a new controller in the `Controllers` folder
2. Define your DTOs in the `DTOs` folder
3. Implement business logic in `Services`
4. Add service registration in `Program.cs`

### Adding New Models

1. Create model classes in the `Models` folder
2. Add DbSet to `AppDbContext.cs`
3. Create and apply migration

## Building for Production

### Publish the Application

```bash
dotnet publish -c Release -o ./publish
```

### Environment-Specific Settings

Create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DbConnection": "YOUR_PRODUCTION_CONNECTION_STRING"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_PRODUCTION_SECRET_KEY"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Fixed Issues

The following issues were corrected in this version:

1. **Namespace Issue**: Fixed nested namespace in `ClaimTypesCustom.cs` that caused compilation error
2. **Package Compatibility**: Ensured all packages are compatible with .NET 10.0
3. **Connection String**: Updated to use LocalDB for easier local development
4. **JWT Secret**: Added proper length secret key

## License

This project is for educational purposes.

## Support

For issues or questions, please check the API documentation at `/scalar/v1` when the application is running.
