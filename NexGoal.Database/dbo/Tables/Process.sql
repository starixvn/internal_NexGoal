CREATE TABLE [dbo].[Process] (
    [ProcessId]           INT            NOT NULL,
    [Name]                NVARCHAR (50)  NOT NULL,
    [Description]         NVARCHAR (500) NULL,
    [ProcessGroupId]      INT            NULL,
    [ProcessDays]         INT            NULL,
    [Priority]            INT            NULL,
    [CreatedBy]           INT            NULL,
    [CreatedDate]         DATETIME       NULL,
    [ModifiedBy]          INT            NULL,
    [ModifiedDate]        DATETIME       NULL,
    [IsEvaluatedByLeader] BIT            NULL,
    [UpperLevel]          TINYINT        NULL,
    [IsSelfEvaluated]     BIT            NULL
);

