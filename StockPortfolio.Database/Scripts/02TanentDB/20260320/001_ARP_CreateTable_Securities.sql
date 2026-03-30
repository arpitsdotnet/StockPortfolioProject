SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Securities](
	[SecurityId] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2] NOT NULL,
	[CreatedById] [int] NOT NULL,
	[LastModifiedOn] [datetime2] NOT NULL,
	[LastModifiedById] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Symbol] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Exchange] [nvarchar](50) NULL,
	[SecurityType] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](10) NULL,
	[ISIN] [nvarchar](20) NULL,
	[Sector] [nvarchar](100) NULL,
	[Industry] [nvarchar](100) NULL,
CONSTRAINT [PK_Securities] PRIMARY KEY CLUSTERED ([SecurityId] ASC))