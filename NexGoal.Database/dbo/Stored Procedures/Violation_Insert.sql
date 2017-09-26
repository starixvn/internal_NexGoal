


CREATE PROCEDURE [dbo].[Violation_Insert] @EmployeeId INT
	,@Description NVARCHAR(max)
	,@CreatedBy INT
	,@EffectedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160901@Razor.Nguyen
Task: Violation management.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Date DATETIME

	SET @Date = GETDATE()

	BEGIN TRY
		INSERT INTO dbo.Violation
		VALUES (
			@EmployeeId
			,@Description
			,1
			,@EffectedDate
			,@CreatedBy
			,@Date
			,NULL
			,NULL
			)
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert unsuccessfully.'
	END CATCH
END