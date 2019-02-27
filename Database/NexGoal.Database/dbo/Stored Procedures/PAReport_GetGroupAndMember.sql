
CREATE PROCEDURE [dbo].[PAReport_GetGroupAndMember] @ActorId INT
AS
/*
Created: 20192603@Oliver.Dang
Task: Get member for PA Report.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Today DATE = GETDATE()
		,@ProbationDuration SMALLINT
	-- process
	DECLARE @EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT TOP 1 @ProbationDuration = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationDuration'

	CREATE TABLE #tplEmployee (EmployeeId INT)

	INSERT INTO #tplEmployee (EmployeeId)
	SELECT DISTINCT member.EmployeeId
	FROM dbo.func_GetMember(@ActorId) AS member
	WHERE member.ProcessId IN (
			@EvaluatorProcessId
			,@ReviewerProcessId
			)

	INSERT INTO #tplEmployee (EmployeeId)
	SELECT @ActorId

	-- Get groups
	SELECT DISTINCT gr.GroupId
		,gr.[Name]
		,gr.Code
		,gr.[Rank]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN #tplEmployee AS tpl WITH (NOLOCK) ON tpl.EmployeeId = emp.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	ORDER BY gr.[Rank]
			,gr.[Name]

	-- Get members
	SELECT emp.EmployeeId
		,emp.FullName
		,emp.NickName
		,emp.GroupId
		,(
			CASE 
				WHEN @Today BETWEEN emp.JoinedDate
						AND DATEADD(DAY, @ProbationDuration, emp.JoinedDate)
					THEN 1
				ELSE 0
				END
			) AS IsProbation
	FROM dbo.Employee AS emp WITH (NOLOCK)
	INNER JOIN #tplEmployee AS tpl ON tpl.EmployeeId = emp.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.IsDeleted = 0
	ORDER BY (
			CASE 
				WHEN emp.EmployeeId = @ActorId
					THEN 0
				ELSE 1
				END
			),
			emp.EmployeeId

	

	DROP TABLE #tplEmployee
END