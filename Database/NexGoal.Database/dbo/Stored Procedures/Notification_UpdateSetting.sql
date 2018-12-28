CREATE PROCEDURE [dbo].[Notification_UpdateSetting] 
	@EmployeeId INT
	,@Value VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181228@Oliver.Dang
Task: Update Notification Setting
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Value : '<ROOT><Row TypeId="4" ReceiveNotification="1" ReceiveEmail="0"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	-- Write log before assign permission	
	SET @xml = (
			SELECT @EmployeeId AS '@r'
				,(
					SELECT TypeId AS '@f'
						,EmployeeId AS '@e'
						,ReceiveNotification AS '@n'
						,ReceiveEmail AS '@m'
					FROM dbo.NotificationSetting WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	CREATE TABLE #tblNotificationSetting (
		TypeId TINYINT
		,ReceiveNotification BIT
		,ReceiveEmail BIT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblNotificationSetting (
		TypeId
		,ReceiveNotification
		,ReceiveEmail
		)
	SELECT TypeId
		,ReceiveNotification
		,ReceiveEmail
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (TypeId TINYINT, ReceiveNotification BIT, ReceiveEmail BIT) AS data
	
	-- update policy for function of role
	UPDATE ns
	SET ns.ReceiveNotification = tbl.ReceiveNotification
		,ns.ReceiveEmail = tbl.ReceiveEmail
	FROM dbo.NotificationSetting AS ns WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblNotificationSetting AS tbl WITH (NOLOCK) ON ns.TypeId = tbl.TypeId
	WHERE ns.EmployeeId = @EmployeeId

	-- insert new policy for function of role
	INSERT INTO dbo.NotificationSetting(
		TypeId
		,EmployeeId
		,ReceiveNotification
		,ReceiveEmail
		)
	SELECT tbl.TypeId
		,@EmployeeId
		,tbl.ReceiveNotification
		,tbl.ReceiveEmail
	FROM #tblNotificationSetting AS tbl WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.NotificationSetting AS ns WITH (NOLOCK)
			WHERE ns.EmployeeId = @EmployeeId
				AND ns.TypeId = tbl.TypeId
			)

	-- write log after update
	SET @xml = (
			SELECT @EmployeeId AS '@r'
				,(
					SELECT TypeId AS '@f'
						,EmployeeId AS '@e'
						,ReceiveNotification AS '@n'
						,ReceiveEmail AS '@m'
					FROM dbo.NotificationSetting WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
					FOR XML PATH('z')
						,TYPE
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @EmployeeId
		,@SPName = 'Notification_UpdateSetting'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblNotificationSetting
END
