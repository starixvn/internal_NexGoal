CREATE PROCEDURE [dbo].[Probation_UpdateEvaluatorEvaluation]
	@EmployeeId INT
	,@Details VARCHAR(MAX)
	,@TotalScore VARCHAR(MAX)
	,@Comment NVARCHAR(MAX)
	,@IsQualifiedByEvaluator BIT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: update evaluator evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM dbo.ProbationSetting WITH (NOLOCK) WHERE EmployeeId = @EmployeeId AND  EvaluatorId = @ActorId AND ProcessId = 4)
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
				,pe.EvaluatorScore '@es'
				,pe.EvaluatorComment '@ec'
				,pe.IsQualifiedByEvaluator '@iqbe'
				,(
					SELECT ped.EmployeeId '@ei'
						,ped.KPIId '@kpiid'
						,ped.EvaluatorPoint '@ep'
					FROM dbo.ProbationEvaluationDetail ped WITH (NOLOCK)
					WHERE ped.EmployeeId = @EmployeeId
					FOR XML PATH('r'),TYPE
				) 'details'
			FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
			WHERE pe.EmployeeId = @EmployeeId
			FOR XML PATH('r'),TYPE
		)
			
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC sp_xml_preparedocument @docHandler OUTPUT, @Details

	UPDATE ped WITH (ROWLOCK, UPDLOCK)
	SET ped.EvaluatorPoint = s.Point
	FROM dbo.ProbationEvaluationDetail ped
		INNER JOIN OPENXML(@docHandler, '/Root/Row', 1) WITH (EmployeeId INT, KPIId TINYINT, Point VARCHAR(MAX)) s
		ON ped.EmployeeId = s.EmployeeId AND ped.KPIId = s.KPIId


	UPDATE dbo.ProbationEvaluation WITH (ROWLOCK, UPDLOCK)
	SET EvaluatorComment = @Comment
		,EvaluatorScore = @TotalScore
		,IsQualifiedByEvaluator= @IsQualifiedByEvaluator
	WHERE EmployeeId = @EmployeeId

	SET @xml = 
	(
		SELECT pe.EmployeeId '@ei'
			,pe.EvaluatorScore '@es'
			,pe.EvaluatorComment '@ec'
			,pe.IsQualifiedByEvaluator '@iqbe'
			,(
				SELECT ped.EmployeeId '@ei'
					,ped.KPIId '@kpiid'
					,ped.EvaluatorPoint '@ep'
				FROM dbo.ProbationEvaluationDetail ped WITH (NOLOCK)
				WHERE ped.EmployeeId = @EmployeeId
				FOR XML PATH('r'),TYPE
			) 'details'
		FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		WHERE pe.EmployeeId = @EmployeeId
		FOR XML PATH('r'),TYPE
	)
			
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'Probation_UpdateEvaluatorEvaluation'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END