
CREATE PROCEDURE [dbo].[NominationPeriod_GetSetting] @NominationPeriodId INT
	,@Keyword NVARCHAR(200) = NULL
	,@GroupIds NVARCHAR(MAX) = ''
	,@RoleId TINYINT = NULL
	,@IsShowAll BIT = 0
AS
/*
Created: 20181122@Tim.Trinh
Task: Get nomination setting.
DB: bodb_NexGoal
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RoleIdString NVARCHAR(3)

	CREATE TABLE #tblGroupIds (GroupId INT)

	SET @RoleIdString = CASE 
			WHEN @RoleId IS NULL THEN ''
			ELSE CAST(@RoleId AS VARCHAR(3))
			END

	INSERT INTO #tblGroupIds (GroupId)
	SELECT DISTINCT val
	FROM dbo.func_SplitString(@GroupIds, ',')
	WHERE ISNUMERIC(val) = 1

	SELECT DISTINCT emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS GroupName
		,job.[Name] AS JobTitle
		,setting.RoleIds
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.JobTitle AS job WITH (NOLOCK) ON emp.JobTitleId = job.JobTitleId
		INNER JOIN #tblGroupIds AS tbl WITH (NOLOCK) ON gr.GroupId = tbl.GroupId
		LEFT JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON emp.EmployeeId = setting.NominatorId
		AND setting.EOQYId = @NominationPeriodId
		LEFT JOIN dbo.EOQY AS eoy WITH (NOLOCK) ON setting.EOQYId = eoy.EOQYId
	WHERE (
			emp.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR emp.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
		AND emp.IsDeleted = 0
		AND emp.[Enabled] = 1
		AND (
			@RoleIdString = ''
			OR setting.RoleIds LIKE @RoleIdString
			OR setting.RoleIds LIKE @RoleIdString + ',%'
			OR setting.RoleIds LIKE '%,' + @RoleIdString + ',%'
			OR setting.RoleIds LIKE '%,' + @RoleIdString
			)
		AND (
			(
				@IsShowAll = 0
				AND setting.EOQYId IS NOT NULL
				)
			OR @IsShowAll = 1
			)
	ORDER BY emp.NickName

	DROP TABLE #tblGroupIds
END