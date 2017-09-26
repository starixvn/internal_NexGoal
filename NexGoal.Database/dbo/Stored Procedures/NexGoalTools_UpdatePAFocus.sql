


CREATE PROCEDURE [dbo].[NexGoalTools_UpdatePAFocus] @PAFocusXML NVARCHAR(max)	
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
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

	DECLARE @DocHandler INT;

	IF OBJECT_ID('tempdb..#Focus') IS NOT NULL
	BEGIN
		DROP TABLE #Focus;
	END;

	CREATE TABLE #Focus (
		PAFocusId INT
		,IsReadyPromotion NVARCHAR(max)
		,IsPoorPerformance NVARCHAR(max)
		,Contribution NVARCHAR(max)
		,BonusGiven NVARCHAR(max)
		);

	BEGIN TRY
		BEGIN TRANSACTION;

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@PAFocusXML

		INSERT INTO #Focus
		SELECT PAFocusId
			,IsReadyPromotion
			,IsPoorPerformance
			,Contribution
			,BonusGiven
		FROM OPENXML(@DocHandler, '/ROOT/PAFocus', 1) WITH (PAFocusId INT, IsReadyPromotion NVARCHAR(max), IsPoorPerformance NVARCHAR(max), Contribution NVARCHAR(max), BonusGiven NVARCHAR(max)) AS Data;

		UPDATE pa
		SET pa.IsReadyPromotion = f.IsReadyPromotion
			,pa.IsPoorPerformance = f.IsPoorPerformance
			,pa.Contribution = f.Contribution
			,pa.BonusGiven = f.BonusGiven
		FROM dbo.PAFocus AS pa
			INNER JOIN #Focus AS f ON f.PAFocusId = pa.PAFocusId

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION;

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update data following a new certificate unsuccessfully.';
	END CATCH

	DROP TABLE #Focus
END