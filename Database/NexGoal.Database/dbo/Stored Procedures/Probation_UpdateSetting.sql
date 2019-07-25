CREATE PROCEDURE [dbo].[Probation_UpdateSetting]
	@EmployeeId INT
	,@ActorId INT
	,@Settings VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190520@Elton.ton
Task: start probation evaluation
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

	EXEC sp_xml_preparedocument @docHandler OUTPUT, @Settings

	SET @xml = 
		(
			SELECT EmployeeId '@ei'
				,ProcessId '@pri'
				,EvaluatorId '@evaid'
				,FromDate '@fd'
				,ToDate '@td'
			FROM dbo.ProbationSetting WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			FOR XML PATH('r'),TYPE
		)
			
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE ps WITH (ROWLOCK, UPDLOCK)
	SET ps.EvaluatorId = s.EvaluatorId
		,ps.FromDate = s.FromDate
		,ps.ToDate = s.ToDate
	FROM dbo.ProbationSetting ps
		INNER JOIN OPENXML(@docHandler, '/Root/Row', 1) WITH (EmployeeId INT, ProcessId TINYINT, EvaluatorId INT, FromDate DATE, ToDate DATE) s 
			ON ps.EmployeeId = s.EmployeeId AND ps.ProcessId = s.ProcessId

	SET @xml = 
	(
		SELECT EmployeeId '@ei'
			,ProcessId '@pri'
			,EvaluatorId '@evaid'
			,FromDate '@fd'
			,ToDate '@td'
		FROM dbo.ProbationSetting WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId
		FOR XML PATH('r'),TYPE
	)
			
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	
	EXEC dbo.LogDetail_Insert @ActorId = @ActorId
		,@SPName = 'Probation_UpdateSetting'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END