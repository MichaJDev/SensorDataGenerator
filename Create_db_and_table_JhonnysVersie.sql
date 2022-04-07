USE [master]
GO

CREATE DATABASE [SensorData]
GO

USE [SensorData]
GO

CREATE TABLE [dbo].[SensorReading](
	[EntryId] [bigint] IDENTITY(1,1) NOT NULL,
	[SensorId] [varchar](255) NULL,
	[People_in] [bigint] NULL,
	[People_out] [bigint] NULL,
	[TimeStamp] [varchar](12) NULL,
 CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED
(
	[EntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PresureSensors](
	[EnrtyId] [bigint] IDENTITY(1,1) NOT NULL,
	[SensorId] [varchar](255) NOT NULL,
	[InUse] [bit] NOT NULL,
	[TimeStamp] [varchar](255) NOT NULL,
 	CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED (
		[EnrtyId]
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
