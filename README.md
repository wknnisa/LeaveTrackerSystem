# 🗂️ Leave Tracker System 
**ASP.NET Core MVC | REST API | Onion Architecture**

A role-based Leave Management System built with **ASP.NET Core MVC** following **Onion Architecture** principles.

The system simulates a real-world internal HR workflow where employees submit leave requests, managers review and approve or reject them, and administrators monitor system-wide leave data.

The project demonstrates backend engineering practices including layered architecture, workflow validation, reporting dashboards, PDF export, and real-world UI handling such as empty data states, safe chart rendering, and conditional feature availability scenarios.

💡 Focused on real-world system behavior including validation, workflow consistency, and handling edge cases to ensure reliable and predictable user experience.

---

## 🌐 Live Demo

Azure deployment:

[Live Demo](https://leavetrackersystem-demo-bfdtfcdvbrf7dwhe.malaysiawest-01.azurewebsites.net)

Demo login credentials are provided below.

---

## 🔑 Demo Login

### Employee  
email: employee@example.com  
password: employee123  

### Manager  
email: manager@example.com  
password: manager123  

### Admin  
email: admin@example.com  
password: admin123

---

## 🧠 Key Features

### Role-Based Workflow
- 🧑‍💼 Role-based access: **Admin, Manager, Employee**
- 🔐 Session-based authentication using ASP.NET Core session
- ⏱ Session timeout handling with automatic login redirect
- 🧭 Active navigation highlighting in the navbar for improved user experience

### Leave Management
- 📝 Employee leave request submission
- 🔍 Leave balance validation
- ⚙️ Business rule validation for leave duration and entitlement
- ✅ Manager approval / rejection workflow
- 🗂 Admin overview of all leave records

### Reporting & Export
- 📊 Leave usage dashboards using **Chart.js**
- 🥧 Pie chart for leave type distribution
- 📈 Monthly leave usage bar chart
- 🖨 Export leave summary to PDF using **PdfSharpCore**
- 🧩 Proper empty state handling across dashboards and tables
- 🚫 Safe Chart.js rendering (prevents display when no data)
- 📄 Conditional PDF export (disabled when no data available)

### Backend Engineering Practices
- 🧱 **Onion Architecture**
- 🔗 **Repository Pattern**
- ⚙️ **Service Layer**
- 📄 **REST API with Swagger**
- 📑 **Pagination for large datasets**
- 🪵 **Structured logging using Serilog**
- 🚨 **Global exception middleware**
- 📡 **Request logging middleware**
- 🧪 **Unit testing using xUnit and FluentAssertions**
- 📦 Dependency Injection across application layers
- 🗄️ Entity Framework Core (ORM) for database interaction

---

## 🔄 System Workflow

The system follows a structured approval workflow.

### Employee

1. Submit leave request
2. System validates leave balance
3. Request status set to **Pending**

### Manager

1. Review employee leave requests
2. Approve or reject requests
3. Request status updated to **Approved** or **Rejected**

### Admin

1. View all leave requests
2. Monitor system-wide leave usage
3. Access reporting dashboards and records

This ensures leave requests follow a **controlled approval process before being finalized**.

---

## 🧰 Tech Stack

### Backend
- .NET 8
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQLite (used for lightweight WebApp deployment and demo)
- SQL Server (used in API project for enterprise-style data handling)

### Frontend
- Razor Views
- Bootstrap
- Chart.js

### Libraries
- PdfSharpCore (PDF export)
- Serilog (logging)
- Swagger (API documentation)

### Testing
- xUnit
- FluentAssertions

---

## 🏗 System Architecture

The system follows **Onion Architecture** where dependencies flow inward.

WebApp / Api  
↓  
Application (services, DTOs)  
↓  
Domain (entities, enums)  
↓  
Infrastructure (EF Core repositories, persistence)

### Project Structure

- `Domain` – Entities, enums, and core domain models
- `Application` – Business logic services and DTOs
- `Infrastructure` – EF Core repositories, persistence, and external services
- `WebApp` – ASP.NET Core MVC controllers, Razor views, ViewModels
- `Api` – REST API controllers exposing system functionality with Swagger documentation
- `Testing` – Unit tests using xUnit and FluentAssertions

---

## 🧩 Flowcharts

- [Login Flow v1.1](docs/flowcharts/LoginFlow_v1.1.png)  
- [Employee Leave Request Flow v1.0](docs/flowcharts/EmployeeLeaveFlow_v1.0.png)
- [Manager Approval Flow v1.0](docs/flowcharts/ManagerFlow_v1.0.png)
- [Admin Leave Management Flow v1.0](docs/flowcharts/AdminFlow_v1.0.png)

---

## 🗃️ ERD

Entity relationship design:

- [ERD v1.1](docs/erd/ERD_v1.1.png)

---

## 🖼️ Screenshots

Preview of major UI pages and reporting features.

![Login Page](docs/screenshots/LoginPage_v1.2.png)
![Employee Dashboard](docs/screenshots/EmployeeDashboard_v1.2.png)
![Submit Leave Form](docs/screenshots/SubmitLeaveForm_v1.2.png)
![My Leave Requests](docs/screenshots/MyLeaveRequests_v1.2.png)
![Manager Approval](docs/screenshots/ManagerPendingRequests_v1.2.png)
![Leave Summary Dashboard](docs/screenshots/LeaveSummaryDashboard_v1.2.png)
![PDF Export](docs/screenshots/LeaveSummaryPdf_v1.1.png)

---

## 🧪 Testing

### Manual Test Cases

Detailed functional test scenarios:

[Manual Test Cases v1.1](docs/test-cases/ManualTests_v1.1.md)

### Unit Tests

Located in the `Testing/` project.

Run tests:

```bash
dotnet test
```

---

## ▶️ Running the Application

Run the MVC application:

```bash
dotnet run --project LeaveTrackerSystem.WebApp
```

Run the API project (for local development and Swagger testing):

```bash
dotnet run --project LeaveTrackerSystem.Api
```

Once the API starts, open Swagger in your browser:

[Swagger](https://localhost:{port}/swagger)

Swagger provides interactive documentation where you can test the available API endpoints.

---

## 🚀 Deployment

The MVC WebApp project is configured to run using **SQLite** so it can be deployed easily without requiring a database server.

The API project uses **SQL Server** and can be configured separately for environments where SQL Server is available.

---

## 🔗 API Endpoints

The system also exposes REST API endpoints through the `LeaveTrackerSystem.Api` project.

API documentation is available via **Swagger UI**.

Example endpoints currently implemented:

- `GET /api/leave-requests`
- `GET /api/leave-types`

These endpoints demonstrate how backend services can be exposed through a REST API layer while reusing the same business logic used by the MVC application.

---

## 📌 Project Status

Core workflow fully implemented and validated through final bugfix pass.

Implemented features include:

 - Role-based leave management workflow
 - REST API endpoints with Swagger documentation
 - Dashboard reporting with Chart.js
 - PDF export functionality
 - Structured logging and exception handling
 - Pagination for large datasets
 - Unit testing for core business logic
 - Improved UI reliability by handling empty data scenarios across charts, tables, and reports

The system is ready for deployment and demonstration as a backend portfolio project.

---

## 📚 Learning Focus

This project was built to practice enterprise backend development concepts including:

- Layered architecture design
- Service and repository pattern implementation
- Workflow validation logic
- Logging and middleware integration
- API design and documentation