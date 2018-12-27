CREATE PROCEDURE [dbo].[Notification_GetSettingByEmployee] 
	@EmployeeId INT
AS
/*
Created: 20181227@Oliver.Dang
Task: Get Notification settings
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	SELECT r.ResourceId AS [Type]
		  ,r.[Description]
		  ,ISNULL(n.ReceiveNotification,1) as ReceiveNotification
		  ,ISNULL(n.ReceiveEmail, 1) as ReceiveEmail
	FROM dbo.RefResource AS r WITH (NOLOCK)
		LEFT JOIN dbo.NotificationSetting AS n WITH (NOLOCK) ON r.ResourceId = n.TypeId
	WHERE r.RefName = 'NotificationType'
		AND n.EmployeeId = @EmployeeId

END