
CREATE PROCEDURE [dbo].[Employee_Search] @Keyword NVARCHAR(250) = NULL
	,@EmployeeId INT = NULL
	,@GroupId INT = NULL
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

	SELECT emp.EmployeeId
		,emp.Code
		,emp.FullName
		,emp.NickName
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		--,STUFF((
		--		SELECT ', ' + rr.Value
		--		FROM dbo.EmployeeRole AS er WITH (NOLOCK)
		--			INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON er.RoleId = rr.ResourceId
		--			AND rr.RefName = 'SystemRole'
		--			AND er.EmployeeId = emp.EmployeeId
		--		FOR XML PATH('')
		--		), 1, 2, '') AS Roles
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		AND gr.IsDeleted = 0
		INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
	WHERE (
			emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
		AND ISNULL(@EmployeeId, emp.EmployeeId) = emp.EmployeeId
		AND ISNULL(@GroupId, gr.GroupId) = gr.GroupId
		AND ISNULL(@IsEnabled, emp.[Enabled]) = emp.[Enabled]
		AND emp.IsDeleted = 0
	ORDER BY emp.NickName
END