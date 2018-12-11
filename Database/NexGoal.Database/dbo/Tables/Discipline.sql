CREATE TABLE [dbo].[Discipline] (
    [DisciplineId]  INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]    INT            NOT NULL,
    [Description]   NVARCHAR (MAX) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [EffectiveDate] DATE           NOT NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [ModifiedBy]    INT            NULL,
    [ModifiedDate]  DATETIME       NULL,
    CONSTRAINT [PK_DisciplineId] PRIMARY KEY CLUSTERED ([DisciplineId] ASC) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];



