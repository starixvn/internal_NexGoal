CREATE PROCEDURE [dbo].[Nomination_Insert_xtest] 
	@NominationPeriodId INT
	,@NomineeId INT
	,@NominatorId INT
	,@KPIList XML
	,@PerformanceRecordId INT OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200616@Otis.vu
Task: Nomiate an employee
DB: bodb_NexGoal
Original:
-@KPICommentList
	<Root>
		<Row KPIGroupId="5" KPIId="19" Rate="4" Comment="good"/>
		<Row KPIGroupId="5" KPIId="20" Rate="4" Comment="excellent"/>
		<Row KPIGroupId="6" KPIId="21" Rate="5" Comment="excellent zzz"/>
	</Root>
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT = 0
		,@Nominated TINYINT = 1
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	IF EXISTS (
			SELECT 1
			FROM dbo.NominationDetail AS nd WITH (NOLOCK)
				INNER JOIN dbo.Nomination AS n WITH (NOLOCK) ON nd.NominationId = n.NominationId
			WHERE nd.NominatorId = @NominatorId
				AND n.NomineeId = @NomineeId
				AND n.EOQYId = @NominationPeriodId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The employee has already been nominated in this period.'

		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	DECLARE @NominationId INT

	SELECT @NominationId = n.NominationId
	FROM dbo.Nomination AS n WITH (NOLOCK)
	WHERE n.NomineeId = @NomineeId
		AND n.EOQYId = @NominationPeriodId


	IF @NomineeId = @NominatorId
	BEGIN
		SET @RoleId = @SelfProcessId
	END
	ELSE
	BEGIN
		SELECT @RoleId = (CASE ProcessId
				WHEN @EvaluatorProcessId THEN @EvaluatorProcessId
				WHEN @ReviewerProcessId THEN @ReviewerProcessId
				ELSE @PMProcessId END
				)
		FROM dbo.func_GetMember(@NominatorId)
		WHERE EmployeeId = @NomineeId
			AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		SET @RoleId = ISNULL(@RoleId, 0)
	END


	IF @NominationId IS NULL
	BEGIN
		INSERT INTO dbo.Nomination (
			EOQYId
			,NomineeId
			,StatusId
			)
		VALUES (
			@NominationPeriodId
			,@NomineeId
			,@Nominated
		)

		SELECT @NominationId = SCOPE_IDENTITY()
	END


	INSERT INTO dbo.PerformanceRecord (
		EmployeeId
		,CreatorId
		,Comment
		,CreatedDate
		,TypeId
		,RoleId
	)
	VALUES (
		@NomineeId
		,@NominatorId
		,NULL
		,GETDATE()
		,3
		,@RoleId
	)

	SET @PerformanceRecordId = SCOPE_IDENTITY()

	INSERT INTO dbo.NominationDetail (
		NominationId
		,NominatorId
		,PerformanceRecordId
	)
	VALUES (
		@NominationId
		,@NominatorId
		,@PerformanceRecordId
	)


	DECLARE @idoc INT
	EXEC sp_xml_preparedocument @idoc OUTPUT, @KPIList

	CREATE TABLE #tblKPIList (
		 KPIGroupId INT
		,KPIId INT
		,Rate INT
		,Comment NVARCHAR(MAX)
	)
	INSERT INTO #tblKPIList (
		 KPIGroupId
		,KPIId
		,Rate
		,Comment
	)
	SELECT KPIGroupId
		,KPIId
		,Rate
		,Comment
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (KPIGroupId INT, KPIId INT, Rate INT, Comment NVARCHAR(MAX)) AS Data


	INSERT INTO dbo.NominationKPI (NominationId, NominatorId ,KPIGroupId, KPIId, Comment)
	SELECT @NominationId, @NominatorId, k.KPIGroupId, k.KPIId, k.Comment
	FROM #tblKPIList as k


	INSERT INTO dbo.NominationKPIGroup (NominationId, NominatorId, KPIGroupId, Rate)
	SELECT DISTINCT @NominationId, @NominatorId, k.KPIGroupId, k.Rate
	FROM #tblKPIList as k


	SET @xml = (
		SELECT n.NominationId AS '@ni'
			,n.EOQYId AS '@eoqyi'
			,n.NomineeId AS '@nei'
			,n.StatusId AS '@si'
			,nd.NominatorId AS '@nri'
			,nd.PerformanceRecordId AS '@pri'
			,pr.CreatedDate AS '@prcd'
			,pr.TypeId AS '@prti'
		FROM dbo.Nomination AS n WITH (NOLOCK)
			INNER JOIN dbo.NominationDetail AS nd WITH (NOLOCK) ON n.NominationId = nd.NominationId
			INNER JOIN dbo.PerformanceRecord AS pr WITH (NOLOCK) ON nd.PerformanceRecordId = pr.PerformanceRecordId
			INNER JOIN dbo.NominationKPI AS nk WITH (NOLOCK) ON n.NominationId = nk.NominationId
			INNER JOIN dbo.NominationKPIGroup AS nkg (NOLOCK) ON nk.KPIGroupId = nkg.KPIGroupId
		WHERE n.EOQYId = @NominationPeriodId
			AND n.NomineeId = @NomineeId
			AND nd.NominatorId = @NominatorId
		FOR XML PATH('r')
			,TYPE
		)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @NominatorId
		,@SPName = 'Nomination_Insert_Test'
		,@ObjectId = @NominationId
		,@Data = @logData
		,@Duration = @Duration

	DROP TABLE #tblKPIList
END