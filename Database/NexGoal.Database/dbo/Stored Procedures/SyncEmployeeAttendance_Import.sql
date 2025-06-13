CREATE OR ALTER PROCEDURE [dbo].[SyncEmployeeAttendance_Import]    
    @AttendanceXml NVARCHAR(MAX),
    @ErrCode INT = 0 OUTPUT,
    @ErrMsg nvarchar(200) = '' OUTPUT
AS
/*
EXEC [dbo].[SyncEmployeeAttendance_Import]    
    @AttendanceXml ='<ROOT>
    <EmployeeAttendance Id='1' EmpId='1' AttendanceDate='2024-03-20' InFixed='2024-03-20 08:00' OutFixed='2024-03-20 17:00' 
        LeaveTypeId='0' TotalWorkHours='8.00' LeaveDay='0' LeaveTypeId2='0' InsertDate='2024-03-20'></EmployeeAttendance>
    </ROOT>'
*/
BEGIN
    SET NOCOUNT ON;

    DECLARE @DocHandler INT

    SELECT @AttendanceXml = REPLACE(ISNULL(@AttendanceXml,''),'&','&amp;')
    
    IF OBJECT_ID('tempdb..#EmployeeAttendance') IS NOT NULL
    BEGIN
        DROP TABLE #EmployeeAttendance
    END

    CREATE TABLE #EmployeeAttendance (        
        Id BIGINT,
        EmpId INT,
        AttendanceDate DATETIME,
        InFixed DATETIME,
        OutFixed DATETIME,
        LeaveTypeId INT,
        TotalWorkHours DECIMAL(18,2),
        LeaveDay DECIMAL(18,1),
        LeaveTypeId2 INT,
        LateIn INT,
        EarlyOut INT,
        Absence BIT,
        InsertDate DATETIME
    )
 
    BEGIN TRY
        EXEC sp_xml_preparedocument @DocHandler OUTPUT, @AttendanceXml

        INSERT INTO #EmployeeAttendance
        SELECT
            Id,
            EmpId,
            AttendanceDate,
            CASE WHEN ISDATE(InFixed) = 1 THEN CAST(InFixed AS DATETIME) ELSE NULL END,
            CASE WHEN ISDATE(OutFixed) = 1 THEN CAST(OutFixed AS DATETIME) ELSE NULL END,
            LeaveTypeId,
            CASE 
                WHEN ISNUMERIC(TotalWorkHours) = 1 THEN CAST(TotalWorkHours AS DECIMAL(18,2))
                ELSE NULL 
            END,
            CASE 
                WHEN ISNUMERIC(LeaveDay) = 1 THEN CAST(LeaveDay AS DECIMAL(18,1))
                ELSE NULL   
            END,
            LeaveTypeId2,
            LateIn,
            EarlyOut,
            Absence,
            CASE WHEN ISDATE(InsertDate) = 1 THEN CAST(InsertDate AS DATETIME) ELSE NULL END
        FROM OPENXML(@DocHandler, '/ROOT/EmployeeAttendance', 1) WITH (
            Id BIGINT,
            EmpId INT,
            AttendanceDate DATETIME,
            InFixed NVARCHAR(50),
            OutFixed NVARCHAR(50),
            LeaveTypeId INT,
            TotalWorkHours NVARCHAR(50),
            LeaveDay NVARCHAR(50),
            LeaveTypeId2 INT,
            LateIn INT,
            EarlyOut INT,
            Absence BIT,
            InsertDate NVARCHAR(50)
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
     
    MERGE INTO [dbo].[EmployeeAttendance] AS Target
    USING #EmployeeAttendance AS Source
    ON Target.Id = Source.Id
    WHEN MATCHED THEN
        UPDATE SET
            Target.EmpId = Source.EmpId,
            Target.AttendanceDate = Source.AttendanceDate,
            Target.InFixed = Source.InFixed,
            Target.OutFixed = Source.OutFixed,
            Target.LeaveTypeId = Source.LeaveTypeId,
            Target.TotalWorkHours = Source.TotalWorkHours,
            Target.LeaveDay = Source.LeaveDay,
            Target.LeaveTypeId2 = Source.LeaveTypeId2,
            Target.LateIn = Source.LateIn,
            Target.EarlyOut = Source.EarlyOut,
            Target.Absence = Source.Absence,
            Target.InsertDate = Source.InsertDate,
            Target.SyncDate = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Id, EmpId, AttendanceDate, InFixed, OutFixed, 
            LeaveTypeId, TotalWorkHours, LeaveDay, LeaveTypeId2,
            LateIn, EarlyOut,
            Absence,
            InsertDate, SyncDate
        )
        VALUES (
            Source.Id, Source.EmpId, Source.AttendanceDate, Source.InFixed, Source.OutFixed, 
            Source.LeaveTypeId, Source.TotalWorkHours, Source.LeaveDay, Source.LeaveTypeId2,
            Source.LateIn, Source.EarlyOut,
            Source.Absence,
            Source.InsertDate, GETDATE()
        );

    SELECT @ErrCode = 0, @ErrMsg = '';

    DROP TABLE #EmployeeAttendance;
END 