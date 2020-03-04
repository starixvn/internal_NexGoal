/*
DROP PROCEDURE [dbo].[Auto_PARecord_GetSettingToSendNotification]
DROP PROCEDURE [dbo].[PAFocus_Generate]
DROP PROCEDURE [dbo].[Auto_Period_Insert]
DROP PROCEDURE [dbo].[ProbationReport_Get]
DROP PROCEDURE [dbo].[PRFeedbackReaction_Delete]
DROP PROCEDURE [dbo].[PRReaction_Delete]
DROP PROCEDURE [dbo].[PAReport_GetDataForExportExcel]
DROP PROCEDURE [dbo].[Report_GetPARecord]
DROP PROCEDURE [dbo].[PRReaction_GetByPRId]
DROP PROCEDURE [dbo].[PRFeedbackReaction_GetByFeedbackId]
DROP PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformance]
DROP PROCEDURE [dbo].[Period_UpdateEmployeeSettingStatus]
DROP PROCEDURE [dbo].[Period_GetPARecordsToUpdateGrade]
DROP PROCEDURE [dbo].[Period_UpdateEmployeeSettings]
DROP PROCEDURE [dbo].[Period_GetEmployeeSettings]
DROP PROCEDURE [dbo].[Period_GetDataToUpdateFinalPAGrade]
DROP PROCEDURE [dbo].[Period_Delete]
DROP PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformanceByMemberId]
DROP PROCEDURE [dbo].[PerformanceRecord_GetByEmployee]
DROP PROCEDURE [dbo].[Auto_PA_GetDataToUpdateGrade]
DROP PROCEDURE [dbo].[PRFeedback_Insert]
DROP PROCEDURE [dbo].[PRFeedbackReaction_Insert]
DROP PROCEDURE [dbo].[PRFeedback_GetByPRId]
DROP PROCEDURE [dbo].[MasterReport_Get]
DROP PROCEDURE [dbo].[Redmine_GetTeamWorkLoad]
DROP PROCEDURE [dbo].[Period_UpdateFinalPAGrade]
DROP PROCEDURE [dbo].[PRReaction_Insert]
DROP PROCEDURE [dbo].[PARecord_GetById]
DROP PROCEDURE [dbo].[Announcement_Get]
DROP PROCEDURE [dbo].[PARecord_GetByReviewer]
DROP PROCEDURE [dbo].[Period_GeneratePA]
DROP PROCEDURE [dbo].[PRFeedback_Update]
DROP PROCEDURE [dbo].[ProbationReport_GetByEmployeeId]
DROP PROCEDURE [dbo].[Period_GetCurrentPerformancePeriod]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_GetCurrentPerformancePeriod]
AS
/*
Created: 20181101@Tim.Trinh
Task: get current performance period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()

	SELECT TOP 1 PeriodId
		,[Name]
		,StartDate
		,EndDate
		,FocusStartDate
		,FocusEndDate
		,DisciplineStartDate
		,DisciplineEndDate
	FROM dbo.Period WITH (NOLOCK)
	WHERE IsActive = 1
		AND DisciplineStartDate <= @Today
	ORDER BY DisciplineStartDate DESC
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProbationReport_GetByEmployeeId] @EmployeeId INT
	,@ActorId INT
AS
/*
Created: 20190705@Elton.ton
Task: get probationer's evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3

	IF EXISTS (
			SELECT 1
			FROM dbo.func_GetMember(@ActorId)
			WHERE EmployeeId = @EmployeeId
				AND ProcessId IN (4, 5)
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole WITH (NOLOCK)
			WHERE RoleId IN (@HRRoleId, @ManagementRoleId)
				AND EmployeeId = @ActorId
			)
		OR @EmployeeId = @ActorId
	BEGIN
		DECLARE @CanViewEvaluatorResult BIT = 0

		IF @EmployeeId != @ActorId
		BEGIN
			SET @CanViewEvaluatorResult = 1
		END

		SELECT pe.EmployeeId
			,pe.IsQualified
			,e.NickName
			,e.FullName
			,e.Code
			,e.GroupId
			,e.JoinedDate
			,jt.[Name] AS JobTitle
			,g.[Name] AS GroupName
			,pe.SelfScore
			,pe.SelfComment
			,pe.EvaluatorScore
			,pe.EvaluatorComment
			,pe.ReviewerComment
			,(CASE 
				WHEN @CanViewEvaluatorResult = 1 THEN pe.IsQualifiedByEvaluator
				ELSE NULL END
				) AS IsQualifiedByEvaluator
			,@CanViewEvaluatorResult AS CanViewEvaluatorResult
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId
					AND ps.ProcessId = 4
				) AS EvaluatorNickName
			,(
				SELECT e.NickName
				FROM dbo.ProbationSetting ps WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON ps.EvaluatorId = e.EmployeeId
				WHERE ps.EmployeeId = @EmployeeId
					AND ps.ProcessId = 5
				) AS ReviewerNickName
		FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
			INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
			INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		WHERE pe.EmployeeId = @EmployeeId
			AND EXISTS (
				SELECT 1
				FROM dbo.ProbationSetting AS ps WITH (NOLOCK)
				WHERE ps.EmployeeId = pe.EmployeeId
					AND ps.ProcessId = 5
					AND ps.ToDate < GETDATE()
				)

		SELECT KPIGroupId
			,[Name]
		FROM dbo.KPIGroup WITH (NOLOCK)

		SELECT ped.EmployeeId
			,ped.KPIId
			,ped.SelfPoint
			,ped.EvaluatorPoint
			,k.[Name] AS KPIName
			,k.KPIGroupId
		FROM dbo.ProbationEvaluationDetail ped WITH (NOLOCK)
			INNER JOIN dbo.KPI k ON ped.KPIId = k.KPIId
			INNER JOIN dbo.KPIGroup kg ON kg.KPIGroupId = k.KPIGroupId
		WHERE ped.EmployeeId = @EmployeeId
	END
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedback_Update] @PRFeedbackId INT
	,@CreatorId INT
	,@Content NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Edit comment on member's PR
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATETIME = GETDATE()
	DECLARE @OldContent NVARCHAR(MAX)

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRFeedback AS feedback WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
			AND DATEDIFF(DAY, CreatedDate, @Today) <= 7
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Cant edit this comment'

		RETURN
	END


	UPDATE dbo.PRFeedback
	SET [Content] = @Content, ModifiedDate = @Today
	WHERE PRFeedbackId = @PRFeedbackId
		AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@id'
				,@CreatorId AS '@cid'
				,@OldContent AS '@old_content'
				,@Content AS '@new_content'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedback_Update'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_GeneratePA] @PeriodId INT
	,@PARecordIds VARCHAR(MAX) = NULL
	,@CreatedBy INT
	,@Override BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180827@Elton.ton
Task: generate pa
DB: bodb_NexGoal
Original:

Revisions:
	20200212@Tim.Trinh: get related performance record
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @periodStartDate DATE
		,@periodEndDate DATE
		,@workingFromDate DATETIME
		,@workingToDate DATETIME
		,@separator NVARCHAR(42) = ','
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		-- Performance Status
		,@NotGenerated TINYINT = 1
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		-- record type
		,@CheckinTypeId TINYINT = 1
		,@PATypeId TINYINT = 2
	DECLARE @today DATE = GETDATE()
		,@createdDate DATETIME = GETDATE()
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblPARecord (
		PARecordId INT PRIMARY KEY
		,EmployeeId INT
		,StatusId TINYINT
		)

	CREATE TABLE #tblPAProcess (
		PARecordId INT
		,EvaluatorId INT
		,ProcessId TINYINT
		,GradeId VARCHAR(MAX)
		,RelatedPerformanceRecordIds VARCHAR(MAX)
		)

	INSERT INTO #tblPARecord (
		PARecordId
		,EmployeeId
		,StatusId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
		,pa.StatusId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.func_SplitString(@PARecordIds, ',') AS func ON ISNUMERIC(func.val) = 1
		AND func.val = pa.PARecordId
	WHERE pa.PeriodId = @PeriodId

	SELECT @periodStartDate = StartDate
		,@periodEndDate = EndDate
		,@workingFromDate = DisciplineStartDate
		,@workingToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SET @xml = (
			SELECT process.PARecordId AS '@parid'
				,process.ProcessId AS '@pid'
				,process.EvaluatorId AS '@eid'
				,process.GradeId AS '@gid'
				,process.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			WHERE process.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE process
	FROM dbo.PAProcess AS process
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
	WHERE process.ProcessId = @PMProcessId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PMSetting AS pm WITH (NOLOCK)
			WHERE process.EvaluatorId = pm.ProjectManagerId
				AND pm.MemberId = pa.EmployeeId
			)

	-- capture for PM
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,pm.ProjectManagerId
		,@PMProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = pm.ProjectManagerId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = @PMProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH('')
				), 1, 1, N'')
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PMSetting AS pm WITH (NOLOCK) ON pa.EmployeeId = pm.MemberId
		OUTER APPLY (
		SELECT TOP 1 record.GradeId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @CheckinTypeId
			AND record.CreatorId = pm.ProjectManagerId
			AND record.EmployeeId = pa.EmployeeId
			AND record.CreatedDate BETWEEN @workingFromDate
				AND @workingToDate
			AND record.RoleId = @PMProcessId
			AND ISNULL(record.GradeId, '') <> ''
		ORDER BY record.CreatedDate DESC
		) AS lg
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = pm.ProjectManagerId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = @PMProcessId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE pa.PARecordId = process.PARecordId
				AND process.EvaluatorId = pm.ProjectManagerId
				AND process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			)

	-- Capture for Self, Reviewer
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.EvaluatorId
		,process.ProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = process.EvaluatorId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = process.ProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH('')
				), 1, 1, N'')
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId IN (@SelfProcessId, @ReviewerProcessId)
		OUTER APPLY (
		SELECT TOP 1 record.GradeId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @CheckinTypeId
			AND record.CreatorId = process.EvaluatorId
			AND record.EmployeeId = pa.EmployeeId
			AND record.CreatedDate BETWEEN @workingFromDate
				AND @workingToDate
			AND record.RoleId = process.ProcessId
			AND ISNULL(record.GradeId, '') <> ''
		ORDER BY record.CreatedDate DESC
		) AS lg
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = process.EvaluatorId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = process.ProcessId
			)

	-- Capture for Evaluator
	INSERT INTO #tblPAProcess (
		PARecordId
		,EvaluatorId
		,ProcessId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.EvaluatorId
		,process.ProcessId
		,lg.GradeId
		,STUFF((
				SELECT @separator + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
				FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
				WHERE prcmt.CreatorId = process.EvaluatorId
					AND prcmt.EmployeeId = pa.EmployeeId
					AND prcmt.TypeId = @CheckinTypeId
					AND prcmt.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND prcmt.RoleId = process.ProcessId
				ORDER BY prcmt.CreatedDate DESC
				FOR XML PATH('')
				), 1, 1, N'')
	FROM #tblPARecord AS pa
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @EvaluatorProcessId
		OUTER APPLY (
		SELECT TOP 1 record.GradeId
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		WHERE record.TypeId = @CheckinTypeId
			AND record.EmployeeId = pa.EmployeeId
			AND record.RoleId = @EvaluatorProcessId
			AND record.CreatedDate <= @workingToDate
			AND ISNULL(record.GradeId, '') <> ''
		ORDER BY record.CreatedDate DESC
		) AS lg
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.EmployeeId = pa.EmployeeId
				AND record.RoleId = @EvaluatorProcessId
				AND (
					(
						record.CreatorId = process.EvaluatorId
						AND record.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						)
					OR (
						record.CreatedDate < @workingFromDate
						AND ISNULL(record.GradeId, '') <> ''
						)
					)
			)

	-- Insert new PM Process
	INSERT INTO dbo.PAProcess (
		PARecordId
		,ProcessId
		,EvaluatorId
		,GradeId
		,RelatedPerformanceRecordIds
		)
	SELECT pa.PARecordId
		,process.ProcessId
		,process.EvaluatorId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
	FROM #tblPAProcess AS process
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
		AND pa.StatusId = @NotGenerated
	WHERE process.ProcessId = @PMProcessId
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS process WITH (NOLOCK)
			WHERE pa.PARecordId = process.PARecordId
				AND process.ProcessId = @PMProcessId
				AND process.EvaluatorId = process.EvaluatorId
			)

	UPDATE process
	SET process.GradeId = tblprocess.GradeId
		,process.RelatedPerformanceRecordIds = tblprocess.RelatedPerformanceRecordIds
	FROM dbo.PAProcess AS process WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
		AND pa.StatusId = @NotGenerated
		INNER JOIN #tblPAProcess AS tblprocess ON pa.PARecordId = tblprocess.PARecordId
		AND process.EvaluatorId = tblprocess.EvaluatorId
		AND process.ProcessId = tblprocess.ProcessId

	UPDATE pa
	SET StatusId = (CASE 
			WHEN @today >= reviewer.StartDate
				AND (
					@today <= reviewer.EndDate
					OR @today <= @periodEndDate
					) THEN @PAInProgress
			WHEN @periodEndDate < @today
				AND reviewer.EndDate < @today THEN @PAClosed
			WHEN @today < reviewer.StartDate THEN @PAOpened
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE EXISTS (
			SELECT 1
			FROM #tblPARecord AS tbl
			WHERE tbl.PARecordId = pa.PARecordId
			)

	-- update if @overide = 1
	IF @Override = 1
	BEGIN
		INSERT INTO dbo.PAProcess (
			PARecordId
			,ProcessId
			,EvaluatorId
			,GradeId
			,RelatedPerformanceRecordIds
			)
		SELECT pa.PARecordId
			,process.ProcessId
			,process.EvaluatorId
			,process.GradeId
			,process.RelatedPerformanceRecordIds
		FROM #tblPAProcess AS process
			INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
		WHERE process.ProcessId = @PMProcessId
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS process WITH (NOLOCK)
				WHERE pa.PARecordId = process.PARecordId
					AND process.ProcessId = @PMProcessId
					AND process.EvaluatorId = process.EvaluatorId
				)

		UPDATE process
		SET process.GradeId = tblprocess.GradeId
			,process.RelatedPerformanceRecordIds = tblprocess.RelatedPerformanceRecordIds
		FROM dbo.PAProcess AS process WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			INNER JOIN #tblPAProcess AS tblprocess ON pa.PARecordId = tblprocess.PARecordId
			AND process.EvaluatorId = tblprocess.EvaluatorId
			AND process.ProcessId = tblprocess.ProcessId
	END

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)
	SET @xml = (
			SELECT process.PARecordId AS '@parid'
				,process.ProcessId AS '@pid'
				,process.EvaluatorId AS '@eid'
				,process.GradeId AS '@gid'
				,process.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS process WITH (NOLOCK)
				INNER JOIN #tblPARecord AS pa ON process.PARecordId = pa.PARecordId
			WHERE process.ProcessId >= @SelfProcessId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatedBy
		,@SPName = 'Period_GeneratePA'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@Duration = @Duration

	DROP TABLE #tblPARecord

	DROP TABLE #tblPAProcess
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PARecord_GetByReviewer] @ActorId INT
	,@PeriodId INT
	,@GroupIds VARCHAR(MAX) = ''
	,@IsRevised BIT = NULL
AS
/*
Created: 20181127@Oliver.Dang
Task: Get PA by Reviewer.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	INSERT INTO #tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblPARecordId (PARecordId)
	SELECT pa.PARecordId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.EvaluatorId = @ActorId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
		AND process.ProcessId = @ReviewerProcessId
		AND (
			@IsRevised IS NULL
			OR @IsRevised = ISNULL(pa.IsRevised, 0)
			)
	GROUP BY pa.PARecordId

	SELECT pa.PARecordId
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.Toeic
		,(CASE 
			WHEN pa.StatusId = @PAInProgress
				AND (
					@Today BETWEEN p.StartDate
						AND p.EndDate
					) THEN @PAInProgress
			ELSE @PAClosed END
			) AS StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,g.[Name] AS GroupName
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON emp.GroupId = g.GroupId
		INNER JOIN dbo.PAProcess AS p WITH (NOLOCK) ON pa.PARecordId = p.PARecordId
		AND p.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS es ON es.EmployeeId = emp.EmployeeId
	ORDER BY es.RankIndex

	SELECT process.PARecordId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
	WHERE process.ProcessId >= @SelfProcessId

	DROP TABLE #tblGroupId

	DROP TABLE #tblPARecordId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Announcement_Get] @EmployeeId INT
AS
/*
Created: 20181221@elton.ton
Task: get announcements
DB: bodb_NexGoal
Original:

Revisions:
	20190411@elton.ton: change date to send notifications before period start date
	20200226@drake.doi: get announcement for probation
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @today DATE = GETDATE()
		,@dateFormat VARCHAR(11) = 'dd MMM yyyy'
		,@PARejected TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		,@NominatorRoleId TINYINT = 1
		,@CommitteeRoleId TINYINT = 2
		,@SystemRole_HR TINYINT = 2

	CREATE TABLE #tblAnnouncement (
		[Message] NVARCHAR(MAX)
		,StartDate DATE
		)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The NexGoal system will capture all the performance records on ', FORMAT(p.StartDate, @dateFormat), '. You are recommended to create a neat & clean Performance Record for yourself BEFORE ', FORMAT(p.StartDate, @dateFormat), '. Please be advised that any record created after that time will NOT be valid for this PA period.')
		,DATEADD(DAY, - 28, p.StartDate)
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today >= DATEADD(DAY, - 28, p.StartDate)
		AND @today < p.StartDate
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EmployeeId = @EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
					INNER JOIN dbo.Employee e WITH (NOLOCK) ON gfs.GroupId = e.GroupId
					AND e.EmployeeId = @EmployeeId
				)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The NexGoal system will capture all the performance records on ', FORMAT(p.StartDate, @dateFormat), '. You are recommended to create a Performance Record for your members (with a detailed comment and also selected grade) BEFORE ', FORMAT(p.StartDate, @dateFormat), '. Please be advised that any record created after that time will NOT be valid for this PA period.')
		,DATEADD(DAY, - 7, p.StartDate)
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today >= DATEADD(DAY, - 7, p.StartDate)
		AND @today < p.StartDate
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EvaluatorId = @EmployeeId
					AND efs.ProcessId > @SelfProcessId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
				WHERE gfs.EvaluatorId = @EmployeeId
					AND gfs.ProcessId > @SelfProcessId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.PMSetting AS pmsetting WITH (NOLOCK)
				WHERE pmsetting.ProjectManagerId = @EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.PAProcess AS pap WITH (NOLOCK)
					INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
				WHERE par.PeriodId = p.PeriodId
					AND par.StatusId != @PARejected
					AND pap.ProcessId > @SelfProcessId
					AND pap.EvaluatorId = @EmployeeId
				)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The Performance Appraisal Process for the ', p.[Name], ' is IN PROGRESS and will end on ', FORMAT(p.EndDate, @dateFormat), ', please revise and submit the PA for your members.')
		,p.StartDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today BETWEEN p.StartDate
			AND p.EndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId
				AND par.StatusId != @PARejected
				AND pap.ProcessId = @ReviewerProcessId
				AND pap.EvaluatorId = @EmployeeId
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT ('The PA Focus for the ', p.[Name], ' is IN PROGRESS and will end on ', FORMAT(p.FocusEndDate, @dateFormat), ', please set the objectives for your members.')
		,p.FocusStartDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE @today BETWEEN p.FocusStartDate
			AND p.FocusEndDate
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
				INNER JOIN dbo.PARecord AS par WITH (NOLOCK) ON pap.PARecordId = pap.PARecordId
			WHERE par.PeriodId = p.PeriodId
				AND pap.ProcessId = @EvaluatorProcessId
				AND pap.EvaluatorId = @EmployeeId
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT (
			'The Nomination ', CASE e.[Quarter]
			WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', e.[Year])
			WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', e.[Year])
			WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', e.[Year])
			WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', e.[Year])
			ELSE CONCAT ('Employee of the Year - ', e.[Year]) END, ' process is IN PROGRESS and will end on ', FORMAT(e.EndDateForNominator, @dateFormat), ', please submit the vote for your nominee.'
			)
		,e.StartDateForNominator
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE @today BETWEEN e.StartDateForNominator
			AND e.EndDateForNominator
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId
				AND ens.NominatorId = @EmployeeId
				AND EXISTS (
					SELECT 1
					FROM dbo.func_SplitString(ens.RoleIds, ',') r
					WHERE r.val = @NominatorRoleId
					)
			)

	INSERT INTO #tblAnnouncement (
		[Message]
		,StartDate
		)
	SELECT CONCAT (
			'The Nomination process for the ', CASE e.[Quarter]
			WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', e.[Year])
			WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', e.[Year])
			WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', e.[Year])
			WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', e.[Year])
			ELSE CONCAT ('Employee of the Year - ', e.[Year]) END, ' is complete, please review the candidate list and choose the winner(s) for this award.'
			)
		,DATEADD(DAY, 1, e.EndDateForNominator)
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE @today > e.EndDateForNominator
		AND @today <= e.EndDateForCommittee
		AND EXISTS (
			SELECT 1
			FROM dbo.EOQYNominatorSetting AS ens WITH (NOLOCK)
			WHERE e.EOQYId = ens.EOQYId
				AND ens.NominatorId = @EmployeeId
				AND EXISTS (
					SELECT 1
					FROM dbo.func_SplitString(ens.RoleIds, ',') r
					WHERE r.val = @CommitteeRoleId
					)
			)

	DECLARE @NominationPeriodId INT
		,@Quarter TINYINT
		,@Year INT
		,@EndDateForCommittee DATE

	SELECT TOP 1 @NominationPeriodId = e.EOQYId
		,@Quarter = e.[Quarter]
		,@Year = e.[Year]
		,@EndDateForCommittee = e.EndDateForCommittee
	FROM dbo.EOQY AS e WITH (NOLOCK)
	WHERE e.EndDateForCommittee < @today
	ORDER BY e.EndDateForCommittee DESC

	IF @NominationPeriodId IS NOT NULL
	BEGIN
		DECLARE @Winners NVARCHAR(MAX) = STUFF((
					SELECT ', ' + e.NickName + ' - ' + e.FullName
					FROM dbo.Nomination AS n WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON n.NomineeId = e.EmployeeId
					WHERE n.EOQYId = @NominationPeriodId
						AND n.PerformanceRecordId IS NOT NULL
						AND n.StatusId = 3
					FOR XML PATH('')
					), 1, 2, '')

		IF @Winners IS NOT NULL
		BEGIN
			DECLARE @Title VARCHAR(200)

			SELECT @Title = CASE @Quarter
					WHEN 1 THEN CONCAT ('Employee of the Quarter - I ', @Year)
					WHEN 2 THEN CONCAT ('Employee of the Quarter - II ', @Year)
					WHEN 3 THEN CONCAT ('Employee of the Quarter - III ', @Year)
					WHEN 4 THEN CONCAT ('Employee of the Quarter - IV ', @Year)
					ELSE CONCAT ('Employee of the Year - ', @Year)
					END

			INSERT INTO #tblAnnouncement (
				[Message]
				,StartDate
				)
			VALUES (
				CONCAT ('The winners of the ', @Title, ': ', @Winners, '.')
				,DATEADD(DAY, 1, @EndDateForCommittee)
				)
		END
	END


	DECLARE @EmployeesInProbation NVARCHAR(200) = NULL

	SELECT @EmployeesInProbation = CONCAT(COALESCE(@EmployeesInProbation + ', ', ''), e.NickName)
	FROM ProbationEvaluation AS p WITH (NOLOCK)
		INNER JOIN Employee AS e WITH (NOLOCK) ON p.EmployeeId = e.EmployeeId
		INNER JOIN ProbationSetting  AS ps WITH (NOLOCK) ON p.EmployeeId = ps.EmployeeId AND ps.ProcessId = @EvaluatorProcessId
	WHERE e.JoinedDate = @today AND e.EmployeeId IS NOT NULL
		AND (
			EXISTS (
			SELECT 1
			FROM EmployeeRole AS er WITH (NOLOCK)
				INNER JOIN RefResource AS ref WITH (NOLOCK) 
					ON ref.RefName = 'SystemRole' AND ref.Value = 'HR' AND er.RoleId = ref.ResourceId
			WHERE er.EmployeeId = @EmployeeId
			)
			OR ps.EvaluatorId = @EmployeeId
		)

	IF(@EmployeesInProbation IS NOT NULL)
	BEGIN
		INSERT INTO #tblAnnouncement ([Message], StartDate)
			(SELECT CONCAT('The Probation evaluation process for ', @EmployeesInProbation, ' has been created on the system. Please visit the NexGoal site to review the Probation settings.'), @today) 
	END

	DECLARE @EmployeesStartReviewProbation NVARCHAR(200) = NULL

	SELECT @EmployeesStartReviewProbation = CONCAT(COALESCE(@EmployeesStartReviewProbation + ', ', ''), e.NickName)
	FROM ProbationEvaluation AS p WITH (NOLOCK)
		INNER JOIN Employee AS e WITH (NOLOCK) ON p.EmployeeId = e.EmployeeId
		INNER JOIN ProbationSetting  AS ps WITH (NOLOCK) ON p.EmployeeId = ps.EmployeeId
	WHERE ps.ProcessId = @ReviewerProcessId
		AND ps.FromDate = @today
		AND ps.EvaluatorId = @EmployeeId

	IF(@EmployeesStartReviewProbation IS NOT NULL)
	BEGIN
		INSERT INTO #tblAnnouncement ([Message], StartDate)
			(SELECT CONCAT('Your evaluation period for ', @EmployeesStartReviewProbation, ' has begun. To conduct a member assessment, please visit the NexGoal site and complete the evaluation form(s).'), @today) 
	END

	INSERT INTO #tblAnnouncement ([Message], StartDate)
		SELECT CONCAT('Your evaluation period will end at 23:59, ', ps.ToDate,'. Please visit the NexGoal site to complete the evaluation form.')
			,ps.ToDate
		FROM ProbationSetting AS ps WITH (NOLOCK)
		WHERE ps.ProcessId = @SelfProcessId
			AND ps.EmployeeId = @EmployeeId
			AND ps.FromDate =  @today
	
	SELECT [Message]
	FROM #tblAnnouncement WITH (NOLOCK)
	ORDER BY StartDate DESC

	DROP TABLE #tblAnnouncement
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PARecord_GetById] @ActorId INT
	,@PARecordId INT
AS
/*
Created: 20180920@Tim.Trinh
Task: Get PA by id.
DB: bodb_NexGoal
Original:

Revisions:
    20190129@Oliver.Dang: fix bugs
    20191118@elton.ton: get process name
	20200212@Tim.Trinh: get related performance record
Param's Explanation:

*/
BEGIN
	DECLARE @Today DATE = GETDATE()
		,@HRProcessId TINYINT = 1
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- PA status
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		-- role
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
	DECLARE @tblRelatedPerformanceRecord TABLE (
		PARecordId INT
		,ProcessId TINYINT
		,PerformanceRecordId INT
		)

	SELECT pa.PARecordId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.OriginalGradeId
		,pa.Toeic
		,pa.ToeicPolicy
		,DATEADD(year, 3, t.LRExamDate) ToeicExpiredDate
		,(CASE 
			WHEN @Today BETWEEN reviewer.StartDate
					AND reviewer.EndDate THEN @PAInProgress
			ELSE @PAClosed END
			) AS StatusId
		,pa.GradeId
		,pa.Comment
		,pa.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,pa.PeriodId
		,period.[Name]
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.TOEIC AS t WITH (NOLOCK) ON pa.EmployeeId = t.EmpId
	WHERE pa.PARecordId = @PARecordId

	SELECT process.PARecordId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.ProcessId
		,rr.Value AS Process
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON process.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId

	INSERT INTO @tblRelatedPerformanceRecord (
		PARecordId
		,ProcessId
		,PerformanceRecordId
		)
	SELECT process.PARecordId
		,process.ProcessId
		,child.val
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		CROSS APPLY dbo.func_SplitString(process.RelatedPerformanceRecordIds, ',') AS child
	WHERE process.PARecordId = @PARecordId
		AND process.ProcessId > @HRProcessId

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,creator.EmployeeId
		,creator.Code
		,creator.NickName
		,creator.FullName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON record.PerformanceRecordId = tbl.PerformanceRecordId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT record.PerformanceRecordId
		,pt.ProjectId
		,pt.[Name]
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON record.PerformanceRecordId = tbl.PerformanceRecordId
		OUTER APPLY dbo.func_SplitString(record.ProjectIds, ',') AS pr
		INNER JOIN dbo.ProjectTag AS pt WITH (NOLOCK) ON pr.val = pt.ProjectId

	SELECT reaction.PerformanceRecordId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON reaction.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	SELECT fb.PRFeedbackId
		,fb.PerformanceRecordId
		,fb.RoleId
		,fb.Content
		,fb.CreatedDate
		,fb.ModifiedDate
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedback AS fb WITH (NOLOCK)
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON fb.CreatorId = creator.EmployeeId

	SELECT reaction.PRFeedbackId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN @tblRelatedPerformanceRecord AS tbl ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRReaction_Insert]  @PerformanceRecordId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Agree member's PR and send notificaion
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT = 0
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	DECLARE @Today DATETIME = GETDATE()
	 ,@EmployeeId INT
	 ,@ReactionCreatorNickName NVARCHAR(20)
	 ,@PRCreatorId INT
	 ,@NotiStatus_Unread TINYINT = 0
	 ,@NotiAction_MyPage TINYINT = 2 -- My page with special performance checkin
	 ,@NotiAction_MemberPage TINYINT = 4 -- My page with special performance checkin
	 ,@NotiAction TINYINT
	 ,@NotiContent NVARCHAR(MAX) = ' likes your Performance Record '

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId, @PRCreatorId = pr.CreatorId,
		@NotiAction = CASE
			WHEN pr.RoleId = 2 THEN @NotiAction_MyPage
			ELSE @NotiAction_MemberPage
		END
	FROM PerformanceRecord pr WITH (NOLOCK)
	WHERE pr.PerformanceRecordId = @PerformanceRecordId

	IF EXISTS (
		SELECT 1
		FROM dbo.PRReaction WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one reaction is allowed per user'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.PerformanceRecord AS pr WITH (NOLOCK)
		WHERE pr.PerformanceRecordId = @PerformanceRecordId
			AND pr.CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Can not agree your PR'

		RETURN
	END

	SELECT @RoleId = CASE
			WHEN MAX(data.[Priority]) = 2 THEN @ReviewerProcessId
			WHEN MAX(data.[Priority]) = 3 THEN @EvaluatorProcessId
			ELSE @PMProcessId  
		END
	FROM
	(SELECT CASE
			WHEN ProcessId = @EvaluatorProcessId THEN 3
			WHEN ProcessId = @ReviewerProcessId THEN 2
			WHEN ProcessId = @PMProcessId THEN 1
			ELSE 0
		END AS [Priority]
	FROM dbo.func_GetMember(@CreatorId)
	WHERE EmployeeId = @EmployeeId) AS data

	IF (@CreatorId = @EmployeeId OR @RoleId = 0)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Permission denied'
		
		RETURN
	END

	SELECT @ReactionCreatorNickName = NickName
	FROM Employee WITH (NOLOCK)
	WHERE EmployeeId = @CreatorId

	IF(@NotiAction = @NotiAction_MyPage)
	BEGIN
		SET @NotiContent = @ReactionCreatorNickName + @NotiContent
	END
	ELSE
	BEGIN
		DECLARE @EmployeeNickName NVARCHAR(20)

		SELECT @EmployeeNickName = NickName
		FROM Employee WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId

		SET @NotiContent = @ReactionCreatorNickName + @NotiContent + 'for ' + @EmployeeNickName
	END

	INSERT INTO PRReaction (PerformanceRecordId, CreatorId, RoleId, CreatedDate)
		VALUES (@PerformanceRecordId, @CreatorId, @RoleId, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @PRCreatorId, @NotiContent , @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@ids'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRReaction_Insert'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_UpdateFinalPAGrade] @ActorId INT
	,@Value VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190219@Tim.Trinh
Task: update final PA grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
    Value : '<ROOT><Row PARecordId="4" OriginalGradeId="cipher text" GradeId="cipher text" ToeicPolicy="1"></Row></ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @CreatedDate DATETIME = GETDATE()
		,@PARecordId INT
		,@PerformanceRecordId INT
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@idoc INT

	CREATE TABLE #tblPARecord (
		PARecordId INT
		,OriginalGradeId VARCHAR(MAX)
		,GradeId VARCHAR(MAX)
		,ToeicPolicy SMALLINT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblPARecord (
		PARecordId
		,OriginalGradeId
		,GradeId
		,ToeicPolicy
		)
	SELECT PARecordId
		,OriginalGradeId
		,GradeId
		,ToeicPolicy
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (PARecordId INT, OriginalGradeId VARCHAR(MAX), GradeId VARCHAR(MAX), ToeicPolicy SMALLINT) AS data

	UPDATE pa
	SET pa.OriginalGradeId = tbl.OriginalGradeId
		,pa.GradeId = tbl.GradeId
		,pa.Toeic = toeic.Listening + toeic.Reading
		,pa.ToeicPolicy = tbl.ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblPARecord AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		LEFT JOIN dbo.TOEIC AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmpId

	UPDATE record
	SET record.GradeId = pa.GradeId
		,record.CreatedDate = @CreatedDate
	FROM dbo.PerformanceRecord AS record WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		INNER JOIN #tblPARecord AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId

	DELETE tbl
	FROM #tblPARecord AS tbl
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON tbl.PARecordId = pa.PARecordId
	WHERE pa.PerformanceRecordId IS NOT NULL
		OR pa.StatusId = @PAInProgress

	WHILE EXISTS (
			SELECT 1
			FROM #tblPARecord WITH (NOLOCK)
			)
	BEGIN
		SELECT TOP 1 @PARecordId = PARecordId
		FROM #tblPARecord WITH (NOLOCK)

		INSERT INTO dbo.PerformanceRecord (
			EmployeeId
			,CreatorId
			,GradeId
			,CreatedDate
			,TypeId
			,RoleId
			)
		SELECT EmployeeId
			,- 1
			,GradeId
			,@CreatedDate
			,4 -- system PA
			,255
		FROM dbo.PARecord WITH (NOLOCK)
		WHERE PARecordId = @PARecordId

		SET @PerformanceRecordId = SCOPE_IDENTITY()

		UPDATE pa
		SET pa.PerformanceRecordId = @PerformanceRecordId
		FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		WHERE pa.PARecordId = @PARecordId

		DELETE
		FROM #tblPARecord
		WHERE PARecordId = @PARecordId
	END

	DROP TABLE #tblPARecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Redmine_GetTeamWorkLoad] @EmployeeId INT
	,@PeriodId INT
AS
/*
Created: 20181001@Oliver.Dang
Task: Get member workload under leader
DB: bodb_NexGoal
Original:

Revisions:
- 20200224@drake.doi: fix perfomance issue
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @toDate DATE
		,@fromDate DATE
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT @fromDate = DisciplineStartDate
		,@toDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT EmployeeId
	INTO #member
	FROM dbo.func_GetMember(@EmployeeId)

	SELECT g.GroupId
		,g.[Name] AS GroupName
		,e.EmployeeId
		,e.NickName
		,p.[Name] AS ProjectName
		,p.ProjectId
		,p.[Hours] AS TotalHour
	FROM #member AS mem
		INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON mem.EmployeeId = e.EmployeeId
		INNER JOIN [dbo].[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN dbo.func_GetSortedEmployee(125) AS ee ON e.EmployeeId = ee.EmployeeId
		OUTER APPLY(
		SELECT p.ProjectId, p.Name, pst.Hours
		FROM	dbo.ProjectSpentTime AS pst WITH (NOLOCK)
		
		INNER JOIN dbo.Project AS p WITH (NOLOCK) ON pst.ProjectId = p.ProjectId
		WHERE e.EmployeeId = pst.EmployeeId
		AND pst.SpentOn BETWEEN @fromDate
			AND @toDate
		) AS p
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
	ORDER BY ee.RankIndex

	DROP TABLE #member
END



GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MasterReport_Get] @PeriodId INT
	,@ActorId INT
	,@GroupIds VARCHAR(MAX) = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
AS
/*
Created: 20190115@Elton.ton
Task: get periods
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PARejected TINYINT = 5
		,@SelfProcessId TINYINT = 2
	DECLARE @WorkingFromDate DATE
		,@WorkingToDate DATE
	DECLARE @tblGroupId TABLE (GroupId INT)
	DECLARE @tblEmployeeId TABLE (EmpId INT)
	DECLARE @tblPARecordId TABLE (PARecordId INT)

	SELECT @WorkingFromDate = DisciplineStartDate
		,@WorkingToDate = DATEADD(DAY, DATEDIFF(DAY, 0, DisciplineEndDate), '23:59:59')
	FROM dbo.Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	INSERT INTO @tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')

	INSERT INTO @tblEmployeeId (EmpId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')

	INSERT INTO @tblPARecordId (PARecordId)
	SELECT par.PARecordId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE par.StatusId <> @PARejected
		AND EXISTS (
			SELECT 1
			FROM @tblGroupId AS g
			WHERE e.GroupId = g.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS eid
			WHERE e.EmployeeId = eid.EmpId
			)
		AND par.PeriodId = @PeriodId

	SELECT pst.EmployeeId
		,SUM(pst.[Hours]) AS TotalSpentTime
	INTO #employeespenttime
	FROM dbo.ProjectSpentTime AS pst WITH (NOLOCK)
	WHERE pst.SpentOn BETWEEN @WorkingFromDate
			AND @WorkingToDate
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS e
			WHERE e.EmpId = pst.EmployeeId
			)
	GROUP BY pst.EmployeeId

	SELECT ec.EmpId
		,SUM(CASE 
				WHEN ISNULL(ec.IsOffDay, 0) = 0
					AND ISNULL(ec.IsHoliday, 0) = 0 THEN 1
				ELSE 0
				END) AS WorkingDay
		,SUM(CASE 
				WHEN ea.Absence IS NOT NULL
					AND ea.Absence = 0
					AND ec.IsLeaveDay = 0 THEN 1
				WHEN ea.Absence IS NOT NULL
					AND ea.Absence = 0
					AND ec.IsLeaveDay = 1
					AND ec.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) ActualWorkingDay
	INTO #workingday
	FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
		LEFT JOIN dbo.EmployeeAttendance AS ea WITH (NOLOCK) ON ec.EmpId = ea.EmpId
		AND ec.WorkDate = ea.AttendanceDate
	WHERE ec.WorkDate BETWEEN @WorkingFromDate
			AND @WorkingToDate
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS e
			WHERE e.EmpId = ec.EmpId
			)
	GROUP BY ec.EmpId

	SELECT e.EmployeeId
		,e.NickName
		,e.FullName
		,e.GroupId
		,g.[Name] AS GroupName
		,jt.[Name] AS JobTitle
		,par.PARecordId
		,par.OriginalGradeId
		,par.GradeId
		,par.Comment
		,par.Toeic
		,par.ToeicPolicy
		,paf.Contribution
		,paf.BonusGiven
		,paf.IsReadyPromotion
		,l.[Description] AS CurrentObjective
		,n.[Description] AS NextObjective
		,wd.WorkingDay
		,wd.ActualWorkingDay
		,est.TotalSpentTime
		,(CASE 
			WHEN e.EmployeeId = g.LeaderId THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT) END
			) AS IsGroupLeader
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN #workingday AS wd ON wd.EmpId = par.EmployeeId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) se ON par.EmployeeId = se.EmployeeId
		LEFT JOIN #employeespenttime AS est ON est.EmployeeId = par.EmployeeId
		LEFT JOIN dbo.PAFocus AS paf WITH (NOLOCK) ON paf.PeriodId = par.PeriodId
		AND paf.EmployeeId = par.EmployeeId
		LEFT JOIN dbo.EmployeeObjective AS l WITH (NOLOCK) ON l.ObjectiveId = paf.LastObjectiveId
		LEFT JOIN dbo.EmployeeObjective AS n WITH (NOLOCK) ON n.ObjectiveId = paf.NextObjectiveId
	WHERE EXISTS (
			SELECT 1
			FROM @tblPARecordId AS tbl
			WHERE par.PARecordId = tbl.PARecordId
			)
	ORDER BY se.RankIndex

	SELECT n.NomineeId
		,n.StatusId
		,rr.Value AS [Status]
		,eoq.[Quarter]
		,eoq.[Year]
	FROM dbo.Nomination AS n WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON n.NomineeId = e.EmployeeId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON n.StatusId = rr.ResourceId
		AND rr.RefName = 'NominationStatus'
		INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON n.EOQYId = eoq.EOQYId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.Period AS p WITH (NOLOCK)
			WHERE p.PeriodId = @PeriodId
				AND p.DisciplineStartDate <= eoq.EndDateForCommittee
				AND p.DisciplineEndDate >= eoq.EndDateForCommittee
			)
		AND EXISTS (
			SELECT 1
			FROM @tblGroupId AS g
			WHERE e.GroupId = g.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeId AS eid
			WHERE e.EmployeeId = eid.EmpId
			)
		AND eoq.EndDateForCommittee < @Today

	CREATE TABLE #tblRelatedPerformanceRecord (
		PARecordId INT
		,ProcessId TINYINT
		,PerformanceRecordId INT
		)

	SELECT pa.PARecordId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.ProcessId
		,process.EvaluatorId
		,evaluator.FullName
		,evaluator.NickName
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON pa.EmployeeId = e.EmployeeId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId >= @SelfProcessId
		INNER JOIN dbo.Employee AS evaluator WITH (NOLOCK) ON process.EvaluatorId = evaluator.EmployeeId
	WHERE EXISTS (
			SELECT 1
			FROM @tblPARecordId AS tbl
			WHERE pa.PARecordId = tbl.PARecordId
			)

	INSERT INTO #tblRelatedPerformanceRecord (
		PARecordId
		,ProcessId
		,PerformanceRecordId
		)
	SELECT pa.PARecordId
		,process.ProcessId
		,record.val
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId >= @SelfProcessId
		CROSS APPLY dbo.func_SplitString(process.RelatedPerformanceRecordIds, ',') AS record
	WHERE EXISTS (
			SELECT 1
			FROM @tblPARecordId AS tbl
			WHERE pa.PARecordId = tbl.PARecordId
			)

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.CreatedDate
		,record.RoleId
		,record.TypeId
		,creator.EmployeeId
		,creator.Code
		,creator.NickName
		,creator.FullName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT reaction.PerformanceRecordId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON reaction.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	SELECT fb.PRFeedbackId
		,fb.PerformanceRecordId
		,fb.RoleId
		,fb.Content
		,fb.CreatedDate
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedback AS fb WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON fb.CreatorId = creator.EmployeeId

	SELECT reaction.PRFeedbackId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #employeespenttime

	DROP TABLE #workingday

	DROP TABLE #tblRelatedPerformanceRecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedback_GetByPRId] @PerformanceRecordId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200220@Drake.Doi
Task: Get feedbacks by PR's ID and EmployeeCode
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT feedback.PRFeedbackId,
		feedback.PerformanceRecordId,
		feedback.RoleId,
		feedback.CreatedDate,
		feedback.Content,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedback AS feedback WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
	WHERE feedback.PerformanceRecordId = @PerformanceRecordId 
		AND creator.EmployeeId = @ActorId
		AND creator.Code = @ActorCode

	SELECT reaction.PRFeedbackId,
		reaction.PRFeedbackReactionId,
		reaction.CreatorId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN PRFeedback AS feedback WITH (NOLOCK) ON reaction.PRFeedbackId = feedback.PRFeedbackId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
	WHERE feedback.PerformanceRecordId = @PerformanceRecordId 
		AND creator.EmployeeId = @ActorId
		AND creator.Code = @ActorCode
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedbackReaction_Insert] @PRFeedbackId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Agree PR's feedback and send notification
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT = 0
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	DECLARE @Today DATETIME = GETDATE()
		,@PerformanceRecordId INT
		,@EmployeeId INT
		,@FeedbackCreatorId INT
		,@CreatorNickName NVARCHAR(20)
		,@NotiStatus_Unread TINYINT = 0
		,@NotiAction TINYINT = 4 -- Member page with special performance checkin
		,@NotiContent NVARCHAR(MAX) = ' likes your comment'

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId, @FeedbackCreatorId = fb.CreatorId, @PerformanceRecordId = fb.PerformanceRecordId
	FROM PRFeedback AS fb WITH (NOLOCK)
	INNER JOIN PerformanceRecord AS pr WITH (NOLOCK) ON fb.PerformanceRecordId = pr.PerformanceRecordId
	WHERE fb.PRFeedbackId = @PRFeedbackId

	IF EXISTS (
		SELECT 1
		FROM dbo.PRFeedbackReaction WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one reaction is allowed per user'

		RETURN
	END

	SELECT @RoleId = CASE
			WHEN MAX(data.[Priority]) = 2 THEN @ReviewerProcessId
			WHEN MAX(data.[Priority]) = 3 THEN @EvaluatorProcessId
			ELSE @PMProcessId  
		END
	FROM
	(SELECT CASE
			WHEN ProcessId = @EvaluatorProcessId THEN 3
			WHEN ProcessId = @ReviewerProcessId THEN 2
			WHEN ProcessId = @PMProcessId THEN 1
			ELSE 0
		END AS [Priority]
	FROM dbo.func_GetMember(@CreatorId)
	WHERE EmployeeId = @EmployeeId) AS data

	IF (@CreatorId = @EmployeeId OR @RoleId = 0)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Permission denied'
		
		RETURN
	END

	SELECT @CreatorNickName = NickName
	FROM Employee WITH (NOLOCK)
	WHERE EmployeeId = @CreatorId

	INSERT INTO PRFeedbackReaction (PRFeedbackId, CreatorId, RoleId, CreatedDate)
		VALUES (@PRFeedbackId, @CreatorId, @RoleId, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @FeedbackCreatorId, @CreatorNickName + @NotiContent, @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@ids'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedbackReaction_Insert'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedback_Insert] @PerformanceRecordId INT
	,@CreatorId INT
	,@Content NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: Comment on member's PR and send notification
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RoleId TINYINT = 0
		-- process
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	DECLARE @Today DATETIME = GETDATE()
		,@EmployeeId INT
		,@CreatorNickName NVARCHAR(20)
		 ,@NotiStatus_Unread TINYINT = 0
		 ,@NotiAction TINYINT = 2 -- My page with special performance checkin
		 ,@NotiContent NVARCHAR(MAX) = ' commented on your performance record'

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SELECT @EmployeeId = pr.EmployeeId
	FROM PerformanceRecord pr
	WHERE pr.PerformanceRecordId = @PerformanceRecordId

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PerformanceRecord WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND TypeId = 1 -- performance checkin
			AND RoleId = 2 -- self checkin
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Can not comment on this PR'

		RETURN
	END

	IF EXISTS (
		SELECT 1
		FROM dbo.PRFeedback WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Only one comment is allowed per user'

		RETURN
	END

	SELECT @RoleId = CASE
			WHEN MAX(data.[Priority]) = 2 THEN @ReviewerProcessId
			WHEN MAX(data.[Priority]) = 3 THEN @EvaluatorProcessId
			ELSE @PMProcessId  
		END
	FROM
	(SELECT CASE
			WHEN ProcessId = @EvaluatorProcessId THEN 3
			WHEN ProcessId = @ReviewerProcessId THEN 2
			WHEN ProcessId = @PMProcessId THEN 1
			ELSE 0
		END AS [Priority]
	FROM dbo.func_GetMember(@CreatorId)
	WHERE EmployeeId = @EmployeeId) AS data

	IF (@CreatorId = @EmployeeId OR @RoleId = 0)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Permission denied'

		RETURN
	END

	SELECT @CreatorNickName = NickName
	FROM Employee WITH (NOLOCK)
	WHERE EmployeeId = @CreatorId

	INSERT INTO dbo.PRFeedback(PerformanceRecordId, CreatorId, RoleId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @CreatorId, @RoleId, @Content, @Today)

	INSERT INTO [Notification] (RecordId, ActionTypeId, StatusId, CreatorId, EmployeeId, Content, CreatedDate)
		VALUES (@PerformanceRecordId, @NotiAction, @NotiStatus_Unread, @CreatorId, @EmployeeId, @CreatorNickName + @NotiContent, @Today)

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@id'
				,@EmployeeId AS '@eid'
				,@CreatorId AS '@cid'
				,@RoleId AS '@role'
				,@Content AS '@content'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedback_Insert'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Auto_PA_GetDataToUpdateGrade]
AS
/*
Created: 20181113@Tim.Trinh
Task: get data to update grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT pa.PARecordId
		,pa.EmployeeId
		,evaluator.GradeId
		,pa.StatusId
		,toeic.Score AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.IsActive = 1
		INNER JOIN dbo.PAProcess AS evaluator WITH (NOLOCK) ON pa.PARecordId = evaluator.PARecordId
		AND evaluator.ProcessId = @EvaluatorProcessId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAInProgress
		AND EXISTS (
			SELECT 1
			FROM dbo.PAProcess AS reviewer WITH (NOLOCK)
			WHERE pa.PARecordId = reviewer.PARecordId
				AND reviewer.ProcessId = @ReviewerProcessId
				AND reviewer.StartDate = @Today
			)
	
	UNION
	SELECT PARecordId
		,EmployeeId
		,GradeId
		,StatusId
		,0 AS ToeicPolicy
	FROM dbo.PARecord WITH (NOLOCK)
	WHERE StatusId = @PAClosed
		AND ISNULL(GradeId, '') = ''
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PerformanceRecord_GetByEmployee] @EmployeeId INT
AS
/*
Created: 20180912@Tim.Trinh
Task: Get PCI by employee.
DB: bodb_NexGoal
Original:

Revisions:
	20191118@elton.ton: get more information for system pa
	20200217@drake.doi: Get PR's feedback, reaction
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAClosed TINYINT = 4
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,IsDraft BIT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,TypeId TINYINT
		,EmployeeId INT
		,CreatorId INT
		,NominationPeriodId INT
		,NominationStatusId TINYINT
		,[Quarter] TINYINT
		,[Year] INT
		,PeriodId INT
		,PeriodName NVARCHAR(300)
		)

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,EmployeeId
		,CreatorId
		,NominationPeriodId
		,NominationStatusId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName
		)
	-- draft performance record
	SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
		,record.GradeId
		,record.Comment
		,1 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,emp.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON (CASE 
			WHEN record.EmployeeIds NOT LIKE '%[^0-9]%' THEN CAST(record.EmployeeIds AS INT)
			ELSE - 1 END
			) = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.CreatorId = @EmployeeId
	
	UNION
	-- performance record
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.TypeId = @Checkin
	
	UNION
	-- system PA
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,NULL AS NominationPeriodId
		,NULL AS NominationStatusId
		,NULL AS [Quarter]
		,NULL AS [Year]
		,p.PeriodId AS PeriodId
		,p.[Name] AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
		AND pa.StatusId = @PAClosed
		AND emp.EmployeeId = pa.EmployeeId
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE record.TypeId = @SystemPA
	
	UNION
	-- nomination result
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.TypeId
		,record.EmployeeId
		,record.CreatorId
		,nomination.EOQYId AS NominationPeriodId
		,nomination.StatusId AS NominationStatusId
		,period.[Quarter]
		,period.[Year]
		,NULL AS PeriodId
		,NULL AS PeriodName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.EmployeeId = @EmployeeId
		LEFT JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON record.PerformanceRecordId = nomination.PerformanceRecordId
		LEFT JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
		AND period.EndDateForCommittee IS NOT NULL
		AND period.EndDateForCommittee < @Today
	WHERE record.TypeId = @SystemNomination
		AND nomination.NominationId IS NOT NULL
		AND period.EOQYId IS NOT NULL

	SELECT PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,NominationStatusId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
		,NominationPeriodId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName AS [Name]
	FROM #tblPerformanceRecord AS record
		LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON record.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CASE 
			WHEN IsDraft = 1 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name]
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.ProjectTag AS prj WITH (NOLOCK) ON a.val = prj.ProjectId

	SELECT pa.PARecordId
		,pa.PerformanceRecordId
		,pa.OriginalGradeId
		,pa.Toeic
		,pa.ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
	WHERE pa.StatusId = @PAClosed
		AND pa.PerformanceRecordId IS NOT NULL

	SELECT pa.PARecordId
		,pap.ProcessId
		,rr.Value AS Process
		,pap.GradeId
		,pap.RelatedPerformanceRecordIds
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
		INNER JOIN dbo.PAProcess AS pap WITH (NOLOCK) ON pa.PARecordId = pap.PARecordId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON pap.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE pa.StatusId = @PAClosed
		AND pa.PerformanceRecordId IS NOT NULL
		AND pap.ProcessId > 1

	SELECT reaction.PRReactionId
		,reaction.PerformanceRecordId
		,reaction.RoleId
		,reaction.CreatedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND reaction.PerformanceRecordId = record.PerformanceRecordId
	ORDER BY reaction.CreatedDate ASC

	SELECT feedback.PRFeedbackId
		,feedback.PerformanceRecordId
		,feedback.Content
		,feedback.RoleId
		,feedback.CreatedDate
		,feedback.ModifiedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRFeedback AS feedback WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND feedback.PerformanceRecordId = record.PerformanceRecordId
	ORDER BY feedback.CreatedDate ASC

	SELECT reaction.PRFeedbackReactionId
		,reaction.PRFeedbackId
		,reaction.CreatorId
		,reaction.RoleId
		,reaction.CreatedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN PRFeedback AS feedback WITH (NOLOCK) ON feedback.PRFeedbackId = reaction.PRFeedbackId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND feedback.PerformanceRecordId = record.PerformanceRecordId
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #tblPerformanceRecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformanceByMemberId] @ActorId INT
	,@EmployeeId INT
AS
/*
Created: 20181026@Tim.Trinh
Task: Get PCI for a member.
DB: bodb_NexGoal
Original:

Revisions:
	20191118@elton.ton: get more information for system pa
	20200217@drake.doi: Get PR's feedback, reaction
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@EmployeeIdString VARCHAR(20)
		,@PAClosed TINYINT = 4
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
	-- Performance Record Type
	DECLARE @Checkin TINYINT = 1
		,@SystemPA TINYINT = 4
		,@SystemNomination TINYINT = 5
		,@IsHost BIT = 0

	SET @EmployeeIdString = CAST(@EmployeeId AS VARCHAR(20))

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,IsDraft BIT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,NominationStatusId TINYINT
		,TypeId TINYINT
		,NominationPeriodId INT
		,[Quarter] TINYINT
		,[Year] INT
		,PeriodId INT
		,PeriodName NVARCHAR(300)
		)

	SELECT @IsHost = MAX(data.IsHost)
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE EmployeeId = @EmployeeId
			AND ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		) AS data
	GROUP BY data.EmployeeId

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,TypeId
		,NominationStatusId
		,NominationPeriodId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName
		)
	SELECT data.PerformanceRecordId
		,data.GradeId
		,data.Comment
		,data.IsDraft
		,data.ProjectIds
		,data.CreatedDate
		,data.ModifiedDate
		,data.RoleId
		,data.EmployeeIds
		,data.CreatorId
		,data.TypeId
		,data.NominationStatusId
		,data.NominationPeriodId
		,data.[Quarter]
		,data.[Year]
		,data.PeriodId
		,data.PeriodName
	FROM (
		SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
			,record.GradeId
			,record.Comment
			,1 AS IsDraft
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,record.EmployeeIds
			,record.CreatorId
			,record.TypeId
			,NULL AS NominationStatusId
			,NULL AS NominationPeriodId
			,NULL AS [Quarter]
			,NULL AS [Year]
			,NULL AS PeriodId
			,NULL AS PeriodName
		FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
		WHERE record.EmployeeIds <> CAST(record.CreatorId AS VARCHAR(20))
			AND record.CreatorId = @ActorId
			AND (
				@EmployeeIdString = ''
				OR record.EmployeeIds LIKE @EmployeeIdString
				OR record.EmployeeIds LIKE @EmployeeIdString + ',%'
				OR record.EmployeeIds LIKE '%,' + @EmployeeIdString + ',%'
				OR record.EmployeeIds LIKE '%,' + @EmployeeIdString
				)
		
		UNION
		SELECT record.PerformanceRecordId
			,record.GradeId
			,record.Comment
			,0 AS IsDraft
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,CAST(record.EmployeeId AS VARCHAR(20))
			,record.CreatorId
			,record.TypeId
			,NULL AS NominationStatusId
			,NULL AS NominationPeriodId
			,NULL AS [Quarter]
			,NULL AS [Year]
			,p.PeriodId
			,p.[Name] AS PeriodName
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
			LEFT JOIN dbo.PARecord AS pa WITH (NOLOCK) ON record.PerformanceRecordId = pa.PerformanceRecordId
			AND emp.EmployeeId = pa.EmployeeId
			AND pa.StatusId = @PAClosed
			LEFT JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
			AND p.IsActive = 1
		WHERE record.EmployeeId = @EmployeeId
			AND (
				record.TypeId = @Checkin
				OR (
					record.TypeId = @SystemPA
					AND pa.PerformanceRecordId IS NOT NULL
					AND p.PeriodId IS NOT NULL
					)
				)
			AND (
				@IsHost = 1
				OR (
					@IsHost = 0
					AND (
						record.CreatorId = @EmployeeId
						OR record.CreatorId = @ActorId
						)
					)
				)
		
		UNION
		SELECT record.PerformanceRecordId
			,record.GradeId
			,record.Comment
			,0 AS IsDraft
			,record.ProjectIds
			,record.CreatedDate
			,record.ModifiedDate
			,record.RoleId
			,CAST(record.EmployeeId AS VARCHAR(20))
			,record.CreatorId
			,record.TypeId
			,nomination.StatusId AS NominationStatusId
			,nomination.EOQYId AS NominationPeriodId
			,period.[Quarter]
			,period.[Year]
			,NULL AS PeriodId
			,NULL AS PeriodName
		FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
			INNER JOIN dbo.Nomination AS nomination WITH (NOLOCK) ON emp.EmployeeId = nomination.NomineeId
			AND record.PerformanceRecordId = nomination.PerformanceRecordId
			INNER JOIN dbo.EOQY AS period WITH (NOLOCK) ON nomination.EOQYId = period.EOQYId
			AND period.EndDateForCommittee < @Today
		WHERE emp.EmployeeId = @EmployeeId
			AND record.TypeId = @SystemNomination
			AND record.CreatorId = - 1
		) AS data

	SELECT PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,NominationStatusId
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
		,NominationPeriodId
		,[Quarter]
		,[Year]
		,PeriodId
		,PeriodName AS [Name]
	FROM #tblPerformanceRecord AS record
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CASE 
			WHEN IsDraft = 1 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT record.PerformanceRecordId AS LinkObjectId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.EmployeeIds, ',') a
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON a.val = e.EmployeeId

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name]
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.ProjectTag AS prj WITH (NOLOCK) ON a.val = prj.ProjectId

	SELECT pa.PARecordId
		,pa.PerformanceRecordId
		,pa.OriginalGradeId
		,pa.Toeic
		,pa.ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
	WHERE pa.StatusId = @PAClosed
		AND pa.PerformanceRecordId IS NOT NULL

	SELECT pa.PARecordId
		,pap.ProcessId
		,rr.Value AS Process
		,pap.GradeId
		,pap.RelatedPerformanceRecordIds
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS p WITH (NOLOCK) ON pa.PeriodId = p.PeriodId
		AND p.IsActive = 1
		INNER JOIN dbo.PAProcess pap WITH (NOLOCK) ON pa.PARecordId = pap.PARecordId
		INNER JOIN dbo.RefResource rr WITH (NOLOCK) ON pap.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE pa.StatusId = @PAClosed
		AND pa.PerformanceRecordId IS NOT NULL
		AND pap.ProcessId > 1

	SELECT reaction.PRReactionId
		,reaction.PerformanceRecordId
		,reaction.RoleId
		,reaction.CreatedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND reaction.PerformanceRecordId = record.PerformanceRecordId
	WHERE reaction.CreatorId = @ActorId
		OR @IsHost = 1
	ORDER BY reaction.CreatedDate ASC

	SELECT feedback.PRFeedbackId
		,feedback.PerformanceRecordId
		,feedback.Content
		,feedback.RoleId
		,feedback.CreatedDate
		,feedback.ModifiedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRFeedback AS feedback WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND feedback.PerformanceRecordId = record.PerformanceRecordId
	WHERE feedback.CreatorId = @ActorId
		OR @IsHost = 1
	ORDER BY feedback.CreatedDate ASC

	SELECT reaction.PRFeedbackReactionId
		,reaction.PRFeedbackId
		,reaction.CreatorId
		,reaction.RoleId
		,reaction.CreatedDate
		,creator.EmployeeId
		,creator.Code
		,creator.FullName
		,creator.NickName
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN PRFeedback AS feedback WITH (NOLOCK) ON feedback.PRFeedbackId = reaction.PRFeedbackId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0
		AND feedback.PerformanceRecordId = record.PerformanceRecordId
	WHERE reaction.CreatorId = @ActorId
		OR feedback.CreatorId = @ActorId
		OR @IsHost = 1
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #tblPerformanceRecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_Delete] @PeriodId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180731@Elton.Ton
Task: delete period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE	@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	IF EXISTS (
			SELECT 1
			FROM dbo.PARecord WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
				AND StatusId IN (@PAOpened, @PAInProgress, @PAClosed)
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The period has already generated PA.'

		RETURN
	END

	SET @xml = (
			SELECT PeriodId AS '@pi'
				,[Name] AS '@n'
				,TargetTypeId AS '@tti'
				,[Description] AS '@d'
				,StartDate AS '@sd'
				,EndDate AS '@ed'
				,DisciplineStartDate AS '@dsd'
				,DisciplineEndDate AS '@ded'
				,FocusStartDate AS '@fsd'
				,IsActive AS '@ia'
				,CreatedBy AS '@cb'
				,CreatedDate AS '@cd'
				,ModifiedBy AS '@mb'
				,ModifiedDate AS '@md'
			FROM dbo.Period WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	DELETE pap
	FROM dbo.PAProcess AS pap
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PARecordId = pap.PARecordId
				AND par.PeriodId = @PeriodId
			)

	DELETE dbo.PARecord
	WHERE PeriodId = @PeriodId

	DELETE dbo.Period
	WHERE PeriodId = @PeriodId

	DELETE dbo.PeriodProcessSetting
	WHERE PeriodId = @PeriodId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_Delete'
		,@ObjectId = @PeriodId
		,@OldData = @logDataOld
		,@Duration = @Duration
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_GetDataToUpdateFinalPAGrade] @PeriodId INT
	,@PARecordIds VARCHAR(MAX) = NULL
AS
/*
Created: 201902193@Tim.Trinh
Task: get data to update final grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@EvaluatorProcessId TINYINT = 4

	CREATE TABLE #tblPARecordIds (PARecordId INT)

	INSERT INTO #tblPARecordIds (PARecordId)
	SELECT val
	FROM dbo.func_SplitString(@PARecordIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT pa.PARecordId
		,pa.OriginalGradeId
		,pa.GradeId
		,pa.StatusId
		,ISNULL(toeic.Score, 0) AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.PeriodId = @PeriodId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAClosed
		AND (
			@PARecordIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tblPARecordIds AS tbl
				WHERE pa.PARecordId = tbl.PARecordId
				)
			)
	
	UNION
	-- reset grade if reviewer is not revised
	SELECT pa.PARecordId
		,NULL AS OriginalGradeId
		,process.GradeId
		,pa.StatusId
		,ISNULL(toeic.Score, 0) AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @EvaluatorProcessId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE pa.StatusId = @PAInProgress
		AND pa.ModifiedBy IS NULL
		AND ISNULL(pa.IsRevised, 0) = 0
		AND (
			@PARecordIds IS NULL
			OR EXISTS (
				SELECT 1
				FROM #tblPARecordIds AS tbl
				WHERE pa.PARecordId = tbl.PARecordId
				)
			)

	DROP TABLE #tblPARecordIds
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_GetEmployeeSettings] @PeriodId INT
	,@GroupId INT = NULL
	,@IsEvaluated BIT = NULL
	,@StatusId TINYINT = NULL
AS
/*
Created: 20180823@Elton.Ton
Task: get employee settings
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PARecordTbl TABLE (Id INT PRIMARY KEY)

	INSERT INTO @PARecordTbl
	SELECT par.PARecordId
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
	WHERE par.PeriodId = @PeriodId
		AND (
			@StatusId IS NULL
			OR par.StatusId = @StatusId
			)
		AND (
			@GroupId IS NULL
			OR g.GroupId = @GroupId
			)
		AND (
			@IsEvaluated IS NULL
			OR par.IsRevised = @IsEvaluated
			)

	SELECT par.PARecordId
		,par.PeriodId
		,e.EmployeeId
		,par.StatusId
		,rr.Value AS [Status]
	FROM dbo.PARecord AS par WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON par.EmployeeId = e.EmployeeId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON par.StatusId = rr.ResourceId
		AND rr.RefName = 'PerformanceStatus'
		INNER JOIN @PARecordTbl AS tbl ON par.PARecordId = tbl.Id
		INNER JOIN dbo.func_GetSortedEmployee(NULL) AS ee ON e.EmployeeId = ee.EmployeeId
	ORDER BY ee.RankIndex

	SELECT pap.PARecordId
		,pap.ProcessId
		,rr.Value AS Process
		,pap.EvaluatorId
		,pap.StartDate
		,pap.EndDate
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN @PARecordTbl AS tbl ON pap.PARecordId = tbl.Id
		INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON pap.ProcessId = rr.ResourceId
		AND rr.RefName = 'PerformanceProcess'
	WHERE pap.ProcessId <> 2
		AND pap.ProcessId <> 3
	ORDER BY pap.PARecordId
		,pap.ProcessId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_UpdateEmployeeSettings] @PeriodId INT
	,@Settings VARCHAR(MAX)
	,@PARecordIds VARCHAR(MAX)
	,@ModifiedBy INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180904@Elton.Ton
Task: Update period employee setting.
DB: bodb_NexGoal
Original:

Revisions:
	20200221@Tim.Trinh: get related performance record
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)
		,@xml XML
		,@docHandler INT
	--
	DECLARE @Today DATE = GETDATE()
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	CREATE TABLE #changedProcessTbl (
		PARecordId INT
		,ProcessId TINYINT
		)

	EXEC sp_xml_preparedocument @docHandler OUTPUT
		,@Settings

	DECLARE @tblEmployeeSetting TABLE (
		ProcessId TINYINT
		,EvaluatorId INT
		,StartDate DATE
		,EndDate DATE
		)
	DECLARE @tblPARecord TABLE (
		PARecordId INT
		,EmployeeId INT
		)

	INSERT INTO @tblPARecord (
		PARecordId
		,EmployeeId
		)
	SELECT pa.PARecordId
		,pa.EmployeeId
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.func_SplitString(@PARecordIds, ',') AS func ON ISNUMERIC(func.val) = 1
		AND func.val = pa.PARecordId

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
				,pap.GradeId AS '@gid'
				,pap.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @tblPARecord AS pari
					WHERE pap.PARecordId = pari.PARecordId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	INSERT INTO @tblEmployeeSetting (
		ProcessId
		,EvaluatorId
		,StartDate
		,EndDate
		)
	SELECT ProcessId
		,NULLIF(EvaluatorId, '')
		,NULLIF(StartDate, '')
		,NULLIF(EndDate, '')
	FROM OPENXML(@docHandler, '/Root/Row', 1) WITH (ProcessId TINYINT, EvaluatorId INT, StartDate DATE, EndDate DATE)

	INSERT INTO #changedProcessTbl (
		PARecordId
		,ProcessId
		)
	SELECT pap.PARecordId
		,pap.ProcessId
	FROM dbo.PAProcess AS pap WITH (NOLOCK)
		INNER JOIN @tblPARecord AS pari ON pap.PARecordId = pari.PARecordId
		INNER JOIN @tblEmployeeSetting AS es ON es.ProcessId = pap.ProcessId
		AND es.EvaluatorId IS NOT NULL
	WHERE pap.EvaluatorId <> es.EvaluatorId
		AND pap.ProcessId >= @EvaluatorProcessId

	UPDATE pap
	SET EvaluatorId = ISNULL(es.EvaluatorId, pap.EvaluatorId)
		,StartDate = ISNULL(es.StartDate, pap.StartDate)
		,EndDate = ISNULL(es.EndDate, pap.EndDate)
	FROM dbo.PAProcess AS pap WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblPARecord AS pari ON pap.PARecordId = pari.PARecordId
		INNER JOIN @tblEmployeeSetting AS es ON es.ProcessId = pap.ProcessId
	WHERE pap.ProcessId >= @EvaluatorProcessId

	-- update PA status
	UPDATE pa
	SET pa.StatusId = (CASE 
			WHEN @Today >= reviewer.StartDate
				AND (
					@Today <= reviewer.EndDate
					OR @Today <= period.EndDate
					)
				AND pa.StatusId <> @PAInProgress THEN @PAInProgress
			WHEN period.EndDate < @Today
				AND reviewer.EndDate < @Today
				AND pa.StatusId <> @PAClosed THEN @PAClosed
			WHEN @Today < reviewer.StartDate THEN @PAOpened
			ELSE pa.StatusId END
			)
	FROM dbo.PARecord AS pa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblPARecord AS paid ON pa.PARecordId = paid.PARecordId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON pa.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE pa.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)

	IF EXISTS (
			SELECT 1
			FROM #changedProcessTbl
			)
	BEGIN
		DECLARE @workingFromDate DATE
			,@workingToDate DATE
			,@CheckinTypeId TINYINT = 1

		SELECT @workingFromDate = p.DisciplineStartDate
			,@workingToDate = p.DisciplineEndDate
		FROM dbo.Period AS p WITH (NOLOCK)
		WHERE p.PeriodId = @PeriodId

		-- update for Reviewer
		UPDATE process
		SET process.GradeId = lg.GradeId
			,process.RelatedPerformanceRecordIds = STUFF((
					SELECT ',' + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.CreatorId = process.EvaluatorId
						AND prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = process.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, N'')
		FROM dbo.PAProcess AS process WITH (NOLOCK)
			INNER JOIN @tblPARecord AS pa ON pa.PARecordId = process.PARecordId
			AND process.ProcessId = @ReviewerProcessId
			OUTER APPLY (
			SELECT TOP 1 record.GradeId
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.CreatorId = process.EvaluatorId
				AND record.EmployeeId = pa.EmployeeId
				AND record.CreatedDate BETWEEN @workingFromDate
					AND @workingToDate
				AND record.RoleId = process.ProcessId
				AND ISNULL(record.GradeId, '') <> ''
			ORDER BY record.CreatedDate DESC
			) AS lg
		WHERE EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE record.TypeId = @CheckinTypeId
					AND record.CreatorId = process.EvaluatorId
					AND record.EmployeeId = pa.EmployeeId
					AND record.CreatedDate BETWEEN @workingFromDate
						AND @workingToDate
					AND record.RoleId = process.ProcessId
				)

		UPDATE process
		SET process.GradeId = lg.GradeId
			,process.RelatedPerformanceRecordIds = STUFF((
					SELECT ',' + CAST(prcmt.PerformanceRecordId AS VARCHAR(20))
					FROM dbo.PerformanceRecord AS prcmt WITH (NOLOCK)
					WHERE prcmt.CreatorId = process.EvaluatorId
						AND prcmt.EmployeeId = pa.EmployeeId
						AND prcmt.TypeId = @CheckinTypeId
						AND prcmt.CreatedDate BETWEEN @workingFromDate
							AND @workingToDate
						AND prcmt.RoleId = process.ProcessId
					ORDER BY prcmt.CreatedDate DESC
					FOR XML PATH('')
					), 1, 1, N'')
		FROM dbo.PAProcess AS process WITH (NOLOCK)
			INNER JOIN @tblPARecord AS pa ON pa.PARecordId = process.PARecordId
			AND process.ProcessId = @EvaluatorProcessId
			OUTER APPLY (
			SELECT TOP 1 record.GradeId
			FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
			WHERE record.TypeId = @CheckinTypeId
				AND record.EmployeeId = pa.EmployeeId
				AND record.RoleId = @EvaluatorProcessId
				AND record.CreatedDate <= @workingToDate
				AND ISNULL(record.GradeId, '') <> ''
			ORDER BY record.CreatedDate DESC
			) AS lg
		WHERE EXISTS (
				SELECT 1
				FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
				WHERE record.TypeId = @CheckinTypeId
					AND record.EmployeeId = pa.EmployeeId
					AND record.RoleId = @EvaluatorProcessId
					AND (
						(
							record.CreatorId = process.EvaluatorId
							AND record.CreatedDate BETWEEN @workingFromDate
								AND @workingToDate
							)
						OR (
							record.CreatedDate < @workingFromDate
							AND ISNULL(record.GradeId, '') <> ''
							)
						)
				)
	END

	SET @xml = (
			SELECT pap.PARecordId AS '@pari'
				,pap.ProcessId AS '@pi'
				,pap.EvaluatorId AS '@ei'
				,pap.StartDate AS '@sd'
				,pap.EndDate AS '@ed'
				,pap.GradeId AS '@gid'
				,pap.RelatedPerformanceRecordIds AS '@prids'
			FROM dbo.PAProcess AS pap WITH (NOLOCK)
			WHERE EXISTS (
					SELECT 1
					FROM @tblPARecord AS pari
					WHERE pap.PARecordId = pari.PARecordId
					)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateEmployeeSettings'
		,@ObjectId = @PeriodId
		,@Data = @logData
		,@OldData = @logOldData
		,@Duration = @Duration

	DROP TABLE #changedProcessTbl
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_GetPARecordsToUpdateGrade] @PARecordIds VARCHAR(MAX)
AS
/*
Created: 20181126@Elton.ton
Task: get data to update grade.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EvaluatorProcessId TINYINT = 4

	SELECT pa.PARecordId
		,pa.EmployeeId
		,evaluator.GradeId
		,toeic.Score AS ToeicPolicy
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.PAProcess AS evaluator WITH (NOLOCK) ON pa.PARecordId = evaluator.PARecordId
		AND evaluator.ProcessId = @EvaluatorProcessId
		LEFT JOIN dbo.ToeicPolicy AS toeic WITH (NOLOCK) ON pa.EmployeeId = toeic.EmployeeId
	WHERE period.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.func_SplitString(@PARecordIds, ',') part
			WHERE pa.PARecordId = part.val
			)
END



GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Period_UpdateEmployeeSettingStatus] @PARecordId INT
	,@IsExcluded BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(2000) = '' OUTPUT
AS
/*
Created: 20180910@Elton.Ton
Task: update employee setting status.
DB: bodb_NexGoal
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
	DECLARE @today DATE = GETDATE()
		,@PANotGenerated TINYINT = 1
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@PARejected TINYINT = 5
		,@periodEndDate DATE
	DECLARE @ReviewerProcessId TINYINT = 5

	SET @xml = (
			SELECT par.PARecordId AS '@pari'
				,par.StatusId AS '@si'
			FROM dbo.PARecord par WITH (NOLOCK)
			WHERE par.PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	SELECT @periodEndDate = p.EndDate
	FROM dbo.Period AS p WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM dbo.PARecord AS par WITH (NOLOCK)
			WHERE par.PeriodId = p.PeriodId
				AND par.PARecordId = @PARecordId
			)

	UPDATE par
	SET StatusId = (CASE 
			WHEN @IsExcluded = 1 THEN @PARejected
			WHEN @today >= reviewer.StartDate
				AND (
					@today <= reviewer.EndDate
					OR @today <= @periodEndDate
					) THEN @PAInProgress
			WHEN @periodEndDate < @today
				AND reviewer.EndDate < @today THEN @PAClosed
			WHEN @today < reviewer.StartDate THEN @PAOpened
			ELSE @PANotGenerated END
			)
	FROM dbo.PARecord AS par WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.PAProcess AS reviewer WITH (NOLOCK) ON par.PARecordId = reviewer.PARecordId
		AND reviewer.ProcessId = @ReviewerProcessId
	WHERE par.PARecordId = @PARecordId

	SET @xml = (
			SELECT par.PARecordId AS '@pari'
				,par.StatusId AS '@si'
			FROM dbo.PARecord par WITH (NOLOCK)
			WHERE par.PARecordId = @PARecordId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @ActorId = @ModifiedBy
		,@SPName = 'Period_UpdateEmployeeSettingStatus'
		,@ObjectId = @PARecordId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PerformanceRecord_GetMemberPerformance] @ActorId INT
AS
/*
Created: 20181026@Tim.Trinh
Task: Get member PCI.
DB: bodb_NexGoal
Original:

Revisions:
- 20200217@drake.doi: Get PR's feedback, reaction
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@Checkin TINYINT = 1

	CREATE TABLE #tblMember (
		EmployeeId INT
		,IsHost BIT
		)

	CREATE TABLE #tblPerformanceRecord (
		PerformanceRecordId INT
		,GradeId VARCHAR(MAX)
		,Comment NVARCHAR(MAX)
		,IsDraft BIT
		,ProjectIds VARCHAR(MAX)
		,CreatedDate DATETIME
		,ModifiedDate DATETIME
		,RoleId TINYINT
		,EmployeeIds VARCHAR(MAX)
		,CreatorId INT
		,TypeId TINYINT
		)

	INSERT INTO #tblMember (
		EmployeeId
		,IsHost
		)
	SELECT data.EmployeeId
		,MAX(data.IsHost)
	FROM (
		SELECT EmployeeId
			,(CASE 
				WHEN ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId) THEN 1
				ELSE 0 END
				) AS IsHost
		FROM dbo.func_GetMember(@ActorId)
		WHERE ProcessId IN (@PMProcessId, @EvaluatorProcessId, @ReviewerProcessId)
		) AS data
	GROUP BY data.EmployeeId

	INSERT INTO #tblPerformanceRecord (
		PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,ProjectIds
		,CreatedDate
		,ModifiedDate
		,RoleId
		,EmployeeIds
		,CreatorId
		,TypeId
		)
	SELECT record.DraftPerformanceRecordId AS PerformanceRecordId
		,record.GradeId
		,record.Comment
		,1 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,record.EmployeeIds
		,record.CreatorId
		,record.TypeId
	FROM dbo.DraftPerformanceRecord AS record WITH (NOLOCK)
	WHERE record.EmployeeIds <> CAST(record.CreatorId AS VARCHAR(20))
		AND record.CreatorId = @ActorId
	
	UNION
	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,0 AS IsDraft
		,record.ProjectIds
		,record.CreatedDate
		,record.ModifiedDate
		,record.RoleId
		,CAST(record.EmployeeId AS VARCHAR(20))
		,record.CreatorId
		,record.TypeId
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		AND emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
		INNER JOIN #tblMember AS member WITH (NOLOCK) ON emp.EmployeeId = member.EmployeeId
	WHERE record.TypeId = @Checkin
		AND (
			member.IsHost = 1
			OR (
				member.IsHost = 0
				AND (
					record.CreatorId = member.EmployeeId
					OR record.CreatorId = @ActorId
					)
				)
			)

	SELECT PerformanceRecordId
		,GradeId
		,Comment
		,IsDraft
		,CreatedDate
		,ModifiedDate
		,RoleId
		,TypeId
		,record.CreatorId AS EmployeeId
		,cre.Code
		,cre.NickName
		,cre.FullName
	FROM #tblPerformanceRecord AS record
		LEFT JOIN dbo.Employee AS cre WITH (NOLOCK) ON record.CreatorId = cre.EmployeeId
	ORDER BY CASE 
			WHEN IsDraft = 1 THEN 1
			ELSE 2
			END
		,CreatedDate DESC

	SELECT record.PerformanceRecordId AS LinkObjectId
		,e.EmployeeId
		,e.Code
		,e.NickName
		,e.FullName
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.EmployeeIds, ',') a
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON a.val = e.EmployeeId

	SELECT record.PerformanceRecordId
		,prj.ProjectId
		,prj.[Name]
	FROM #tblPerformanceRecord AS record
		OUTER APPLY [dbo].[func_SplitString](record.ProjectIds, ',') a
		INNER JOIN dbo.ProjectTag AS prj WITH (NOLOCK) ON a.val = prj.ProjectId

	SELECT reaction.PRReactionId,
		reaction.PerformanceRecordId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0 AND reaction.PerformanceRecordId = record.PerformanceRecordId 
		INNER JOIN #tblMember AS mem ON mem.EmployeeId = CAST(record.EmployeeIds AS VARCHAR(20))
	WHERE reaction.CreatorId = @ActorId
		OR mem.IsHost = 1
	ORDER BY reaction.CreatedDate ASC

	SELECT feedback.PRFeedbackId,
		feedback.PerformanceRecordId,
		feedback.Content,
		feedback.RoleId,
		feedback.CreatedDate,
		feedback.ModifiedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedback AS feedback WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = feedback.CreatorId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0 AND feedback.PerformanceRecordId = record.PerformanceRecordId  
		INNER JOIN #tblMember AS mem ON mem.EmployeeId = CAST(record.EmployeeIds AS VARCHAR(20))
	WHERE feedback.CreatorId = @ActorId
		OR mem.IsHost = 1
	ORDER BY feedback.CreatedDate ASC

	SELECT reaction.PRFeedbackReactionId,
		reaction.PRFeedbackId,
		reaction.CreatorId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
		INNER JOIN PRFeedback AS feedback WITH (NOLOCK) ON feedback.PRFeedbackId = reaction.PRFeedbackId
		INNER JOIN #tblPerformanceRecord AS record ON record.IsDraft = 0 AND feedback.PerformanceRecordId = record.PerformanceRecordId 
		INNER JOIN #tblMember AS mem ON mem.EmployeeId = CAST(record.EmployeeIds AS VARCHAR(20))
	WHERE reaction.CreatorId = @ActorId
		OR feedback.CreatorId = @ActorId
		OR mem.IsHost = 1
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #tblMember

	DROP TABLE #tblPerformanceRecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedbackReaction_GetByFeedbackId] @PRFeedbackId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200213@Drake.Doi
Task: Get feedback reactions by feedback's ID
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT reaction.PRFeedbackReactionId,
		reaction.PRFeedbackId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
	WHERE reaction.PRFeedbackId = @PRFeedbackId
		AND creator.Code = @ActorCode
		AND creator.EmployeeId = @ActorId
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRReaction_GetByPRId] @PerformanceRecordId INT
	,@ActorId INT
	,@ActorCode VARCHAR(20)
AS

/*
Created: 20200213@Drake.Doi
Task: Get reactions by PR's ID
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;

	SELECT reaction.PRReactionId,
		reaction.PerformanceRecordId,
		reaction.RoleId,
		reaction.CreatedDate,
		creator.EmployeeId,
		creator.Code,
		creator.FullName,
		creator.NickName
	FROM PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON creator.EmployeeId = reaction.CreatorId
	WHERE reaction.PerformanceRecordId = @PerformanceRecordId
		AND reaction.CreatorId = @ActorId
		AND creator.Code = @ActorCode
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Report_GetPARecord] @ActorId INT
	,@PeriodIds VARCHAR(MAX) = ''
	,@GroupIds VARCHAR(MAX) = ''
	,@EmployeeIds VARCHAR(MAX) = ''
	,@IsRevised BIT = NULL
AS
/*
Created: 20181220@Oliver.Dang
Task: Get PA Report.
DB: bodb_NexGoal
Original:

Revisions:
    20190117@Tim.Trinh: refactor
Param's Explanation:

*/
BEGIN
	DECLARE @SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@Today DATE = GETDATE()

	CREATE TABLE #tblGroupId (GroupId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	CREATE TABLE #tblPeriodId (PeriodId INT)

	CREATE TABLE #tblEmployee (EmpId INT)

	INSERT INTO #tblPeriodId (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@PeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblGroupId (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO #tblEmployee (EmpId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')
	WHERE ISNUMERIC(val) = 1

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO #tblPARecordId (PARecordId)
		SELECT pa.PARecordId
		FROM dbo.PARecord AS pa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
			INNER JOIN #tblPeriodId AS p WITH (NOLOCK) ON period.PeriodId = p.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
			INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			INNER JOIN #tblEmployee AS e WITH (NOLOCK) ON e.EmpId = emp.EmployeeId
		WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
			AND (
				@IsRevised IS NULL
				OR ISNULL(pa.IsRevised, 0) = @IsRevised
				)
		GROUP BY pa.PARecordId
	END
	ELSE
	BEGIN
		INSERT INTO #tblPARecordId (PARecordId)
		SELECT pa.PARecordId
		FROM dbo.PARecord AS pa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
			INNER JOIN #tblPeriodId AS p WITH (NOLOCK) ON period.PeriodId = p.PeriodId
			INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
			AND process.EvaluatorId = @ActorId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
			INNER JOIN #tblGroupId AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
			INNER JOIN #tblEmployee AS e WITH (NOLOCK) ON e.EmpId = emp.EmployeeId
		WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
			AND (process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId, @SelfProcessId))
			AND (
				@IsRevised IS NULL
				OR ISNULL(pa.IsRevised, 0) = @IsRevised
				)
		GROUP BY pa.PARecordId
	END

	SELECT p.PeriodId
		,p.[Name]
		,(CASE 
			WHEN p.EndDate >= @ToDay THEN 1
			ELSE 0 END
			) AS IsInProgress
		,p.StartDate
		,p.EndDate
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN #tblPeriodId AS tpl WITH (NOLOCK) ON p.PeriodId = tpl.PeriodId
	ORDER BY p.EndDate DESC

	SELECT pa.PARecordId
		,period.PeriodId
		,pa.Toeic
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,jobtitle.[Name] AS JobTitle
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobTitle AS jobtitle WITH (NOLOCK) ON emp.JobTitleId = jobtitle.JobTitleId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS ee ON emp.EmployeeId = ee.EmployeeId
	ORDER BY ee.RankIndex

	SELECT process.PARecordId
		,process.ProcessId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE process.ProcessId >= @SelfProcessId

	DROP TABLE #tblGroupId

	DROP TABLE #tblPARecordId

	DROP TABLE #tblPeriodId
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PAReport_GetDataForExportExcel] @ActorId INT
	,@PeriodIds VARCHAR(MAX) = ''
	,@GroupIds VARCHAR(MAX) = ''
	,@EmployeeIds VARCHAR(MAX) = ''
	,@IsRevised BIT = NULL
AS
/*
Created: 20200220@Tim.Trinh
Task: Get PA Report for excel.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	DECLARE @SelfProcessId TINYINT = 2
		,@ReviewerProcessId TINYINT = 5
		,@EvaluatorProcessId TINYINT = 4
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@Today DATE = GETDATE()
	DECLARE @tblGroup TABLE (GroupId INT)
	DECLARE @tblEmployee TABLE (EmpId INT)
	DECLARE @tblPeriod TABLE (PeriodId INT)

	CREATE TABLE #tblPARecordId (PARecordId INT)

	CREATE TABLE #tblRelatedPerformanceRecord (
		PARecordId INT
		,ProcessId TINYINT
		,PerformanceRecordId INT
		)

	INSERT INTO @tblPeriod (PeriodId)
	SELECT val
	FROM dbo.func_SplitString(@PeriodIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO @tblGroup (GroupId)
	SELECT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO @tblEmployee (EmpId)
	SELECT val
	FROM dbo.func_SplitString(@EmployeeIds, ',')
	WHERE ISNUMERIC(val) = 1

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO #tblPARecordId (PARecordId)
		SELECT pa.PARecordId
		FROM dbo.PARecord AS pa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
			INNER JOIN @tblPeriod AS p ON period.PeriodId = p.PeriodId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
			INNER JOIN @tblGroup AS gr ON emp.GroupId = gr.GroupId
			INNER JOIN @tblEmployee AS e ON e.EmpId = emp.EmployeeId
		WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
			AND (
				@IsRevised IS NULL
				OR ISNULL(pa.IsRevised, 0) = @IsRevised
				)
		GROUP BY pa.PARecordId
	END
	ELSE
	BEGIN
		INSERT INTO #tblPARecordId (PARecordId)
		SELECT pa.PARecordId
		FROM dbo.PARecord AS pa WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
			INNER JOIN @tblPeriod AS p ON period.PeriodId = p.PeriodId
			INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
			AND process.EvaluatorId = @ActorId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
			INNER JOIN @tblGroup AS gr ON emp.GroupId = gr.GroupId
			INNER JOIN @tblEmployee AS e ON e.EmpId = emp.EmployeeId
		WHERE pa.StatusId IN (@PAInProgress, @PAClosed)
			AND (process.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId, @SelfProcessId))
			AND (
				@IsRevised IS NULL
				OR ISNULL(pa.IsRevised, 0) = @IsRevised
				)
		GROUP BY pa.PARecordId
	END

	SELECT p.PeriodId
		,p.[Name]
		,(CASE 
			WHEN p.EndDate >= @Today THEN 1
			ELSE 0 END
			) AS IsInProgress
		,p.StartDate
		,p.EndDate
	FROM dbo.Period AS p WITH (NOLOCK)
		INNER JOIN @tblPeriod AS tpl ON p.PeriodId = tpl.PeriodId
	ORDER BY p.EndDate DESC

	SELECT pa.PARecordId
		,pa.PeriodId
		,pa.Toeic
		,pa.ToeicPolicy
		,pa.OriginalGradeId
		,pa.ModifiedDate
		,pa.IsRevised
		,pa.StatusId
		,pa.GradeId
		,pa.Comment
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,jobtitle.[Name] AS JobTitle
	FROM dbo.PARecord AS pa WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON pa.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobTitle AS jobtitle WITH (NOLOCK) ON emp.JobTitleId = jobtitle.JobTitleId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS ee ON emp.EmployeeId = ee.EmployeeId
	ORDER BY ee.RankIndex

	SELECT process.PARecordId
		,process.ProcessId
		,process.GradeId
		,process.RelatedPerformanceRecordIds
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Code
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		LEFT JOIN dbo.PerformanceRecord AS record WITH (NOLOCK) ON process.PerformanceRecordId = record.PerformanceRecordId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId
	WHERE process.ProcessId >= @SelfProcessId

	INSERT INTO #tblRelatedPerformanceRecord (
		PARecordId
		,ProcessId
		,PerformanceRecordId
		)
	SELECT process.PARecordId
		,process.ProcessId
		,child.val
	FROM dbo.PAProcess AS process WITH (NOLOCK)
		INNER JOIN #tblPARecordId AS tbl WITH (NOLOCK) ON process.PARecordId = tbl.PARecordId
		CROSS APPLY dbo.func_SplitString(process.RelatedPerformanceRecordIds, ',') AS child
	WHERE process.ProcessId >= @SelfProcessId

	SELECT record.PerformanceRecordId
		,record.GradeId
		,record.Comment
		,record.CreatedDate
		,record.RoleId
		,record.TypeId
		,creator.EmployeeId
		,creator.Code
		,creator.NickName
		,creator.FullName
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON record.EmployeeId = emp.EmployeeId
		LEFT JOIN dbo.Employee AS creator WITH (NOLOCK) ON record.CreatorId = creator.EmployeeId

	SELECT record.PerformanceRecordId
		,pt.ProjectId
		,pt.[Name]
	FROM dbo.PerformanceRecord AS record WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON record.PerformanceRecordId = tbl.PerformanceRecordId
		OUTER APPLY dbo.func_SplitString(record.ProjectIds, ',') AS pr
		INNER JOIN dbo.ProjectTag AS pt WITH (NOLOCK) ON pr.val = pt.ProjectId

	SELECT reaction.PerformanceRecordId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRReaction AS reaction WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON reaction.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	SELECT fb.PRFeedbackId
		,fb.PerformanceRecordId
		,fb.RoleId
		,fb.Content
		,fb.CreatedDate
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedback AS fb WITH (NOLOCK)
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON fb.CreatorId = creator.EmployeeId

	SELECT reaction.PRFeedbackId
		,reaction.RoleId
		,creator.EmployeeId
		,creator.NickName
		,creator.FullName
		,creator.Code
	FROM dbo.PRFeedbackReaction AS reaction WITH (NOLOCK)
		INNER JOIN dbo.PRFeedback AS fb WITH (NOLOCK) ON reaction.PRFeedbackId = fb.PRFeedbackId
		INNER JOIN #tblRelatedPerformanceRecord AS tbl WITH (NOLOCK) ON fb.PerformanceRecordId = tbl.PerformanceRecordId
		INNER JOIN dbo.Employee AS creator WITH (NOLOCK) ON reaction.CreatorId = creator.EmployeeId
	ORDER BY reaction.CreatedDate ASC

	DROP TABLE #tblPARecordId

	DROP TABLE #tblRelatedPerformanceRecord
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRReaction_Delete] @PerformanceRecordId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: remove member's PR agreement
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRReaction WITH (NOLOCK)
		WHERE PerformanceRecordId = @PerformanceRecordId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The reaction is not existed.'

		RETURN
	END

	DELETE 
	FROM dbo.PRReaction
	WHERE PerformanceRecordId = @PerformanceRecordId
		AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PerformanceRecordId AS '@id'
				,@CreatorId AS '@cid'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRReaction_Delete'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRFeedbackReaction_Delete] @PRFeedbackId INT
	,@CreatorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS

/*
Created: 20200213@Drake.Doi
Task: remove PR feedback reaction
DB: bodb_NexGoal
Original: 
*/

BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATETIME = GETDATE()

	-- For write log
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
		SELECT 1
		FROM dbo.PRFeedbackReaction WITH (NOLOCK)
		WHERE PRFeedbackId = @PRFeedbackId
			AND CreatorId = @CreatorId
	)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'The reaction is not existed.'

		RETURN
	END

	DELETE 
	FROM dbo.PRFeedbackReaction
	WHERE PRFeedbackId = @PRFeedbackId
	AND CreatorId = @CreatorId

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	SET @xml = (
			SELECT @PRFeedbackId AS '@id'
				,@CreatorId AS '@cid'
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC dbo.LogDetail_Insert @ActorId = @CreatorId
		,@SPName = 'PRFeedbackReaction_Delete'
		,@Data = @logData
		,@Duration = @duration
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ProbationReport_Get]
	@FromDate DATE = NULL
	,@ToDate DATE = NULL
	,@EmployeeIds VARCHAR(MAX) = NULL
	,@IsQualifiedByEvaluator BIT = NULL
	,@IsQualified BIT = NULL
	,@ActorId INT
AS
/*
Created: 20190520@Elton.ton
Task: get probationer's evaluation
DB: bodb_NexGoal
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
	
	DECLARE	@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@SelfProcessId TINYINT = 2
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5
		,@HRProcessId TINYINT = 1

	CREATE TABLE #member(EmployeeId INT, ProcessId TINYINT)

	 SELECT e.val AS EmployeeId
	 INTO #selectedEmployee
	 FROM dbo.func_SplitString(@EmployeeIds,',') AS e

	IF EXISTS (
		SELECT 1
		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		WHERE er.EmployeeId = @ActorId 
			AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
	)
	BEGIN
		INSERT INTO #member(EmployeeId, ProcessId)
		SELECT EmployeeId
			,@HRProcessId
		FROM #selectedEmployee
	END
	ELSE
	BEGIN
		INSERT INTO #member (EmployeeId,ProcessId)
		SELECT member.EmployeeId, MIN(member.ProcessId) AS ProcessId
		FROM dbo.func_GetMember(@ActorId) AS member
			INNER JOIN #selectedEmployee se ON member.EmployeeId = se.EmployeeId
		WHERE member.ProcessId IN (@EvaluatorProcessId, @ReviewerProcessId)
			AND EXISTS (
					SELECT 1
					FROM dbo.ProbationEvaluation AS pe WITH (NOLOCK)
					WHERE member.EmployeeId = pe.EmployeeId
			)
		GROUP BY member.EmployeeId
		UNION 
		SELECT ps.EmployeeId, ps.ProcessId
		FROM dbo.ProbationSetting ps WITH (NOLOCK)
			INNER JOIN #selectedEmployee se ON ps.EmployeeId = se.EmployeeId
		WHERE ps.EmployeeId = @ActorId AND ps.EvaluatorId = @ActorId AND ps.ProcessId = @SelfProcessId
	END

	SELECT pe.EmployeeId
		,pe.IsQualified
		,e.NickName
		,e.FullName
		,e.Code
		,e.GroupId
		,e.JoinedDate
		,jt.Name AS JobTitle
		,g.Name AS GroupName
		,pe.SelfScore
		,pe.SelfComment
		,pe.EvaluatorScore
		,pe.EvaluatorComment
		,pe.ReviewerComment
		,(CASE 
				WHEN m.ProcessId = @EvaluatorProcessId OR m.ProcessId = @ReviewerProcessId  OR m.ProcessId = @HRProcessId THEN pe.IsQualifiedByEvaluator
				ELSE NULL
			END) AS IsQualifiedByEvaluator
		,CAST((CASE 
				WHEN m.ProcessId = @EvaluatorProcessId OR m.ProcessId = @ReviewerProcessId OR m.ProcessId = @HRProcessId THEN 1
				ELSE 0
		END) AS BIT) AS CanViewEvaluatorResult
	FROM dbo.ProbationEvaluation pe WITH (NOLOCK)
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON pe.EmployeeId = e.EmployeeId
		INNER JOIN dbo.JobTitle jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
		INNER JOIN #member m ON pe.EmployeeId = m.EmployeeId
	WHERE ((@FromDate IS NULL AND @ToDate IS NULL) OR e.JoinedDate BETWEEN @FromDate AND @ToDate)
		AND (@IsQualifiedByEvaluator IS NULL OR pe.IsQualifiedByEvaluator = @IsQualifiedByEvaluator)
		AND (@IsQualified IS NULL OR pe.IsQualified = @IsQualified)
		AND EXISTS (
			SELECT 1
			FROM dbo.ProbationSetting ps WITH (NOLOCK)
			WHERE ps.EmployeeId = pe.EmployeeId AND ps.ProcessId = @SelfProcessId AND ps.FromDate <= @Today
		)
	ORDER BY e.JoinedDate DESC

	DROP TABLE #selectedEmployee
	DROP TABLE #member
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Auto_Period_Insert] @ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181031@Tim.Trinh
Task: auto create period.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@Month TINYINT = DATEPART(MONTH, GETDATE())
		,@Year SMALLINT = DATEPART(YEAR, GETDATE())
		,@Name NVARCHAR(200)
		,@SemiTargetTypeId TINYINT = 2
	DECLARE @StartDate DATE
		,@EndDate DATE
		,@DisciplineStartDate DATE
		,@DisciplineEndDate DATE
		,@FocusStartDate DATE
		,@FocusEndDate DATE

	IF @Month BETWEEN 6
			AND 11
	BEGIN
		SET @Name = '2H ' + CAST(@Year AS VARCHAR(5)) + ' - Auto'
		SET @StartDate = CAST(@Year AS VARCHAR(5)) + '-11-01'
		SET @DisciplineStartDate = CAST(@Year AS VARCHAR(5)) + '-06-01'
	END
	ELSE
	BEGIN
		IF @Month = 12
		BEGIN
			SET @Name = '1H ' + CAST((@Year + 1) AS VARCHAR(5)) + ' - Auto'
			SET @StartDate = CAST((@Year + 1) AS VARCHAR(5)) + '-05-01'
			SET @DisciplineStartDate = CAST(@Year AS VARCHAR(5)) + '-12-01'
		END
		ELSE
		BEGIN
			SET @Name = '1H ' + CAST(@Year AS VARCHAR(5)) + ' - Auto'
			SET @StartDate = CAST(@Year AS VARCHAR(5)) + '-05-01'
			SET @DisciplineStartDate = CAST((@Year - 1) AS VARCHAR(5)) + '-12-01'
		END
	END

	SET @EndDate = DATEADD(DAY, - 1, DATEADD(MONTH, 1, @StartDate))
	SET @DisciplineEndDate = @EndDate
	SET @FocusStartDate = @StartDate
	SET @FocusEndDate = DATEADD(DAY, 7, @FocusStartDate)

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.Period WITH (NOLOCK)
			WHERE @Today BETWEEN DisciplineStartDate
					AND DisciplineEndDate
				AND TargetTypeId = @SemiTargetTypeId
			)
	BEGIN
		INSERT INTO dbo.Period (
			[Name]
			,TargetTypeId
			,StartDate
			,EndDate
			,DisciplineStartDate
			,DisciplineEndDate
			,FocusStartDate
			,FocusEndDate
			,IsActive
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@Name
			,@SemiTargetTypeId
			,@StartDate
			,@EndDate
			,@DisciplineStartDate
			,@DisciplineEndDate
			,@FocusStartDate
			,@FocusEndDate
			,1
			,- 1
			,GETDATE()
			)

		DECLARE @PeriodId INT = SCOPE_IDENTITY()

		INSERT INTO dbo.PeriodProcessSetting (
			PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
			,FocusDay
			)
		SELECT @PeriodId
			,ProcessId
			,Duration
			,NoticeAtBegin
			,RemindDay
			,FocusDay
		FROM dbo.PeriodDefaultProcessSetting WITH (NOLOCK)
		WHERE TargetTypeId = @SemiTargetTypeId
			AND ProcessId = 5 -- reviewer
	END
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PAFocus_Generate] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20181008@Tim.Trinh
Task: PA Focus
DB: bobd_NexGoal
Original:

Revisions: 
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @employeeId INT
		,@lastObjectiveId INT
		,@nextObjectiveId INT
		,@rowIndex INT = 1
		,@totalRow INT
		-- PA Status
		,@PAOpened TINYINT = 2
		,@PAInProgress TINYINT = 3
		,@PAClosed TINYINT = 4

	CREATE TABLE #tblEmployeeIds (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,EmployeeId INT
		)

	INSERT INTO #tblEmployeeIds (EmployeeId)
	SELECT emp.EmployeeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS record WITH (NOLOCK) ON emp.EmployeeId = record.EmployeeId
		AND record.StatusId IN (@PAOpened, @PAInProgress, @PAClosed)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON record.PeriodId = period.PeriodId
		AND period.PeriodId = @PeriodId
		AND period.IsActive = 1
		LEFT JOIN dbo.PAFocus AS focus WITH (NOLOCK) ON emp.EmployeeId = focus.EmployeeId
		AND focus.PeriodId = @PeriodId
	WHERE emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND focus.EmployeeId IS NULL

	SELECT @totalRow = COUNT(1)
	FROM #tblEmployeeIds

	WHILE @rowIndex <= @totalRow
	BEGIN
		SELECT @employeeId = EmployeeId
		FROM #tblEmployeeIds
		WHERE RowNum = @rowIndex

		SELECT TOP 1 @lastObjectiveId = ObjectiveId
		FROM dbo.EmployeeObjective WITH (NOLOCK)
		WHERE EmployeeId = @employeeId
		ORDER BY CreatedDate DESC

		INSERT INTO dbo.EmployeeObjective
		VALUES (
			@employeeId
			,NULL
			,NULL
			,GETDATE()
			)

		SET @nextObjectiveId = SCOPE_IDENTITY()

		INSERT INTO dbo.PAFocus (
			PeriodId
			,EmployeeId
			,LastObjectiveId
			,NextObjectiveId
			,IsReadyPromotion
			,Contribution
			,BonusGiven
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @PeriodId
			,@employeeId
			,@lastObjectiveId
			,@nextObjectiveId
			,NULL
			,NULL
			,NULL
			,NULL
			,NULL

		SET @lastObjectiveId = NULL
		SET @rowIndex = @rowIndex + 1
	END

	DROP TABLE #tblEmployeeIds
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Auto_PARecord_GetSettingToSendNotification] @ReceiveType TINYINT
AS
/*
Created: 20190107@Tim.Trinh
Task: get setting to send notification.
DB: bodb_NexGoal
Original:

Revisions:
	20190411@elton.ton: change date to send notifications before period start date
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@NotificationType TINYINT = 2
		,@InProgress TINYINT = 3
		,@Closed TINYINT = 4
		,@StartMode TINYINT = 1
		,@RemindMode TINYINT = 2
		,@RemindStartEvaluationMode TINYINT = 3
		,@EndEvaluationMode TINYINT = 4
		,@SelfProcessId TINYINT = 2
		,@PMProcessId TINYINT = 3
		,@ReviewerProcessId TINYINT = 5
		-- receive type
		,@ReceiveMail TINYINT = 1
		,@ReceiveNotification TINYINT = 2
		,@ReceiveSkypeMessage TINYINT = 3

	CREATE TABLE #tblProcessSetting (
		PeriodId INT
		,PerformanceRecordId INT
		,NotificationMode TINYINT
		,ProcessId TINYINT
		,EvaluatorId INT
		,Nickname NVARCHAR(20)
		,FullName NVARCHAR(50)
		,Email VARCHAR(50)
		,ConversationId VARCHAR(255)
		,StartDate DATE
		,EndDate DATE
		)

	INSERT INTO #tblProcessSetting (
		PeriodId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT data.PeriodId
		,@RemindStartEvaluationMode AS NotificationMode
		,MAX(data.ProcessId) AS ProcessId
		,data.EvaluatorId
		,data.NickName
		,data.FullName
		,data.Email
		,data.ConversationId
	FROM (
		SELECT period.PeriodId
			,efs.ProcessId
			,efs.EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.ConversationId
		FROM dbo.Employee AS emp WITH (NOLOCK)
			INNER JOIN dbo.EmployeeFlowSetting AS efs WITH (NOLOCK) ON emp.EmployeeId = efs.EvaluatorId
			AND efs.ProcessId > @SelfProcessId
			,dbo.Period AS period WITH (NOLOCK)
		WHERE emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
		
		UNION
		SELECT period.PeriodId
			,gfs.ProcessId
			,gfs.EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.ConversationId
		FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON gfs.EvaluatorId = emp.EmployeeId
			AND emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON gfs.GroupId = gr.GroupId
			AND gr.IsDeleted = 0
			,dbo.Period AS period WITH (NOLOCK)
		WHERE gfs.ProcessId > @SelfProcessId
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
		
		UNION
		SELECT period.PeriodId
			,@PMProcessId AS ProcessId
			,emp.EmployeeId AS EvaluatorId
			,emp.NickName
			,emp.FullName
			,emp.Email
			,emp.ConversationId
		FROM dbo.Employee AS emp WITH (NOLOCK)
			,dbo.Period AS period WITH (NOLOCK)
		WHERE emp.[Enabled] = 1
			AND emp.IsDeleted = 0
			AND DATEADD(DAY, - 7, period.StartDate) = @Today
			AND EXISTS (
				SELECT 1
				FROM dbo.PMSetting AS pmsetting WITH (NOLOCK)
				WHERE pmsetting.ProjectManagerId = emp.EmployeeId
				)
		) AS data
	GROUP BY data.PeriodId
		,data.EvaluatorId
		,data.NickName
		,data.FullName
		,data.Email
		,data.ConversationId

	INSERT INTO #tblProcessSetting (
		PeriodId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		)
	SELECT period.PeriodId
		,@RemindStartEvaluationMode AS NotificationMode
		,@SelfProcessId AS ProcessId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.ConversationId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		,dbo.Period AS period WITH (NOLOCK)
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
		AND DATEADD(DAY, -28, period.StartDate) = @Today
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeFlowSetting AS efs WITH (NOLOCK)
				WHERE efs.EmployeeId = emp.EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupFlowSetting AS gfs WITH (NOLOCK)
				WHERE gfs.GroupId = gr.GroupId
				)
			)


	INSERT INTO #tblProcessSetting (
		PeriodId
		,PerformanceRecordId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,ConversationId
		,StartDate
		,EndDate
		)
	SELECT DISTINCT period.PeriodId
		,NULL AS PerformanceRecordId
		,(CASE 
			WHEN process.StartDate = @Today THEN @StartMode
			ELSE @RemindMode END
			) AS NotificationMode
		,process.ProcessId
		,process.EvaluatorId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.ConversationId
		,process.StartDate
		,process.EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.StatusId = @InProgress
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON process.EvaluatorId = emp.EmployeeId
		INNER JOIN dbo.PeriodProcessSetting AS ps WITH (NOLOCK) ON period.PeriodId = ps.PeriodId
		AND ps.ProcessId = @ReviewerProcessId
	WHERE period.IsActive = 1
		AND (
			process.StartDate = @Today
			OR DATEADD(DAY, - ps.RemindDay, process.EndDate) = @Today
			)
	
	UNION
	SELECT DISTINCT period.PeriodId
		,pa.PerformanceRecordId
		,@EndEvaluationMode AS NotificationMode
		,process.ProcessId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Email
		,emp.ConversationId
		,NULL AS StartDate
		,NULL AS EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
		INNER JOIN dbo.PARecord AS pa WITH (NOLOCK) ON pa.PeriodId = period.PeriodId
		AND pa.StatusId IN (@InProgress, @Closed)
		INNER JOIN dbo.PAProcess AS process WITH (NOLOCK) ON pa.PARecordId = process.PARecordId
		AND process.ProcessId = @ReviewerProcessId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON pa.EmployeeId = emp.EmployeeId
	WHERE period.IsActive = 1
		AND period.EndDate < @Today
		AND process.EndDate < @Today
		AND (
			DATEADD(DAY, 1, period.EndDate) = @Today
			OR DATEADD(DAY, 1, process.EndDate) = @Today
			)

	SELECT period.PeriodId
		,period.[Name]
		,period.StartDate
		,period.EndDate
	FROM dbo.Period AS period WITH (NOLOCK)
	WHERE period.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM #tblProcessSetting AS tbl WITH (NOLOCK)
				LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
			WHERE period.PeriodId = tbl.PeriodId
				AND ISNULL(setting.TypeId, @NotificationType) = @NotificationType
				AND (
					(
						@ReceiveType = @ReceiveMail
						AND ISNULL(setting.ReceiveEmail, 1) = 1
						)
					OR (
						@ReceiveType = @ReceiveNotification
						AND ISNULL(setting.ReceiveNotification, 1) = 1
						)
					OR (
						@ReceiveType = @ReceiveSkypeMessage
						AND ISNULL(setting.ReceiveSkypeMessage, 1) = 1
						)
					)
			)

	SELECT PeriodId
		,PerformanceRecordId
		,NotificationMode
		,ProcessId
		,EvaluatorId
		,Nickname
		,FullName
		,Email
		,StartDate
		,EndDate
		,ConversationId
	FROM #tblProcessSetting AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.NotificationSetting AS setting WITH (NOLOCK) ON tbl.EvaluatorId = setting.EmployeeId
	WHERE ISNULL(setting.TypeId, @NotificationType) = @NotificationType
		AND (
			(
				@ReceiveType = @ReceiveMail
				AND ISNULL(setting.ReceiveEmail, 1) = 1
				)
			OR (
				@ReceiveType = @ReceiveNotification
				AND ISNULL(setting.ReceiveNotification, 1) = 1
				)
			OR (
				@ReceiveType = @ReceiveSkypeMessage
				AND ISNULL(setting.ReceiveSkypeMessage, 1) = 1
				)
			)

	DROP TABLE #tblProcessSetting
END
*/
GO
