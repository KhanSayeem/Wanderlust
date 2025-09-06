# TourismApp

An ASP.NET Core MVC application for managing tour packages, bookings, and user profiles.

## Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- SQL Server instance (default connection string targets `localhost`)
- Optional: [dotnet-ef](https://learn.microsoft.com/ef/core/cli/dotnet) tool for applying migrations

## Setup

1. Update the connection string in `appsettings.json` to point to your SQL Server.  
   Example configuration:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TourismDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
   ```

   The application reads this value in `Program.cs` when setting up Entity Framework Core:

   ```csharp
   var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(connectionString));
   ```
2. Restore and build the project:

   ```bash
   dotnet build
   ```
3. Apply migrations (requires `dotnet-ef`):

   ```bash
   dotnet ef database update
   ```
4. Run the application:

   ```bash
   dotnet run
   ```

The site will start on `https://localhost:5001` (or a port shown in the console).

## Project Structure

- `Controllers/` – MVC controllers
- `Data/` – Entity Framework Core context and seed data
- `Models/` – domain models
- `Views/` – Razor views
- `Migrations/` – EF Core migrations

## Notes

Seed data for roles and demo users is created automatically at startup.
