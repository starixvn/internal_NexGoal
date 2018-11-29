



CREATE VIEW [dbo].[EmployeeAttendance]
AS
SELECT EmpId
	,AttendanceDate
	,InFixed
	,LeaveDay
	,OutFixed
	,LateIn
	,EarlyOut
	,LeaveTypeId
	,LeaveTypeId2
	,Absence
	,TotalWorkHours
FROM [$(NexHRM)].dbo.EmployeeAttendance WITH (NOLOCK)