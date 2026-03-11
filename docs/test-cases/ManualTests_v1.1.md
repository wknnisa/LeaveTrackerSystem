# Leave Tracker System - Manual Test Cases (v1.1)

Created: 4 Aug 2025
Last Updated: 11 Mar 2026
Author: wknnisa
System: Leave Tracker (ASP.NET Core MVC)
Version: v1.1

Scope: Core functional scenarios covering leave submission, approval workflow, access control, and report export.

Roles covered: 
- Employee
- Manager

---

## Test Environment

Environment: Localhost
Framework: ASP.NET Core MVC
Language: C#
Database: SQL Server
Browser used: Google Chrome
Testing type: Manual functional testing

---

## TC01 - Submit Valid Leave Request

**Role:** Employee

**Steps**

1. Login as Employee
2. Navigate to **Submit Leave Request**
3. Set:
   - Start Date = 10/08/2025
   - End Date = 12/08/2025
4. Select Leave Type = Annual
5. Enter Reason = "Family vacation"
6. Click **Submit**

**Expected Result**

- Form submission succeeds
- Leave request is saved to database
- Status automatically set to **Pending**
- Confirmation message appears: 
  > "Leave request submitted successfully and marked as Pending."
- Record appears in **My Requests** list

---

## TC02 - Invalid Date Validation

**Role:** Employee

**Steps**

1. Login as Employee
2. Navigate to **Submit Leave Request**
3. Set:
   - Start Date = 12/08/2025
   - End Date = 10/08/2025
4. Select Leave Type = Emergency
5. Enter Reason = "Emergency test"
6. Click **Submit**

**Expected Result**

- Form submission is blocked
- Validation message appears: 
  > "End Date must be on or after Start Date."
- No record is created in database

---

## TC03 - Submit Leave with Insufficient Balance

**Role:** Employee

**Steps**

1. Login as Employee
2. Ensure the user has already used their full leave entitlement for the selected leave type
3. Attempt to submit a new request using the same leave type
4. Click **Submit**

**Expected Result**

- Submission is blocked by business logic
- Error message appears: 
  > "Insufficient leave balance for this leave type."
- No new leave request record is created

---

## TC04 - Unauthorized Access to Manager Page

**Role:** Employee

**Steps**

1. Login as Employee
2. Attempt to manually navigate to `/Manager/Index`

**Expected Result**

- Access is blocked by session role validation
- User is redirected to the **Login page**
- Manager pages are accessible only when session role = **Manager**

---

## TC05 - Export Leave Summary PDF

**Role:** Employee

**Precondition**

Employee has at least **one Approved leave request**.

**Steps**

1. Login as Employee
2. Navigate to **Leave Summary**
3. Click **Download PDF**

**Expected Result**

- PDF file downloads successfully as `LeaveSummary.pdf`
- Title: **Leave Summary Report**
- Leave type summary table
- Columns: 
  - Leave Type
  - Used
  - Remaining
- Data matches values shown on the Leave Summary page
- Layout and formatting render correctly

---

## TC06 - Session Expiry Handling

**Role:** Employee

**Steps**

1. Login as Employee
2. Leave system idle until session timeout occurs
3. Attempt to access Employee dashboard or any protected page

**Expected Result**

- User is redirected to **Login page**
- Session is cleared
- Protected pages cannot be accessed without login

---

## TC07 - Manager Approve Leave Request

**Role:** Manager

**Precondition**

At least one leave request exists with status **Pending**.

**Steps**

1. Login as Manager
2. Navigate to **Manager Dashboard**
3. Locate a leave request with status **Pending**
4. Click **Approve**

**Expected Result**

- Request status changes to **Approved**
- Updated status appears in the request list
- Employee Leave Summary reflects updated leave usage

---

# Test Coverage Summary

The above test cases verify the following system behaviors:

- Leave submission workflow
- Input validation logic
- Leave balance business rules
- Role-based access control
- PDF report generation
- Session timeout handling

---

# Test Execution Status

| Test Case | Description                            | Status |
|-----------|----------------------------------------|--------|
| TC01      | Submit Valid Leave Request             | Passed |
| TC02      | Invalid Date Validation                | Passed |
| TC03      | Submit Leave with Insufficient Balance | Passed |
| TC04      | Unauthorized Access to Manager Page    | Passed |
| TC05      | Export Leave Summary PDF               | Passed |
| TC06      | Session Expiry Handling                | Passed |
| TC07      | Manager Approve Leave Request          | Passed |

Test execution performed locally on 11 Mar 2026.
All core functional scenarios executed successfully and behaved as expected.