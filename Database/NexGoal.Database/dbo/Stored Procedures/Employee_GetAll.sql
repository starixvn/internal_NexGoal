
CREATE PROCEDURE [dbo].[Employee_GetAll] @Keyword VARCHAR(250) = NULL
	,@GroupId INT = NULL
	,@RoleIds VARCHAR(MAX) = NULL
	,@IsDisabled BIT = NULL
AS
/*
Created: 20180704@Elton.ton
Task: Get users
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmployeeRole TABLE (
		RoleId SMALLINT
		,[Name] VARCHAR(500)
		)
	DECLARE @tblRoleIds TABLE (RoleId SMALLINT)

	INSERT INTO @tblRoleIds (RoleId)
	SELECT val
	FROM dbo.func_SplitString(@RoleIds, ',')
	WHERE ISNUMERIC(val) = 1

	INSERT INTO @EmployeeRole (
		RoleId
		,[Name]
		)
	SELECT ResourceId
		,Value
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = 'SystemRole'
		AND Value = 'Employee'

	SELECT e.EmployeeId
		,e.Code
		,e.FullName
		,e.NickName
		,jt.[Name] AS JobTitle
		,g.Code AS GroupCode
		,g.[Name] AS GroupName
		,STUFF((
				SELECT ', ' + data.[Name]
				FROM (
					SELECT rr.ResourceId AS RoleId
						,rr.Value AS [Name]
					FROM dbo.EmployeeRole AS er WITH (NOLOCK)
						INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON er.RoleId = rr.ResourceId
						AND rr.RefName = 'SystemRole'
						AND er.EmployeeId = e.EmployeeId
					
					UNION
					SELECT RoleId
						,[Name]
					FROM @EmployeeRole
					) AS data
				ORDER BY data.RoleId
				FOR XML PATH('')
				), 1, 2, '') AS Roles
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE ISNULL(@GroupId, e.GroupId) = e.GroupId
		AND (
			e.[Enabled] <> @IsDisabled
			OR @IsDisabled IS NULL
			)
		AND (
			e.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR e.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
			)
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole AS emprole WITH (NOLOCK)
					INNER JOIN @tblRoleIds AS tbl ON emprole.RoleId = tbl.RoleId
				WHERE emprole.EmployeeId = e.EmployeeId
				)
			OR EXISTS (
				SELECT 1
				FROM @EmployeeRole AS tblemprole
					INNER JOIN @tblRoleIds AS tblrole ON tblemprole.RoleId = tblrole.RoleId
				)
			)
	ORDER BY g.[Rank]
		,g.Code
		,(CASE e.EmployeeId
			WHEN g.LeaderId THEN 2
			WHEN g.AssistantLeaderId THEN 1
			ELSE 0 END
			) DESC
		,e.JoinedDate
END
