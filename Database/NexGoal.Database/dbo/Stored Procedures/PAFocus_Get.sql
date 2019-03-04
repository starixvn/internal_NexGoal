
CREATE PROCEDURE [dbo].[PAFocus_Get] @ActorId INT
	,@PeriodId INT
	,@GroupIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
AS
/*
Created: 20181008@Tim.Trinh
Task: PA Focus
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@Policy TINYINT = 0
		,@FunctionKey VARCHAR(50) = 'PAFocus'
		-- system role
		,@SARoleId TINYINT = 1
		,@HRRoleId TINYINT = 2
		,@ManagementRoleId TINYINT = 3
		,@EvaluatorRoleId TINYINT = 4
		,@ReviewerRoleId TINYINT = 5

	CREATE TABLE #tblGroupIds (GroupId INT)

	CREATE TABLE #tblPAFocusIds (
		PAFocusId INT
		,IsEvaluator BIT
		)

	INSERT INTO #tblGroupIds (GroupId)
	SELECT DISTINCT val
	FROM dbo.func_SplitString(@GroupIds, ',')

	SELECT @Policy = r.Policy
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.RoleFunction AS r WITH (NOLOCK) ON r.RoleId = er.RoleId
		AND r.Policy > 0
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON f.FunctionId = r.FunctionId
		AND f.FunctionKey = @FunctionKey
	WHERE emp.EmployeeId = @ActorId

	INSERT INTO #tblPAFocusIds (
		PAFocusId
		,IsEvaluator
		)
	SELECT focus.PAFocusId
		,1
	FROM dbo.PAFocus AS focus WITH (NOLOCK)
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON period.PeriodId = focus.PeriodId
		AND period.IsActive = 1
		AND period.FocusStartDate <= @Today
		AND period.PeriodId = @PeriodId
		INNER JOIN dbo.func_GetMember(@ActorId) AS member ON focus.EmployeeId = member.EmployeeId
		AND member.RoleId = @EvaluatorRoleId
	WHERE (
			@EmployeeId IS NULL
			OR focus.EmployeeId = @EmployeeId
			)

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId IN (@HRRoleId, @ManagementRoleId)
				INNER JOIN dbo.RoleFunction AS r WITH (NOLOCK) ON r.RoleId = er.RoleId
				AND r.Policy > 0
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON f.FunctionId = r.FunctionId
				AND f.FunctionKey = @FunctionKey
			WHERE emp.EmployeeId = @ActorId
			
			UNION
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON er.EmployeeId = emp.EmployeeId
				AND er.RoleId = @SARoleId
			WHERE emp.EmployeeId = @ActorId
			)
	BEGIN
		INSERT INTO #tblPAFocusIds (
			PAFocusId
			,IsEvaluator
			)
		SELECT DISTINCT focus.PAFocusId
			,0
		FROM dbo.PAFocus AS focus WITH (NOLOCK)
			INNER JOIN dbo.Period AS period WITH (NOLOCK) ON period.PeriodId = focus.PeriodId
			AND period.IsActive = 1
			AND period.FocusStartDate <= @Today
			AND period.PeriodId = @PeriodId
		WHERE focus.PAFocusId NOT IN (
				SELECT PAFocusId
				FROM #tblPAFocusIds WITH (NOLOCK)
				)
			AND (
				@EmployeeId IS NULL
				OR focus.EmployeeId = @EmployeeId
				)
			AND EXISTS (
				SELECT 1
				FROM dbo.Employee AS emp WITH (NOLOCK)
				WHERE focus.EmployeeId = emp.EmployeeId
					AND emp.[Enabled] = 1
					AND emp.IsDeleted = 0
				)
	END

	SELECT focus.PAFocusId
		,l.[Description] AS CurrentObjective
		,n.[Description] AS NextObjective
		,focus.Contribution
		,focus.BonusGiven
		,focus.IsReadyPromotion
		,(CASE 
			WHEN @Policy = 2
				AND focusid.IsEvaluator = 1
				AND @Today BETWEEN period.FocusStartDate
					AND period.FocusEndDate THEN 1
			ELSE 0 END
			) AS IsHost
		,parecord.OriginalGradeId
		,parecord.GradeId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,jobtitle.[Name] AS JobTitle
	FROM #tblPAFocusIds AS focusid WITH (NOLOCK)
		INNER JOIN dbo.PAFocus AS focus WITH (NOLOCK) ON focus.PAFocusId = focusid.PAFocusId
		INNER JOIN dbo.Period AS period WITH (NOLOCK) ON focus.PeriodId = period.PeriodId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmployeeId = focus.EmployeeId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobTitle AS jobtitle WITH (NOLOCK) ON emp.JobTitleId = jobtitle.JobTitleId
		INNER JOIN dbo.PARecord AS parecord WITH (NOLOCK) ON parecord.EmployeeId = emp.EmployeeId
		AND parecord.PeriodId = focus.PeriodId
		INNER JOIN #tblGroupIds AS grid WITH (NOLOCK) ON emp.GroupId = grid.GroupId
		LEFT JOIN dbo.EmployeeObjective AS l WITH (NOLOCK) ON l.ObjectiveId = focus.LastObjectiveId
		LEFT JOIN dbo.EmployeeObjective AS n WITH (NOLOCK) ON n.ObjectiveId = focus.NextObjectiveId
		INNER JOIN dbo.func_GetSortedEmployee(@ActorId) AS ee ON emp.EmployeeId = ee.EmployeeId
	ORDER BY ee.RankIndex

	DROP TABLE #tblGroupIds

	DROP TABLE #tblPAFocusIds
END
