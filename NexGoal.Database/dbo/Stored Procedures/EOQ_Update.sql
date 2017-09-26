





CREATE PROCEDURE [dbo].[EOQ_Update] @EOQYId INT
	,@Quarter INT
	,@Year INT
	,@StartDate DATE
	,@EndDate DATE
	,@RemindDate DATE
	,@Description NVARCHAR(200) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160901@Tim.Trinh
Task: Update EOQ.
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
	SET NOCOUNT ON

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
				AND EOQYId != @EOQYId
				AND (
					@StartDate BETWEEN StartDate
						AND EndDate
					OR @EndDate BETWEEN StartDate
						AND EndDate
					)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Start Date/End Date overlaps on another EOQ.'

		RETURN
	END

IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 1
				AND EOQYId != @EOQYId
				AND [Year] = @Year
				And [Quarter] = @Quarter
			)
	BEGIN
		SELECT @ErrCode = 2
			,@ErrMsg = N'Employee of quater ' + CONVERT(NVARCHAR, @Quarter) + ' already exists.'

		RETURN
	END
	
	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE dbo.EOQY WITH (ROWLOCK, UPDLOCK)
		SET StartDate = @StartDate
			,EndDate = @EndDate
			,RemindDate = @RemindDate
			,[Quarter] = @Quarter
			,[Year] = @Year
			,[Description] = @Description
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = GETDATE()
		WHERE EOQYId = @EOQYId
			AND IsQuarter = 1

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH
END