🚌 ### Online Bus Reservation System (OBRS)

A full-stack web application built with ASP.NET Core MVC and SQL Server that simplifies bus ticket booking, seat management, and revenue tracking.

✨ ##Features
👤 User Panel

Search for buses by route & date

View seat availability & fares

Book and manage tickets

E-ticket generation

👨‍💼 ##Admin Panel

Manage buses, routes, prices, and schedules

Approve / reject / cancel bookings

Track revenue and generate reports

Manage users and roles

📊 ##Dashboard

Real-time insights (bookings, revenue, users)

Key statistics with counters

Graphical reports

🔐 ##Authentication & Authorization

Secure login & registration

Role-based access (Admin / User)

Identity Framework integration

🏗️ ##System Architecture

The project follows a 3-Tier Architecture:

Presentation Layer → ASP.NET Core MVC (Views)

Business Layer → Controllers & Services

Data Layer → Entity Framework Core + SQL Server

🔄 ##Project Flow

User searches for buses

User selects a bus & route

System displays seat availability & fare

User books a seat (confirmation + payment)

Admin manages bookings (approve/reject/cancel)

Reports & revenues are generated


🗄️ ##Database Design

Main tables in the system:

tbl_users → stores user details

tbl_buses → stores bus info (Bus No, Capacity, etc.)

tbl_routes → route info (From, To, Distance, etc.)

tbl_prices → stores fare (Base Fare, Final Fare)

tbl_bookings → booking details (UserId, BusId, Seat, Status, Dates)

📌 ##ERD (Entity Relationship Diagram):


🛠️ ##Technology Stack

Frontend: HTML, CSS, Bootstrap, Javascript

Backend: ASP.NET Core MVC (C#)

Database: SQL Server

ORM: Entity Framework Core

Authentication: Identity (Role-based Authorization)

📌 ##Diagram:


🚀 ##Installation & Setup

Clone the repo

git clone https://github.com/yourusername/Online-Bus-Reservation-System.git
cd Online-Bus-Reservation-System


Open in Visual Studio

Open .sln file

Restore NuGet packages

Configure Database

Update appsettings.json with your SQL Server connection string

Run migrations:

Add-Migration InitAuth -Context OBRSContext
Update-Database -Context OBRSContext

Add-Migration InitDashboard -Context obrsContext
Update-Database -Context obrsContext


Run the project

Press F5 or dotnet run



📈 ##Future Enhancements

Online Payment Gateway Integration

Real-time Bus Tracking (GPS)

Mobile App (Xamarin / React Native)

Notification system (Email / SMS)

👨‍💻 ##Author

##Developed by Huzaifa Mustafa
📌 For queries & collaboration: https://www.linkedin.com/in/muhammad-huzaifa-mustafa-77a96b266
