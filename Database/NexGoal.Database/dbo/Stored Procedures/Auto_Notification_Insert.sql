
CREATE PROCEDURE [dbo].[Auto_Notification_Insert] @Notifications NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190107@Tim.Trinh
Task: insert notification.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Notifications : '<ROOT><Row EmployeeId="" Content="" StatusId="" ActionTypeId="" RecordId=""></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CreatedDate DATETIME = GETDATE()
		,@idoc INT

	CREATE TABLE #tblNotification (
		EmployeeId INT
		,Content NVARCHAR(MAX)
		,StatusId TINYINT
		,ActionTypeId TINYINT
		,RecordId INT NULL
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Notifications

	INSERT INTO #tblNotification (
		EmployeeId
		,Content
		,StatusId
		,ActionTypeId
		,RecordId
		)
	SELECT EmployeeId
		,Content
		,StatusId
		,ActionTypeId
		,RecordId
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (EmployeeId INT, Content NVARCHAR(MAX), StatusId TINYINT, ActionTypeId TINYINT, RecordId INT) AS data

	INSERT INTO dbo.[Notification] (
		CreatorId
		,EmployeeId
		,Content
		,StatusId
		,ActionTypeId
		,RecordId
		,CreatedDate
		)
	SELECT - 1
		,EmployeeId
		,Content
		,StatusId
		,ActionTypeId
		,RecordId
		,@CreatedDate
	FROM #tblNotification WITH (NOLOCK)

	DROP TABLE #tblNotification
END