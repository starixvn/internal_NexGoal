CREATE TABLE [dbo].[Notification] (
    [NotificationId] INT            IDENTITY (1, 1) NOT NULL,
    [CreatorId]      INT            NOT NULL,
    [EmployeeId]     INT            NOT NULL,
    [Content]        NVARCHAR (MAX) NOT NULL,
    [StatusId]       TINYINT        NOT NULL,
    [ActionTypeId]   TINYINT        NULL,
    [RecordId]       INT            NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([NotificationId] ASC)
);



