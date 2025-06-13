CREATE OR ALTER PROCEDURE [dbo].[SyncTOEIC_Import]    
    @TOEICXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncTOEIC_Import]    
    @TOEICXml ='<ROOT>
    <TOEIC EmpId='1' MinimumScore='600' Listening='300' Reading='300' 
        LRExamDate='2024-03-20' InputDate='2024-03-20' ModifiedDate='2024-03-20'></TOEIC>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @TOEICXml = REPLACE(ISNULL(@TOEICXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#TOEIC') IS NOT NULL
    BEGIN
        DROP TABLE #TOEIC
    END

    CREATE TABLE #TOEIC (        
        EmpId INT,
        MinimumScore INT,
        Listening INT,
        Reading INT,
        LRExamDate DATETIME,
        InputDate DATETIME,
        ModifiedDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @TOEICXml

        INSERT INTO #TOEIC
        SELECT
            EmpId,
            MinimumScore,
            Listening,
            Reading,
            LRExamDate,
            InputDate,
            ModifiedDate
        FROM OPENXML(@DocHandler, '/ROOT/TOEIC', 1) WITH (
            EmpId INT,
            MinimumScore INT,
            Listening INT,
            Reading INT,
            LRExamDate DATETIME,
            InputDate DATETIME,
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
        MERGE INTO [dbo].[TOEIC] AS Target
        USING #TOEIC AS Source
        ON Target.EmpId = Source.EmpId
        WHEN MATCHED THEN
            UPDATE SET
                Target.MinimumScore = Source.MinimumScore,
                Target.Listening = Source.Listening,
                Target.Reading = Source.Reading,
                Target.LRExamDate = Source.LRExamDate,
                Target.InputDate = Source.InputDate,
                Target.ModifiedDate = Source.ModifiedDate,
                Target.SyncDate = GETDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (
                EmpId, MinimumScore, Listening, Reading, 
                LRExamDate, InputDate, ModifiedDate, SyncDate
            )
            VALUES (
                Source.EmpId, Source.MinimumScore, Source.Listening, Source.Reading, 
                Source.LRExamDate, Source.InputDate, Source.ModifiedDate, GETDATE()
            );

        SELECT @ErrCode = 0, @ErrMsg = '';
    END TRY
    BEGIN CATCH
        SELECT @ErrCode = ERROR_NUMBER()
            ,@ErrMsg = ERROR_MESSAGE()
    END CATCH;

    IF OBJECT_ID('tempdb..#TOEIC') IS NOT NULL
        DROP TABLE #TOEIC;
END 