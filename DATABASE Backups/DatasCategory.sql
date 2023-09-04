USE [CashBookDB]
GO
SET IDENTITY_INSERT [dbo].[Category] ON 
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (2, N'Food	', N'Expense', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (3, N'Transportation	', N'Expense', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (4, N'Rent	', N'Expense', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (5, N'Home', N'Expense', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (6, N'Entertainments	', N'Expense', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (7, N'Paycheck	', N'Income', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (8, N'Reward	', N'Income', 0)
GO
INSERT [dbo].[Category] ([id], [name], [type], [isDelete]) VALUES (9, N'CashBack	', N'Income', 0)
GO
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
