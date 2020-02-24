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
