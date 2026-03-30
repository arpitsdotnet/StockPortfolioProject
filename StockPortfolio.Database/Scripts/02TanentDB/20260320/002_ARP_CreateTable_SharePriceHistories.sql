SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SharePriceHistories](
	[SharePriceId] [bigint] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CreatedById] [int] NOT NULL,
	[LastModifiedOn] [datetime2](7) NOT NULL,
	[LastModifiedById] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[SecurityId] [int] NOT NULL,
	[SeriesDate] [datetime2] NOT NULL,
	[Open] [decimal](18, 4) NOT NULL,
	[High] [decimal](18, 4) NOT NULL,
	[Low] [decimal](18, 4) NOT NULL,
	[Close] [decimal](18, 4) NOT NULL,
	[Volume] [bigint] NOT NULL,
CONSTRAINT [PK_SharePriceHistories] PRIMARY KEY CLUSTERED ([SharePriceId] ASC))