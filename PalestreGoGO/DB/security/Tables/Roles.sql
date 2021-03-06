CREATE TABLE [security].[Roles]
(
	[Id]				UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
	[ConcurrencyStamp]	NVARCHAR(MAX) NULL,
	[Name]				NVARCHAR(256) NULL,
	[NormalizedName]	NVARCHAR(256) NULL,
	
	CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
)