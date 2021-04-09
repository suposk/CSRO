IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CSRO.TokenCacheDb')
BEGIN
  CREATE DATABASE [CSRO.TokenCacheDb];
END;
GO


USE [CSRO.TokenCacheDb]
GO

IF NOT EXISTS (
        SELECT 1
        FROM sys.tables
        WHERE name = 'TokenCache'
            AND type = 'U'
        )
BEGIN

CREATE TABLE [dbo].[TokenCache](
    [Id] [nvarchar](449) NOT NULL,
    [Value] [varbinary](max) NOT NULL,
    [ExpiresAtTime] [datetimeoffset](7) NOT NULL,
    [SlidingExpirationInSeconds] [bigint] NULL,
    [AbsoluteExpiration] [datetimeoffset](7) NULL,
 CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
       IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, 
       ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[TokenCache]
(
    [ExpiresAtTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
       SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, 
       ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
       ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END
