
CREATE PROCEDURE [dbo].[Employee_Search] @Keyword NVARCHAR(250) = NULL
	,@EmployeeId INT = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@IsEnabled BIT = NULL
AS
/*
Created: 20180716@Elton.ton
Task: search employee
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Today DATE = GETDATE()
		,@ProbationDuration SMALLINT

	SELECT TOP 1 @ProbationDuration = CAST(Value AS SMALLINT)
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'ProbationDuration'

	SELECT emp.EmployeeId
		,emp.Code
		,emp.FullName
		,emp.NickName
		,emp.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		,(CASE 
			WHEN @Today BETWEEN emp.JoinedDate
					AND DATEADD(DAY, @ProbationDuration, emp.JoinedDate) THEN 1
			ELSE 0 END
			) AS IsProbation
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
		LEFT JOIN dbo.func_SplitString(@GroupIds, ',') AS grid ON ISNUMERIC(grid.val) = 1
		AND grid.val = gr.GroupId
	WHERE (
			emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		AND (
			@GroupIds IS NULL
			OR grid.val IS NOT NULL
			)
		AND ISNULL(@IsEnabled, emp.[Enabled]) = emp.[Enabled]
		--AND emp.IsDeleted = 0
	ORDER BY emp.NickName
END
