\# Leave Tracker System - Manual Test Cases (v1.0)

Created: 4 Aug 2025

Version: v1.0

Author: wknnisa

System: Leave Tracker (C#.NET MVC)

Scope: Core functional scenarios for leave submission, approval, and export workflows

Roles covered: Employee, Manager

---

## TC01 - Submit Valid Leave Request

\*\*Role:\*\* Employee

\*\*Steps:\*\*

1. Login as employee
2. Navigate to Submit Leave Request page
3. Set Start Date = 10/08/2025, End Date = 12/08/2025
4. Select Leave Type = Annual
5. Enter Reason = "Family vacation"
6. Click Submit

\*\*Expected Result:\*\*

* Form submits successfully
* Status is set to 'Pending'
* TempData confirmation message appears: "Leave request submitted successfully and marked as Pending."
* Record appears under My Requests

---

## TC02 - Invalid Date Validation

\*\*Role:\*\* Employee

\*\*Steps:\*\*

1. Login as employee
2. Open Submit Leave Request page
3. Set Start Date = 12/08/2025, End Date = 10/08/2025
4. Select Leave Type = Emergency
5. Enter Reason = 'Emergency test'
6. Click Submit

\*\*Expected Result:\*\*

* Form is blocked with validation message
* Custom validation triggers error: "End Date must be on or after Start Date."

---

## TC03 - Submit Leave with Zero Balance (Deferred)

\*\*Role:\*\* Employee

\*\*Note:\*\* This test is deferred until Phase 8 (#37) when MSSQL and real user leave balance is integrated.

\*\*Expected Implementation:\*\*

* When a user has already used their full entitlement for a leave type (e.g. 14 days Annual Leave),
* Submitting a new request of the same type should be blocked

\*\*Expected Result:\*\*

* Submission is blocked by business logic
* Error message shown: "Insufficient leave balance for this leave type."

---

## TC04 - Unauthorized Access to Manager Page

\*\*Role:\*\* Employee

\*\*Steps:\*\*

1. Login as Employee
2. Attempt to manually access '/Manager/Index'

\*\*Expected Result:\*\*

* Redirected to Login page (as session role is not Manager)
* Access to Manager views is blocked by session check

---

## TC04b - Unauthorized POST to Reject Action (Deferred)

\*\*Role:\*\* Employee

\*\*Note:\*\* This test is deferred until Phase 8 (#44) when session role checks are enforced in 'Reject(int id)' action.

\*\*Expected Implementation:\*\*

* Manager-only actions like 'Reject(id)' must validate the user's session role
* If the role is not "Manager", access should be denied or redirected

\*\*Expected Result:\*\*

* POST access is denied or redirected
* Unauthorized role cannot execute approval logic

---

## TC05 - Export PDF Summary (Approved Only)

\*\*Role:\*\* Employee

\*\*Note:\*\* This test currently reflects the in-memory implementation where only Approved requests are included in the summary.

\*\*Steps:\*\*

1. Login as employee with at least one Approved request
2. Navigate to Leave Summary page
3. Click "Download PDF"

\*\*Expected Result:\*\*

* PDF file downloads as 'LeaveSummary.pdf'
* Table matches Approved requests only
* Includes: Leave Type, Used, Remaining
* Title is shown: "Leave Summary Report"
* Company logo is \*\*not yet included\*\* (to be added in Phase 8: UI Polish)

\*\*Observation:\*\*

* Matches chart/table in LeaveSummary.cshtml
* Data reflects actual Approved summary from ViewModel