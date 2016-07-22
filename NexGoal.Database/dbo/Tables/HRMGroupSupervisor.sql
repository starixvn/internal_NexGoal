CREATE TABLE [dbo].[HRMGroupSupervisor] (
    [OwnerId]      INT        NOT NULL,
    [GroupId]      INT        NOT NULL,
    [ActionType]   TINYINT    NOT NULL,
    [RequestType]  TINYINT    NOT NULL,
    [ModifiedBy]   INT        NULL,
    [ModifiedDate] DATETIME   NULL,
    [SyncDate]     NCHAR (10) NULL
);

