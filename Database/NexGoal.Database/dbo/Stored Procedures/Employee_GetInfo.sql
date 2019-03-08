
CREATE PROCEDURE [dbo].[Employee_GetInfo] @EmployeeId INT
	,@UnreadNotification INT OUT
	,@IsHasMember BIT OUT
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

	DECLARE @TotalMembers INT
	-- process
	DECLARE @PMProcessId TINYINT = 3
		,@EvaluatorProcessId TINYINT = 4
		,@ReviewerProcessId TINYINT = 5

	SELECT @TotalMembers = COUNT(1)
	FROM dbo.func_GetMember(@EmployeeId)

	SELECT emp.EmployeeId
		,emp.Code
		,emp.FullName
		,emp.NickName
		,emp.GroupId
		,emp.JoinedDate
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		,dbo.fnc_CheckIsProbation(emp.EmployeeId) AS IsProbation
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
	WHERE emp.EmployeeId = @EmployeeId

	SELECT @UnreadNotification = COUNT(1)
		,@IsHasMember = CASE 
			WHEN @TotalMembers > 0 THEN 1
			ELSE 0
			END
	FROM dbo.[Notification] AS noti WITH (NOLOCK)
	WHERE noti.EmployeeId = @EmployeeId
		AND noti.StatusId = 0
END