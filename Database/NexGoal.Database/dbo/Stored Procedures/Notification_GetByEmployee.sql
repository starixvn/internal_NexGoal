CREATE PROCEDURE [dbo].[Notification_GetByEmployee] @EmployeeId INT
AS
/*
Created: 20190102@Oliver.Dang
Task: Get Notification by Employee
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tplEmployee (EmpId INT)

	INSERT INTO #tplEmployee (EmpId)
	SELECT DISTINCT CreatorId
	FROM dbo.[Notification] WITH (NOLOCK)
	WHERE CreatorId <> @EmployeeId
	
	UNION
	
	SELECT @EmployeeId

	SELECT noti.NotificationId
		,noti.CreatorId
		,noti.EmployeeId
		,noti.Content
		,noti.StatusId
		,noti.CreatedDate
	FROM dbo.[Notification] AS noti WITH (NOLOCK)
	WHERE EmployeeId = @EmployeeId

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.Code
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN #tplEmployee AS tpl WITH (NOLOCK) ON e.EmployeeId = tpl.EmpId

	DROP TABLE #tplEmployee
END
