USE [CashBookDB]
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 
GO
INSERT [dbo].[Payment] ([id], [name], [openingBalance], [isPerson], [isDelete]) VALUES (2, N'Cash', CAST(0.00 AS Decimal(18, 2)), 0, 0)
GO
INSERT [dbo].[Payment] ([id], [name], [openingBalance], [isPerson], [isDelete]) VALUES (3, N'Canara Bank', CAST(0.00 AS Decimal(18, 2)), 0, 0)
GO
INSERT [dbo].[Payment] ([id], [name], [openingBalance], [isPerson], [isDelete]) VALUES (4, N'Axis Bank', CAST(0.00 AS Decimal(18, 2)), 0, 0)
GO
INSERT [dbo].[Payment] ([id], [name], [openingBalance], [isPerson], [isDelete]) VALUES (5, N'Amazon Wallet', CAST(0.00 AS Decimal(18, 2)), 0, 0)
GO
INSERT [dbo].[Payment] ([id], [name], [openingBalance], [isPerson], [isDelete]) VALUES (6, N'Syamanthak', CAST(0.00 AS Decimal(18, 2)), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
