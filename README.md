# WanderLust - Tour and Travel Management System

A comprehensive ASP.NET Core web application for managing tourism operations, enabling travel agencies to create and manage tour packages while allowing tourists to browse, book, and manage their travel experiences.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [User Roles](#user-roles)
- [Key Functionalities](#key-functionalities)
- [API Endpoints](#api-endpoints)
- [Models](#models)
- [Contributing](#contributing)
- [License](#license)

## Overview

TourismApp is a full-featured tourism management platform that facilitates the entire tourism ecosystem. Travel agencies can create, manage, and monitor their tour packages, while tourists can discover, book, and manage their travel experiences. The application includes comprehensive booking management, payment tracking, feedback systems, and detailed reporting capabilities.

## Features

### For Travel Agencies
- ✅ **Profile Management**: Complete agency profile with business information and branding
- ✅ **Tour Package Management**: Create, edit, and delete tour packages with multiple dates
- ✅ **Booking Management**: Monitor and manage customer bookings with status tracking
- ✅ **Payment Tracking**: Track payment status for all bookings
- ✅ **Capacity Management**: Set and monitor group size limits for tour packages
- ✅ **Revenue Reporting**: Access detailed reports on bookings and revenue
- ✅ **Image Upload**: Upload and manage tour package and profile images

### For Tourists
- ✅ **Tour Discovery**: Browse and search available tour packages
- ✅ **Booking System**: Book tours with participant count and date selection
- ✅ **Booking History**: View and manage personal booking history
- ✅ **Profile Management**: Maintain personal profile information
- ✅ **Feedback System**: Provide feedback after completing tours
- ✅ **Booking Cancellation**: Cancel bookings when permitted

### General Features
- 🔐 **Role-Based Authentication**: Separate access levels for tourists and agencies
- 🎨 **Responsive Design**: Mobile-friendly interface using Bootstrap
- 📊 **Dashboard Analytics**: Homepage with key statistics and popular tours
- 🔍 **Search Functionality**: Search tours by title and description
- 📱 **Image Management**: Support for tour package and profile images
- 🗄️ **Database Integration**: Entity Framework Core with SQL Server

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Language**: C# with nullable reference types enabled
- **Database**: SQL Server with Entity Framework Core 9.0.8
- **Authentication**: ASP.NET Core Identity with role-based authorization
- **Frontend**: Razor Pages with Bootstrap CSS framework
- **ORM**: Entity Framework Core with Code-First migrations
- **Development Tools**: Visual Studio 2022, SQL Server Express

### Key Dependencies

```xml
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="9.0.8" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.8" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="9.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.8" />
```

## Project Structure

```
TouristApp/
├── Controllers/                        # MVC Controllers
│   ├── HomeController.cs              # Homepage and dashboard
│   ├── ToursController.cs             # Tour package management
│   ├── BookingsController.cs          # Booking operations
│   ├── ProfileController.cs           # User profile management
│   ├── FeedbacksController.cs         # Feedback system
│   └── ReportsController.cs           # Reporting functionality
├── Models/                             # Data models
│   ├── ApplicationUser.cs             # Extended identity user
│   ├── TourPackage.cs                # Tour package entity
│   ├── Booking.cs                    # Booking entity
│   ├── AgencyProfile.cs              # Agency profile data
│   ├── TouristProfile.cs             # Tourist profile data
│   ├── TourDate.cs                   # Available tour dates
│   ├── Amenity.cs                    # Tour amenities
│   ├── Feedback.cs                   # Customer feedback
│   ├── Enums.cs                      # Booking and payment status enums
│   └── ViewModels/                   # View-specific models
├── Views/                              # Razor view templates
│   ├── Home/                          # Homepage views
│   ├── Tours/                         # Tour-related views
│   ├── Bookings/                      # Booking views
│   ├── Profile/                       # Profile management views
│   ├── Reports/                       # Reporting views
│   └── Shared/                        # Shared layout and partials
├── Data/                              # Data access layer
│   ├── ApplicationDbContext.cs        # EF Core context
│   └── SeedData.cs                   # Database seeding
├── Migrations/                        # EF Core migrations
├── wwwroot/                           # Static files (CSS, JS, images)
├── Areas/                             # Identity UI areas
├── bin/                               # Compiled binaries (excluded from git)
├── obj/                               # Build artifacts (excluded from git)
├── Program.cs                         # Application startup
├── appsettings.json                  # Configuration
├── appsettings.Development.json      # Development settings (excluded from git)
├── TourismApp.csproj                 # Project file
├── TourApp.sln                       # Solution file
├── .gitignore                        # Git ignore rules
└── README.md                         # This file
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server
- [SQL Server Management Studio](https://learn.microsoft.com/en-us/ssms/install/install) (set it up and then move on to next steps)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or any .NET-compatible IDE
- [dotnet-ef](https://www.nuget.org/packages/dotnet-ef)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/TouristApp.git
   cd TouristApp
   ```

2. **Stay in the project root directory** (TouristApp/)

3. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

4. **Update the connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TourismDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
   ```

5. **Create and apply database migrations:**
   ```bash
   dotnet ef database update
   ```

6. **Build and run the application:**
   ```bash
   dotnet build TourApp.sln
   dotnet run --project TourismApp.csproj
   ```
   
   **Alternative build commands:**
   ```bash
   # Build using solution file
   dotnet build TourApp.sln
   
   # Build using project file directly
   dotnet build TourismApp.csproj
   ```
   **Alternative run commands**
   ```bash
   dotnet run
   ```

7. **Open your browser** and navigate to `https://localhost:5001` or the URL shown in the terminal.

## Database Setup

The application uses Entity Framework Core with automatic database seeding. The `SeedData.cs` file initializes:

- Default user roles (Tourist, Agency)
- Sample users for testing
- Initial data for demonstration

### Default Test Accounts

After running the application for the first time, you'll have access to seeded test accounts. Check the `SeedData.cs` file for specific credentials.

## User Roles

### Tourist Role
- Browse and search tour packages
- Create and manage bookings
- View booking history
- Manage personal profile
- Provide feedback on completed tours
- Cancel bookings (when allowed)

### Agency Role
- Create and manage tour packages
- Set tour dates and capacity limits
- Monitor and manage customer bookings
- Update booking and payment statuses
- Access booking and revenue reports
- Manage agency profile and branding

## Key Functionalities

### Tour Package Management
- **Create Tours**: Agencies can create detailed tour packages with descriptions, pricing, duration, and capacity limits
- **Multiple Dates**: Each tour package can have multiple available dates
- **Image Support**: Upload and manage tour package images
- **Amenities**: Associate amenities with tour packages

### Booking System
- **Real-time Availability**: Check tour capacity and availability
- **Participant Management**: Specify number of participants per booking
- **Status Tracking**: Complete booking lifecycle management
- **Payment Integration**: Track payment status throughout the process

### Reporting and Analytics
- **Dashboard Statistics**: View key metrics on the homepage
- **Agency Reports**: Detailed booking and revenue reports for agencies
- **Popular Tours**: Track and display most booked tour packages

## API Endpoints

### Tours
- `GET /Tours` - Browse all tour packages
- `GET /Tours/Details/{id}` - View tour package details
- `GET /Tours/MyPackages` - View agency's tour packages (Agency role)
- `POST /Tours/Create` - Create new tour package (Agency role)
- `POST /Tours/Edit/{id}` - Edit tour package (Agency role)
- `POST /Tours/Delete/{id}` - Delete tour package (Agency role)

### Bookings
- `GET /Bookings` - View user's bookings (Tourist role)
- `GET /Bookings/Manage` - Manage agency bookings (Agency role)
- `POST /Bookings/Create` - Create new booking (Tourist role)
- `POST /Bookings/Cancel/{id}` - Cancel booking
- `POST /Bookings/SetStatus/{id}` - Update booking status (Agency role)
- `POST /Bookings/SetPayment/{id}` - Update payment status (Agency role)

### Profile
- `GET /Profile/Tourist` - Tourist profile form
- `POST /Profile/Tourist` - Update tourist profile
- `GET /Profile/Agency` - Agency profile form
- `POST /Profile/Agency` - Update agency profile

## Models

### Core Entities

**TourPackage**: Represents tour packages with pricing, duration, and capacity information
**Booking**: Manages tour bookings with status and payment tracking
**ApplicationUser**: Extended identity user supporting both tourist and agency roles
**TourDate**: Available dates for each tour package
**AgencyProfile**: Business information for travel agencies
**TouristProfile**: Personal information for tourists
**Feedback**: Customer feedback and ratings system

### Enums

```csharp
public enum BookingStatus
{
    Pending,
    Confirmed, 
    Completed,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded,
    Unpaid
}
```

## Configuration

### Password Policy (Development)
- Minimum length: 6 characters
- No uppercase requirement
- No non-alphanumeric requirement
- No email confirmation required

### Database Configuration
The application is configured to use SQL Server with the following features:
- Integrated Security
- Multiple Active Result Sets
- TrustServerCertificate for development

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Future Enhancements

- Payment gateway integration
- Email notification system
- Advanced search and filtering
- Mobile application
- Multi-language support
- Real-time chat support
- Integration with external booking platforms
- Advanced reporting and analytics

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, email your-email@example.com or create an issue in the GitHub repository.

---

**Note**: This application is designed for educational and demonstration purposes. For production deployment, additional security measures, error handling, and scalability considerations should be implemented.
