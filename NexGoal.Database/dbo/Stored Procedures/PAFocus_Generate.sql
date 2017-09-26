


CREATE PROCEDURE [dbo].[PAFocus_Generate] @PeriodId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160830@Razor.Nguyen
Task: PA Focus
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastObjectiveId INT
		,@nextObjectiveId INT
		,@EmployeeId INT;

	CREATE TABLE #EmployeeIds (EmployeeId INT)

	INSERT INTO #EmployeeIds
	SELECT DISTINCT e.EmployeeId
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.PeriodFlowSetting AS a WITH (NOLOCK) ON a.EmployeeId = e.EmployeeId
	WHERE e.IsDeleted = 0
		AND e.[Enabled] = 1
		AND e.EmployeeId NOT IN (
			SELECT EmployeeId
			FROM dbo.PAFocus WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			)
		AND a.PeriodId = @PeriodId
		AND a.EmployeeId NOT IN (
			SELECT EmployeeId
			FROM dbo.PeriodExcludedEmployee WITH (NOLOCK)
			WHERE PeriodId = @PeriodId
			)

	BEGIN TRY
		BEGIN TRANSACTION

		WHILE (
				EXISTS (
					SELECT 1
					FROM #EmployeeIds
					)
				)
		BEGIN
			SELECT @EmployeeId = EmployeeId
			FROM #EmployeeIds

			SELECT TOP 1 @lastObjectiveId = ObjectiveId
			FROM dbo.ObjectiveEmployee WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			ORDER BY CreatedDate DESC

			INSERT INTO [dbo].[ObjectiveEmployee]
			VALUES (
				@EmployeeId
				,NULL
				,NULL
				,GETDATE()
				)

			SET @nextObjectiveId = SCOPE_IDENTITY()

			INSERT INTO dbo.PAFocus
			SELECT @PeriodId
				,@EmployeeId
				,@lastObjectiveId
				,@nextObjectiveId
				,NULL
				,NULL
				,NULL
				,NULL
				,NULL
				,NULL
			FROM dbo.Employee AS e WITH (NOLOCK)
			WHERE e.EmployeeId = @EmployeeId

			EXEC dbo.PAFocusProcess_Generate @PeriodId
				,@EmployeeId
				,@ErrCode OUTPUT
				,@ErrMsg OUTPUT

			DELETE
			FROM #EmployeeIds
			WHERE EmployeeId = @EmployeeId
		END

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Generate PAFocus unsuccessfully.EmployeeId = ' + Convert(VARCHAR(10), @EmployeeId);
	END CATCH;

	DROP TABLE #EmployeeIds;
END