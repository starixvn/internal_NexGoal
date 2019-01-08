
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
    Notifications : '<ROOT><Row EmployeeId="" Content="" StatusId=""></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CreatedDate DATE = GETDATE()
		,@idoc INT

	CREATE TABLE #tblNotification (
		EmployeeId INT
		,Content NVARCHAR(MAX)
		,StatusId TINYINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Notifications

	INSERT INTO #tblNotification (
		EmployeeId
		,Content
		,StatusId
		)
	SELECT EmployeeId
		,Content
		,StatusId
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (EmployeeId INT, Content NVARCHAR(MAX), StatusId TINYINT) AS data

	INSERT INTO dbo.[Notification] (
		CreatorId
		,EmployeeId
		,Content
		,StatusId
		,CreatedDate
		)
	SELECT - 1
		,EmployeeId
		,Content
		,StatusId
		,@CreatedDate
	FROM #tblNotification WITH (NOLOCK)

	DROP TABLE #tblNotification
END