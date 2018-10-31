
CREATE PROCEDURE [dbo].[LogDetail_Insert] @ActorId INT
	,@SPName NVARCHAR(50)
	,@ObjectId INT = NULL
	,@Data NVARCHAR(MAX) = NULL
	,@OldData NVARCHAR(MAX) = NULL
	,@Duration INT = NULL
AS
/*
Created: 20180629@Tim.Trinh
Task: write log
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @actionId SMALLINT

	SELECT @actionId = ActionId
	FROM dbo.LogAction WITH (NOLOCK)
	WHERE SPName = @SPName

	INSERT INTO dbo.LogDetail (
		ActorId
		,ActionId
		,ObjectId
		,LogDate
		,Duration
		,Data
		,OldData
		)
	VALUES (
		@ActorId
		,@actionId
		,@ObjectId
		,GETDATE()
		,@Duration
		,@Data
		,@OldData
		)
END
