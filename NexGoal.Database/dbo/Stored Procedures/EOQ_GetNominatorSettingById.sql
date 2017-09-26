



CREATE PROCEDURE [dbo].[EOQ_GetNominatorSettingById] @EOQYId INT
	,@Query NVARCHAR(200) = NULL
	,@GroupIds NVARCHAR(MAX) = NULL
	,@RoleId TINYINT = NULL
	,@ShowAllEmployee BIT = 0
AS
/*
Created: 20160906@Tim.Trinh
Task: Get nominator setting.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Groups IntId

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO @Groups (Id)
		SELECT DISTINCT val
		FROM dbo.fnc_strSplit(@GroupIds, ',')
	END

	SELECT setting.RoleId
		,emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,dept.DepartmentId
		,dept.Code
		,dept.[Name]
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,job.Id AS JobTitleId
		,job.[Name] AS JobTitleName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		LEFT JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.HRMJobTitle AS job WITH (NOLOCK) ON emp.JobTitle = job.Id
		LEFT JOIN dbo.Department AS dept WITH (NOLOCK) ON gr.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.EOQYNominatorSetting AS setting WITH (NOLOCK) ON emp.EmployeeId = setting.NominatorId
		AND EOQYId = @EOQYId
	WHERE (
			ISNULL(@Query, '') = ''
			OR emp.FullName LIKE '%' + @Query + '%'
			OR emp.NickName LIKE '%' + @Query + '%'
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM @Groups
				WHERE Id < 0
					AND gr.GroupId = ABS(Id)
				)
			OR EXISTS (
				SELECT 1
				FROM @Groups
				WHERE Id > 0
					AND dept.DepartmentId = Id
				)
			)
		AND (
			ISNULL(@RoleId, '') = ''
			OR setting.RoleId = @RoleId
			)
		AND (
			(
				@ShowAllEmployee = 0
				AND EXISTS (
					SELECT 1
					FROM dbo.EOQYNominatorSetting
					WHERE emp.EmployeeId = NominatorId
						AND setting.EOQYId = EOQYId
					)
				)
			OR (
				@ShowAllEmployee = 1
				AND emp.IsDeleted = 0
				AND emp.[Enabled] = 1
				)
			)
END