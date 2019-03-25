CREATE TABLE [dbo].[PerformanceGrade] (
    [GradeId]     INT            NOT NULL,
    [Name]        VARCHAR (10)   NOT NULL,
    [Description] NVARCHAR (100) NULL,
    [Point]       REAL           NOT NULL,
	[Explanation] NVARCHAR(500)	NOT NULL,
    CONSTRAINT [PK_PerformanceGrade] PRIMARY KEY CLUSTERED ([GradeId] ASC)
);

