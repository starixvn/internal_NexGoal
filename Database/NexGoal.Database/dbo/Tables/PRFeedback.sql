CREATE TABLE [dbo].[PRFeedback](
	[PRFeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[PerformanceRecordId] [int] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[RoleId] [tinyint] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PRSubComment_1] PRIMARY KEY CLUSTERED ( [PRFeedbackId] ASC )
 );