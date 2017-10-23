CREATE PROCEDURE [dbo].[EOY_Update] @EOQYId INT
	,@Year INT
	,@StartDate DATE
	,@EndDate DATE
	,@RemindDate DATE
	,@EndDateForCommittee DATE
	,@Description NVARCHAR(200) = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160912@Tim.Trinh
Task: Update EOY.
DB: NexGoal
Original: 

Revisions:
    20170929@Tim.Trinh: add EndDateForCommittee  
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
			WHERE IsQuarter = 0
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
			,@ErrMsg = N'Start Date/End Date overlaps on another EOY.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EOQY WITH (NOLOCK)
			WHERE IsQuarter = 0
				AND EOQYId != @EOQYId
				AND [Year] = @Year
			)
	BEGIN
		SELECT @ErrCode = 2
			,@ErrMsg = N'Employee of year ' + CONVERT(NVARCHAR, @year) + ' already exists.'

		RETURN
	END

	UPDATE dbo.EOQY WITH (ROWLOCK, UPDLOCK)
	SET StartDate = @StartDate
		,EndDate = @EndDate
		,RemindDate = @RemindDate
		,EndDateForCommittee = @EndDateForCommittee
		,[Year] = @Year
		,[Description] = @Description
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = GETDATE()
	WHERE EOQYId = @EOQYId
		AND IsQuarter = 0
END