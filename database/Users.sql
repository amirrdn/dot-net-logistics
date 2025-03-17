/*
 Navicat Premium Data Transfer

 Source Server         : sql server
 Source Server Type    : SQL Server
 Source Server Version : 15004430 (15.00.4430)
 Source Host           : localhost:1433
 Source Catalog        : logistik_db
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 15004430 (15.00.4430)
 File Encoding         : 65001

 Date: 18/03/2025 00:57:19
*/


-- ----------------------------
-- Table structure for Users
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type IN ('U'))
	DROP TABLE [dbo].[Users]
GO

CREATE TABLE [dbo].[Users] (
  [Id] int  IDENTITY(1,1) NOT NULL,
  [Username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Password] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [FullName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Email] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Role] nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [IsActive] bit  NOT NULL,
  [CreatedAt] datetime2(7)  NOT NULL,
  [LastLoginAt] datetime2(7)  NULL
)
GO

ALTER TABLE [dbo].[Users] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Users
-- ----------------------------
BEGIN TRANSACTION
GO

SET IDENTITY_INSERT [dbo].[Users] ON
GO

INSERT INTO [dbo].[Users] ([Id], [Username], [Password], [FullName], [Email], [Role], [IsActive], [CreatedAt], [LastLoginAt]) VALUES (N'4', N'admin', N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', N'Admin', N'admin@gmail.com', N'Admin', N'1', N'2025-03-17 16:31:31.4123750', N'2025-03-17 17:49:36.8231930')
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO

COMMIT
GO


-- ----------------------------
-- Auto increment value for Users
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 4)
GO


-- ----------------------------
-- Indexes structure for table Users
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
ON [dbo].[Users] (
  [Email] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username]
ON [dbo].[Users] (
  [Username] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

