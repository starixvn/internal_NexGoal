CREATE TABLE [dbo].[TargetTemplate] (
    [TargetTemplateId]  INT            NOT NULL,
    [Name]              NVARCHAR (200) NOT NULL,
    [ScoreRatingTypeId] INT            NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [TargetTypeId]      TINYINT        NULL,
    [IsActive]          BIT            NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [RatingTypeId]      INT            NULL
);

