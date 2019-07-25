CREATE PROCEDURE [dbo].[Probation_UpdateSelfEvaluation]
	@EmployeeId INT
	,@Details VARCHAR(MAX)
	,@TotalScore VARCHAR(MAX)
	,@Comment NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: update self evaluation
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
		,@docHandler INT
		,@xml XML

	SET @xml = 
		(
			SELECT pe.EmployeeId '@ei'
				,pe.SelfScore '@ss'
				,pe.SelfComment '@sc'
				,(
					SELECT ped.EmployeeId '@ei'
						,ped.KPIId '@kpiid'
						,ped.SelfPoint '@sp'
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
	SET ped.SelfPoint = s.Point
	FROM dbo.ProbationEvaluationDetail ped
		INNER JOIN OPENXML(@docHandler, '/Root/Row', 1) WITH (EmployeeId INT, KPIId TINYINT, Point  VARCHAR(MAX)) s
		ON ped.EmployeeId = s.EmployeeId AND ped.KPIId = s.KPIId

	UPDATE dbo.ProbationEvaluation WITH (ROWLOCK, UPDLOCK)
	SET SelfComment = @Comment
		,SelfScore = @TotalScore
	WHERE EmployeeId = @EmployeeId

	SET @xml = 
	(
		SELECT pe.EmployeeId '@ei'
			,pe.SelfScore '@ss'
			,pe.SelfComment '@sc'
			,(
				SELECT ped.EmployeeId '@ei'
					,ped.KPIId '@kpiid'
					,ped.SelfPoint '@sp'
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

	EXEC dbo.LogDetail_Insert @ActorId = @EmployeeId
		,@SPName = 'Probation_UpdateSelfEvaluation'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END