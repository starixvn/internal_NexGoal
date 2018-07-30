
CREATE PROCEDURE [dbo].[User_Get] @Keyword VARCHAR(250) = NULL
	,@GroupId INT = NULL
	,@IsLocal BIT = NULL
	,@IsDisabledUser BIT = NULL
AS
/*
Created: 20180704@Elton.ton
Task: Get users
DB: bodb_NexGoal

*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.Code
		,e.FullName
		,e.NickName
		,jt.[Name] AS JobTitle
		,g.Code AS 'Group'
		,STUFF((
				SELECT ', ' + rr.Value
				FROM dbo.EmployeeRole AS er WITH (NOLOCK)
					INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON er.RoleId = rr.ResourceId
					AND rr.RefName = 'SystemRole'
					AND er.EmployeeId = e.EmployeeId
				FOR XML PATH('')
				), 1, 2, '') AS Roles
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.JobTitle AS jt WITH (NOLOCK) ON e.JobTitleId = jt.JobTitleId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
	WHERE ISNULL(@GroupId, e.GroupId) = e.GroupId
		AND (
			e.[Enabled] <> @IsDisabledUser
			OR @IsDisabledUser IS NULL
			)
		AND ISNULL(@IsLocal, e.IsLocal) = e.IsLocal
		AND (
			e.FullName LIKE '%' + ISNULL(@Keyword, '') + '%'
			OR e.NickName LIKE '%' + ISNULL(@Keyword, '') + '%'
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