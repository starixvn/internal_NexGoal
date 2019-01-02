CREATE PROCEDURE [dbo].[Notification_MarkAsRead] 
	@EmployeeId INT
	,@StatusId TINYINT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190102@Oliver.Dang
Task: mark as read Notification
DB: bodb_NexGoal
Original: 
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT n.EmployeeId AS '@ei'
				,n.StatusId AS '@si'
			FROM dbo.[Notification] AS n WITH (NOLOCK)
			WHERE n.EmployeeId = @EmployeeId
			FOR XML PATH('r'),TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.[Notification] WITH (ROWLOCK, UPDLOCK)
	SET StatusId = @StatusId
	WHERE EmployeeId = @EmployeeId

	SET @xml = (
			SELECT n.EmployeeId AS '@ei'
				,n.StatusId AS '@si'
			FROM dbo.[Notification] AS n WITH (NOLOCK)
			WHERE n.EmployeeId = @EmployeeId
			FOR XML PATH('r'),TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @EmployeeId
		,@SPName = 'Notification_MarkAsRead'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END