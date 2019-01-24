CREATE VIEW [dbo].[EmployeeOvertime]
AS
SELECT otd.EmployeeId
	,ot.[Date]
	,otd.[Hour]
FROM [$(NexHRM)].dbo.Overtime AS ot WITH (NOLOCK)
INNER JOIN [$(NexHRM)].dbo.OvertimeDetail AS otd WITH (NOLOCK) ON ot.OvertimeId = otd.OvertimeId
