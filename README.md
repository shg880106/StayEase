# StayEase 🏠

A modern property rental platform inspired by Airbnb, built with .NET 8 backend and Angular 18 frontend, featuring JWT authentication and comprehensive booking management.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🎯 Overview

StayEase is a full-stack property rental application that allows users to list properties, search for accommodations, make bookings, and leave reviews. The platform is built following Clean Architecture principles and implements modern security practices with JWT authentication.

## ✨ Features

### Authentication & Authorization
- User registration and login with JWT tokens
- Secure password hashing
- Token-based authentication for protected endpoints
- Role-based authorization (Guest/Owner perspectives)

### Property Management
- Browse all available properties
- View detailed property information
- Create new property listings (authenticated users)
- Delete owned properties with soft delete
- Property ownership validation
- Image support for property listings

### Booking System
- Create bookings with date validation
- View personal booking history
- Get detailed booking information
- Owner's perspective for property bookings
- Guest details for property owners
- Automatic total price calculation
- Booking overlap prevention
- Status tracking (Pending, Confirmed, Cancelled)

### Review System (not yet)
- Leave reviews for properties
- Rating system (1-5 stars)
- Comment functionality
- User and property association

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns.


## 🛠️ Tech Stack

### Backend
- **.NET 8** - Modern web framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 8.0.5** - ORM
- **SQL Server** - Relational database
- **JWT Bearer Authentication** - Secure token-based auth
- **Swashbuckle 6.5.0** - API documentation (Swagger)
- **BCrypt** - Password hashing

### Frontend
- **Angular 18** - Modern SPA framework
- **TypeScript** - Type-safe JavaScript
- **Ionic Framework** - UI components
- **RxJS** - Reactive programming
- **Angular Router** - Navigation
- **HttpClient** - API communication

### DevOps & Deployment
- **Azure App Service** - Backend hosting
- **Azure Static Web Apps** - Frontend hosting
- **GitHub Actions** - CI/CD pipelines
- **Azure SQL Database** - Production database

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code
- Git

### Backend Setup

1. **Clone the repository**
   git clone https://github.com/shg880106/StayEase.git cd StayEase/backend

2. **Configure database connection**   
   Update `appsettings.json` in `StayEaseApp.API`:
   { "ConnectionStrings": { "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=StayEaseDB;Trusted_Connection=true;" }, "Jwt": { "SecretKey": "your-secret-key-min-32-characters", "Issuer": "StayEaseApp", "Audience": "StayEaseApp", "ExpiryMinutes": 60 } }
   
3. **Applydatabase migrations**
   cd src/StayEaseApp.API dotnet ef database update

4. **Run the API**
   dotnet run
   
API will be available at: `https://localhost:7141`
   Swagger UI: `https://localhost:7141/swagger`

### Frontend Setup

1. **Navigate to frontend directory**
   cd frontend/stayease-app
   
2. **Install dependencies**
   npm install

3. **Configure API endpoint**
   Update `src/environments/environment.ts`:
   export const environment = { production: false, apiUrl: 'https://localhost:7141/api' };
   
4. **Run the application**
   npm start
   
App will be available at: `http://localhost:4200`

## 📚 API Documentation

### Authentication Endpoints

#### Register
POST /api/auth/register Content-Type: application/json
{ "name": "John Doe", "email": "john@example.com", "password": "SecurePass123!" }

#### Login
POST /api/auth/login Content-Type: application/json
{ "email": "john@example.com", "password": "SecurePass123!" }

#### Get Current User
GET /api/auth/me Authorization: Bearer {token}

### Property Endpoints

#### Get All Properties
GET /api/property

#### Get Property by ID
GET /api/property/{propertyId}

#### Create Property (Auth Required)
POST /api/property Authorization: Bearer {token} Content-Type: application/json
{ "title": "Cozy Beach House", "description": "Beautiful oceanfront property", "pricePerNight": 150.00, "location": "Miami Beach, FL", "maxGuests": 4, "imageUrl": "https://example.com/image.jpg" }

#### Delete Property (Auth Required)
DELETE /api/property/{propertyId} Authorization: Bearer {token}

### Booking Endpoints (All Auth Required)

#### Create Booking
POST /api/booking Authorization: Bearer {token} Content-Type: application/json
{ "propertyID": "guid", "startDate": "2026-06-01T00:00:00Z", "endDate": "2026-06-05T00:00:00Z" }

#### Get My Bookings
GET /api/booking/my-bookings Authorization: Bearer {token}

#### Get Booking Details
GET /api/booking/{bookingId} Authorization: Bearer {token}

#### Get Booking Details (Owner Perspective)
GET /api/booking/{bookingId}/owner Authorization: Bearer {token}

### Response Status Codes

- `200 OK` - Success
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid input or validation error
- `401 Unauthorized` - Missing or invalid authentication
- `403 Forbidden` - User doesn't have permission
- `404 Not Found` - Resource not found

## 🗄️ Database Schema

### Users Table
- `UserID` (PK, Guid)
- `Name` (string)
- `Email` (string, unique)
- `PasswordHash` (string)
- `CreatedAt` (DateTime)
- `IsActive` (bool)

### Properties Table
- `PropertyID` (PK, Guid)
- `OwnerID` (FK → Users)
- `Title` (string)
- `Description` (string)
- `PricePerNight` (decimal)
- `Location` (string)
- `MaxGuests` (int)
- `ImageUrl` (string, nullable)
- `IsDeleted` (bool)
- `DeletedAt` (DateTime, nullable)

### Bookings Table
- `BookingID` (PK, Guid)
- `PropertyID` (FK → Properties)
- `UserID` (FK → Users)
- `StartDate` (DateTime)
- `EndDate` (DateTime)
- `TotalPrice` (decimal)
- `BookingStatus` (enum: Pending, Confirmed, Cancelled, Completed)

### Reviews Table
- `ReviewID` (PK, Guid)
- `PropertyID` (FK → Properties)
- `UserID` (FK → Users)
- `Rating` (int, 1-5)
- `Comment` (string)
- `CreatedAt` (DateTime)

## 🌐 Deployment

### Production URLs (not yet)
- **Backend API**: https://stayease-webapp-shg-gjdve9gcghgwbqc7.westeurope-01.azurewebsites.net
- **Frontend**: https://salmon-water-06fe6a403.7.azurestaticapps.net

### Deployment Architecture
- Backend: Azure App Service (West Europe)
- Frontend: Azure Static Web Apps
- Database: Azure SQL Database
- CI/CD: GitHub Actions

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Team

Developed by the StayEase team

## 📧 Contact

- **GitHub**: [@shg880106](https://github.com/shg880106)
- **Project Repository**: [StayEase](https://github.com/shg880106/StayEase)

---

**Main Branch** - Stable Production Release  
**Last Updated**: May 2026
