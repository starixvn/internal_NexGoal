CREATE VIEW OverTime
AS
SELECT EmpId AS EmployeeId,Date,Minutes
FROM NexHRM.dbo.OvertimeTrans WITH(NOLOCK)