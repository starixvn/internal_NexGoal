CREATE TABLE [dbo].[EPADetail] (
    [EPAId]                INT            NOT NULL,
    [ProcessId]            INT            NOT NULL,
    [EvaluatorId]          INT            NOT NULL,
    [KPIId]                INT            NOT NULL,
    [KPIGroupId]           INT            NOT NULL,
    [TargetTemplateId]     INT            NULL,
    [Score]                NVARCHAR (MAX) NULL,
    [Rate]                 NVARCHAR (MAX) NULL,
    [HasComment]           BIT            NULL,
    [IsRequiredComment]    BIT            NULL,
    [Comment]              NVARCHAR (MAX) NULL,
    [IsRequiredEvaluate]   BIT            NULL,
    [KPILogNo]             VARCHAR (20)   NULL,
    [KPIGroupLogNo]        VARCHAR (20)   NULL,
    [KPIDisplayOrder]      INT            NULL,
    [KPIGroupDisplayOrder] INT            NULL,
    [IsSpreadKPI]          BIT            NULL,
    [KPIGroupWeight]       FLOAT (53)     NULL,
    [KPIWeight]            FLOAT (53)     NULL,
    [ModifiedBy]           INT            NULL,
    [ModifiedDate]         DATETIME       NULL,
    [IsMaxScore]           BIT            NULL,
    CONSTRAINT [PK_EPADetail] PRIMARY KEY CLUSTERED ([EPAId] ASC, [ProcessId] ASC, [EvaluatorId] ASC, [KPIId] ASC, [KPIGroupId] ASC)
);



