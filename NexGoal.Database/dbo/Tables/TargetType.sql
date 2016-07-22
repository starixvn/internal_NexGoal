CREATE TABLE [dbo].[TargetType] (
    [TargetTypeId]     TINYINT        NOT NULL,
    [Name]             NVARCHAR (200) NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [DisplayOrder]     TINYINT        NULL,
    [ScoreTypeId]      TINYINT        NULL,
    [ProcessGroupId]   INT            NULL,
    [TargetTemplateId] INT            NULL,
    [CreatedBy]        INT            NULL,
    [CreatedDate]      DATETIME       NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedDate]     DATETIME       NULL,
    [AllowMultiple]    BIT            NULL
);

