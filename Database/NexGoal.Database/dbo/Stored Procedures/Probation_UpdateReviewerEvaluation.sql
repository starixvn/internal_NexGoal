CREATE PROCEDURE [dbo].[Probation_UpdateReviewerEvaluation]
	@EmployeeId INT
	,@IsQualified INT
	,@Comment NVARCHAR(MAX)
	,@ActorId NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: update reviewer evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM dbo.ProbationSetting WITH (NOLOCK) WHERE EmployeeId = @EmployeeId AND  EvaluatorId = @ActorId AND ProcessId = 5)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission.'
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@docHandler INT
		,@xml XML

	SET @xml = 
		(
			SELECT pe.EmployeeId '@ei'
				,pe.ReviewerComment '@rc'
				,pe.IsQualified '@iq'
			FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
			WHERE pe.EmployeeId = @EmployeeId
			FOR XML PATH('r'),TYPE
		)
			
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	 
	 UPDATE dbo.ProbationEvaluation WITH (ROWLOCK, UPDLOCK)
	 SET IsQualified = @IsQualified
		,ReviewerComment = @Comment
	 WHERE EmployeeId = @EmployeeId

	SET @xml = 
	(
		SELECT pe.EmployeeId '@ei'
			,pe.ReviewerComment '@rc'
			,pe.IsQualified '@iq'
		FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		WHERE pe.EmployeeId = @EmployeeId
		FOR XML PATH('r'),TYPE
	)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'Probation_UpdateReviewerEvaluation'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END