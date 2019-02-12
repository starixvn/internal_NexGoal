CREATE PROCEDURE [dbo].[Notification_GetByEmployee] @EmployeeId INT
	,@PageSize INT
	,@PageIndex INT
	,@TotalItems INT OUT
	,@UnReadNoti INT OUT
AS
/*
Created: 20190102@Oliver.Dang
Task: Get Notification by Employee
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT noti.NotificationId
		,noti.CreatorId
		,creator.Code As CreatorCode
		,noti.EmployeeId
		,noti.Content
		,noti.StatusId
		,noti.ActionTypeId
		,noti.RecordId
		,noti.CreatedDate
	FROM dbo.[Notification] AS noti WITH (NOLOCK)
	LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON noti.CreatorId = creator.EmployeeId
	WHERE noti.EmployeeId = @EmployeeId
	ORDER BY CreatedDate DESC OFFSET(@PageIndex * @PageSize) ROWS

	FETCH NEXT @PageSize ROWS ONLY

	SELECT @TotalItems = COUNT(1)
		,@UnreadNoti = SUM(CASE 
				WHEN noti.StatusId = 0 THEN 1
				ELSE 0
				END)
	FROM dbo.[Notification] AS noti WITH (NOLOCK)
	WHERE EmployeeId = @EmployeeId
END
