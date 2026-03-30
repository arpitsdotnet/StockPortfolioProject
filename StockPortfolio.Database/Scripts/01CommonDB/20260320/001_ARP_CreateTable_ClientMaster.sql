SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientMaster](
	[ClientId] [int] IDENTITY(1001,1) NOT NULL,
	[CreatedOn] [datetime2] NOT NULL,
	[CreatedById] [int] NOT NULL,
	[LastModifiedOn] [datetime2] NOT NULL,
	[LastModifiedById] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ClientCode] [nvarchar](20) NOT NULL,
	[ClientShortName] [nvarchar](50) NOT NULL,
	[ClientName] [nvarchar](200) NOT NULL,
	[Address] [nvarchar](500) NULL,
	[CityId] [nvarchar](100) NULL,
	[StateId] [nvarchar](100) NULL,
	[CountryId] [nvarchar](100) NULL,
	[PinCode] [nvarchar](20) NULL,
	[AdminLoginId] [int] NOT NULL,
	[AdminEmail] [nvarchar](100) NOT NULL,
	[AdminMobile] [nvarchar](20) NOT NULL,
CONSTRAINT [PK_ClientMaster] PRIMARY KEY CLUSTERED([ClientId]))