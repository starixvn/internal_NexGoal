CREATE TABLE [dbo].[NotificationSetting] (
    [TypeId]              TINYINT NOT NULL,
    [EmployeeId]          INT     NOT NULL,
    [ReceiveNotification] BIT     NOT NULL,
    [ReceiveEmail]        BIT     NOT NULL,
    CONSTRAINT [PK_NotificationSetting] PRIMARY KEY CLUSTERED ([TypeId] ASC, [EmployeeId] ASC)
);

