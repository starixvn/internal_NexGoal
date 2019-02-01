CREATE PROCEDURE [dbo].[Employee_GetInfo] @EmployeeId INT = NULL
	,@UnreadNotification INT OUT
	,@IsHasMember BIT
AS
/*
Created: 20190201@Oliver.Dang
Task: Get employee info
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
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT TOP 1 @ProbationDuration = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationDuration'

	SELECT emp.EmployeeId
		,emp.Code
		,emp.FullName
		,emp.NickName
		,emp.GroupId
		,emp.JoinedDate
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		,(
			CASE 
				WHEN @Today BETWEEN emp.JoinedDate
						AND DATEADD(DAY, @ProbationDuration, emp.JoinedDate)
					THEN 1
				ELSE 0
				END
			) AS IsProbation
	FROM dbo.Employee AS emp WITH (NOLOCK)
	INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
	INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
	WHERE emp.EmployeeId = @EmployeeId

	SELECT @UnreadNotification = COUNT(1)
		,@IsHasMember = CASE 
			WHEN EXISTS (
					SELECT COUNT(1)
					FROM func_GetMember(@EmployeeId)
					)
			THEN 1
			ELSE 0
			END
	FROM dbo.[Notification] AS noti WITH (NOLOCK)
	WHERE noti.EmployeeId = @EmployeeId
		AND noti.StatusId = 0
END