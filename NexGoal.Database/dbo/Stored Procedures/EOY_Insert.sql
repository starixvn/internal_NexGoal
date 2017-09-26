






CREATE PROCEDURE [dbo].[EOY_Insert] @EOQYId INT OUTPUT
	,@Year INT
	,@StartDate DATE
	,@EndDate DATE
	,@RemindDate DATE
	,@Description NVARCHAR(200) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160912@Tim.Trinh
Task: Insert EOY.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATETIME

	SET @Date = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND [Year] = @Year
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'EOY already existes.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND (
					@StartDate BETWEEN StartDate
						AND EndDate
					OR @EndDate BETWEEN StartDate
						AND EndDate
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Start Date/End Date overlaps on another EOY.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO dbo.EOQY (
			[Year]
			,StartDate
			,EndDate
			,RemindDate
			,IsQuarter
			,[Description]
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			@Year
			,@StartDate
			,@EndDate
			,@RemindDate
			,0
			,@Description
			,@CreatedBy
			,@Date
			)

		SELECT @EOQYId = SCOPE_IDENTITY()

		-- copy last nominator setting
		INSERT INTO dbo.EOQYNominatorSetting (
			EOQYId
			,RoleId
			,NominatorId
			,ModifiedBy
			,ModifiedDate
			)
		SELECT DISTINCT @EOQYId
			,setting.RoleId
			,setting.NominatorId
			,@CreatedBy
			,@Date
		FROM dbo.EOQYNominatorSetting AS setting WITH (NOLOCK)
			INNER JOIN dbo.EOQY AS eoq WITH (NOLOCK) ON setting.EOQYId = eoq.EOQYId
		WHERE eoq.IsQuarter = 0
			AND eoq.EndDate IN (
				SELECT MAX(EndDate)
				FROM dbo.EOQY WITH (NOLOCK)
				WHERE EOQYId != @EOQYId
					AND IsQuarter = 0
				)

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert unsuccessfully.'
	END CATCH
END