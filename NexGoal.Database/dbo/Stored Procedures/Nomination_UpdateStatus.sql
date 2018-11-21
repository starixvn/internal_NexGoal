CREATE PROCEDURE [dbo].[Nomination_UpdateStatus] 
	@NominationId INT
	,@StatusId TINYINT
	,@UpdatedById INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181120@Elton.Ton
Task:  change nomination status
DB: bodb_NexGoal
Original: 
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @xml = (
			SELECT n.NominationId AS '@ni'
				,n.StatusId AS '@si'
			FROM dbo.Nomination AS n WITH (NOLOCK)
			WHERE n.NominationId = @NominationId
			FOR XML PATH('r'),TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE dbo.Nomination WITH (ROWLOCK, UPDLOCK)
	SET StatusId = @StatusId
	WHERE NominationId = @NominationId

	SET @xml = (
			SELECT n.NominationId AS '@ni'
				,n.StatusId AS '@si'
			FROM dbo.Nomination AS n WITH (NOLOCK)
			WHERE n.NominationId = @NominationId
			FOR XML PATH('r'),TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @UpdatedById
		,@SPName = 'Nomination_UpdateStatus'
		,@ObjectId = @NominationId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END
