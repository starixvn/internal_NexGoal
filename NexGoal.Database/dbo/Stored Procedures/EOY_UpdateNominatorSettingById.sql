



CREATE PROCEDURE [dbo].[EOY_UpdateNominatorSettingById] @EOQYId INT
	,@Details NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160912@Tim.Trinh
Task: Update nominator setting.
DB: NexGoal
Original: 

Revisions:  
Param's Explanation:
• @Details: N'
     <ROOT>
        <NominatorSetting RoleId='' NominatorId='' />
     </ROOT>
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON

	CREATE TABLE #NominatorSetting (
		RoleId TINYINT
		,NominatorId INT
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@Details

		INSERT INTO #NominatorSetting
		SELECT RoleId
			,NominatorId
		FROM OPENXML(@DocHandler, '/ROOT/NominatorSetting', 1) WITH (RoleId INT, NominatorId INT) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully. Parameter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		BEGIN TRANSACTION

		DELETE setting
		FROM dbo.EOQYNominatorSetting AS setting
		WHERE setting.EOQYId = @EOQYId
			AND EXISTS (
				SELECT 1
				FROM #NominatorSetting
				WHERE setting.NominatorId = NominatorId
				)

		INSERT INTO dbo.EOQYNominatorSetting (
			EOQYId
			,RoleId
			,NominatorId
			,ModifiedBy
			,ModifiedDate
			)
		SELECT @EOQYId
			,RoleId
			,NominatorId
			,@ModifiedBy
			,GETDATE()
		FROM #NominatorSetting
		WHERE RoleId > 0

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Update unsuccessfully.'
	END CATCH

	DROP TABLE #NominatorSetting
END