


CREATE VIEW [dbo].[EmployeeCalendar]
AS
SELECT EmpId
	  ,WorkDate
	  ,IsOffDay
	  ,IsHoliday
	  ,IsLeaveDay
	  ,LeaveDay
FROM [$(NexHRM)].dbo.EmployeeCalendar WITH (NOLOCK)