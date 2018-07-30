
CREATE PROCEDURE [dbo].[EOQ_GetNominatorSetting] @EOQId INT = 0
	,@Keyword NVARCHAR(200) = NULL
	,@GroupIds NVARCHAR(MAX) = NULL
	,@RoleId TINYINT = NULL
	,@IsShowAll BIT = 0
AS
/*
Created: 20180719@Tim.Trinh
Task: Get nominator setting.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblGroupIds (GroupId INT)

	IF @GroupIds IS NOT NULL
	BEGIN
		INSERT INTO #tblGroupIds (GroupId)
		SELECT DISTINCT val
		FROM dbo.func_SplitString(@GroupIds, ',')
		WHERE ISNUMERIC(val) = 1
	END

	SELECT DISTINCT emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		,setting.RoleIds
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
		LEFT JOIN #tblGroupIds AS tbl WITH (NOLOCK) ON gr.GroupId = tbl.GroupId
		LEFT JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON emp.EmployeeId = setting.NominatorId
		AND setting.EOQYId = @EOQId
		LEFT JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON setting.EOQYId = eoq.EOQYId
		AND eoq.IsQuarter = 1
	WHERE (
			emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND (
			@GroupIds IS NULL
			OR tbl.GroupId IS NOT NULL
			)
		AND (
			@RoleId IS NULL
			OR EXISTS (
				SELECT 1
				FROM dbo.EOQYNominatorSetting AS st WITH (NOLOCK)
					CROSS APPLY dbo.func_SplitString(st.RoleIds, ',') AS f
				WHERE st.EOQYId = setting.EOQYId
					AND f.val = @RoleId
				)
			)
		AND (
			(
				@IsShowAll = 0
				AND setting.EOQYId IS NOT NULL
				)
			OR @IsShowAll = 1
			)

	DROP TABLE #tblGroupIds
END