CREATE TABLE [dbo].[ScoreRatingType] (
    [ScoreRatingTypeId] INT            NOT NULL,
    [Name]              VARCHAR (100)  NOT NULL,
    [IsActive]          BIT            NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [SelfEvaluated]     FLOAT (53)     NULL,
    [Evaluator]         FLOAT (53)     NULL,
    [Reviewer]          FLOAT (53)     NULL,
    [Management]        FLOAT (53)     NULL,
    [Attendance]        FLOAT (53)     NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [ModifiedDate]      DATETIME       NULL
);

