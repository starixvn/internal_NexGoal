CREATE TABLE [dbo].[Discipline] (
    [DisciplineId]  INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]    INT            NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [IsActive]      BIT            NULL,
    [EffectiveDate] DATE           NULL,
    [CreatedBy]     INT            NULL,
    [CreateDate]    DATETIME       NULL,
    [ModifiedBy]    INT            NULL,
    [ModifiedDate]  DATETIME       NULL,
    CONSTRAINT [PK_DisciplineId] PRIMARY KEY CLUSTERED ([DisciplineId] ASC) ON [PRIMARY]
) TEXTIMAGE_ON [PRIMARY];

