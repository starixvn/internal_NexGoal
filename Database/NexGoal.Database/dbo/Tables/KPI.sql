CREATE TABLE [dbo].[KPI] (
    [KPIId]      INT            IDENTITY (1, 1) NOT NULL,
    [KPIGroupId] INT            NULL,
    [Name]       NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([KPIId] ASC)
);



