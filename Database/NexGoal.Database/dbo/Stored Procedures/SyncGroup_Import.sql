CREATE OR ALTER   PROCEDURE [dbo].[SyncGroup_Import]	
	@GroupXml NVARCHAR(MAX),
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncGroup_Import]	
	@GroupXml ='<ROOT>
	<Group GroupId='1' Code='G001' Name='Group 1' DepartmentId='1' Rank='1' IsDeleted='0' ParentId='0' ModifiedDate='2024-03-20'></Group>
	</ROOT>'
*/
BEGIN
	SET NOCOUNT ON;

	-- For write log
	DECLARE 
		@StartRun					DATETIME,
		@EndRun						DATETIME,
		@Duration					INT,
		@LogTraderData				NVARCHAR(MAX),	
		@LogTraderOldData			NVARCHAR(MAX),	
		@Xml						XML		 

	DECLARE @DocHandler INT

	SELECT @GroupXml = REPLACE(ISNULL(@GroupXml,''),'&','&amp;')
	
	IF OBJECT_ID('tempdb..#Group') IS NOT NULL
	BEGIN
		DROP TABLE #Group
	END

	CREATE TABLE #Group (		
		GroupId INT,
		Code VARCHAR(20),
		Name NVARCHAR(50),
		DepartmentId INT,
		Rank INT,
		IsDeleted BIT,
		ParentId INT,
		ModifiedDate DATETIME
	)
 
	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT, @GroupXml

		INSERT INTO #Group
		SELECT
			GroupId,
			Code,
			Name,
			DepartmentId,
			Rank,
			IsDeleted,
			ParentId,
			CASE WHEN ISDATE(ModifiedDate) = 1 THEN CAST(ModifiedDate AS DATETIME) ELSE NULL END
		FROM OPENXML(@DocHandler, '/ROOT/Group', 1) WITH (
			GroupId INT,
			Code VARCHAR(20),
			Name NVARCHAR(50),
			DepartmentId INT,
			Rank INT,
			IsDeleted BIT,
			ParentId INT,
			ModifiedDate NVARCHAR(50)
		)

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = N'Parameter invalid.'
		RETURN
	END CATCH;
	 
	MERGE INTO [dbo].[Group] AS Target
	USING #Group AS Source
	ON Target.GroupId = Source.GroupId
	WHEN MATCHED THEN
		UPDATE SET
			Target.Code = Source.Code,
			Target.Name = Source.Name,
			Target.DepartmentId = Source.DepartmentId,
			Target.Rank = Source.Rank,
			Target.IsDeleted = Source.IsDeleted,
			Target.ParentId = Source.ParentId,
			Target.ModifiedDate = Source.ModifiedDate,
			Target.SyncDate = GETDATE()
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			GroupId, Code, Name, DepartmentId, Rank, IsDeleted, ParentId, ModifiedDate, SyncDate
		)
		VALUES (
			Source.GroupId, Source.Code, Source.Name, Source.DepartmentId, Source.Rank,
			Source.IsDeleted, Source.ParentId, Source.ModifiedDate, GETDATE()
		);

	SELECT @ErrCode = 0, @ErrMsg = '';

	DROP TABLE #Group;
END