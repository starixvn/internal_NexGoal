
CREATE PROCEDURE [dbo].[LogUserLogin_Insert] @Username VARCHAR(50)
	,@IP VARCHAR(50) = ''
	,@MachineName VARCHAR(100) = ''
	,@Platform VARCHAR(255) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190304@Tim.Trinh
Task: log login
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logTime DATETIME = GETDATE()

	INSERT INTO dbo.LogUserLogin (
		Username
		,LogTime
		,IP
		,MachineName
		,[Platform]
		)
	VALUES (
		@Username
		,GETDATE()
		,@IP
		,@MachineName
		,@Platform
		)
END