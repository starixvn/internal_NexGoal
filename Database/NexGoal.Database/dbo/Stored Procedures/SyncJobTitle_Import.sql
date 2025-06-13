CREATE OR ALTER PROCEDURE [dbo].[SyncJobTitle_Import]    
    @JobTitleXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncJobTitle_Import]    
    @JobTitleXml ='<ROOT>
    <JobTitle Id='1' Name='Software Engineer' ModifiedDate='2024-03-20'></JobTitle>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @JobTitleXml = REPLACE(ISNULL(@JobTitleXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
    BEGIN
        DROP TABLE #JobTitle
    END

    CREATE TABLE #JobTitle (        
        JobTitleId INT,
        Name NVARCHAR(100),
        ModifiedDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @JobTitleXml

        INSERT INTO #JobTitle
        SELECT
            Id,
            Name,
            ModifiedDate
        FROM OPENXML(@DocHandler, '/ROOT/JobTitle', 1) WITH (
            Id INT,
            Name NVARCHAR(100),
            ModifiedDate DATETIME
        )

        EXEC sp_xml_removedocument @DocHandler
    END TRY
    BEGIN CATCH
        IF @DocHandler IS NOT NULL
            EXEC sp_xml_removedocument @DocHandler

        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
        RETURN
    END CATCH;
     
    BEGIN TRY
        MERGE INTO [dbo].[JobTitle] AS Target
        USING #JobTitle AS Source
        ON Target.JobTitleId = Source.JobTitleId
        WHEN MATCHED THEN
            UPDATE SET
                Target.Name = Source.Name,
                Target.ModifiedDate = Source.ModifiedDate,
                Target.SyncDate = GETDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (
                JobTitleId, Name, ModifiedDate, SyncDate
            )
            VALUES (
                Source.JobTitleId, Source.Name, Source.ModifiedDate, GETDATE()
            );

        SELECT @ErrCode = 0, @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
    END CATCH;

    IF OBJECT_ID('tempdb..#JobTitle') IS NOT NULL
        DROP TABLE #JobTitle;
END 