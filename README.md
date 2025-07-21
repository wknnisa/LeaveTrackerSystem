\# 🗂️ Leave Tracker System



A leave management system built with ASP.NET Core MVC using Onion Architecture.  



---



\## 🧠 Features



\- Employee leave request with balance validation

\- Manager approval/rejection flow with confirmation

\- Admin access to all leave requests with full filter/search

\- Role-based login and dashboard redirect

\- Session timeout handling



---



\## 🧱 Architecture



\- Follows \*\*Onion Architecture\*\*:

&nbsp; - 'Domain' – Core models \& interfaces

&nbsp; - 'Application' – Business logic

&nbsp; - 'Infrastructure' – Data access, PDF, external tools

&nbsp; - 'WebApp' – MVC UI using Razor views

&nbsp; - 'Testing' – xUnit test project



---



\## 📊 Flowcharts



\- \[Login Flow v1.1](docs/flowcharts/LoginFlow\_v1.1.png)  

&nbsp; → Includes session timeout logic (enhanced from v1.0)

\- \[Employee Leave Request Flow v1.0](docs/flowcharts/EmployeeLeaveFlow\_v1.0.png)

\- \[Manager Approval Flow v1.0](docs/flowcharts/ManagerFlow\_v1.0.png)

\- \[Admin Leave Management Flow v1.0](docs/flowcharts/AdminFlow\_v1.0.png)

