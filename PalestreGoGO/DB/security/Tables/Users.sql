CREATE TABLE [security].[Users]
(
	[Id]					UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID()),
	[AccessFailedCount]		INT				NOT NULL,
	[ConcurrencyStamp]		NVARCHAR(MAX)	NULL,
	[Email]					NVARCHAR(256)	NULL,
	[EmailConfirmed]		BIT				NOT NULL,
	[LockoutEnabled]		BIT				NOT NULL,
	[LockoutEnd]			DATETIMEOFFSET(7) NULL,
	[NormalizedEmail]		NVARCHAR(256) NULL,
	[NormalizedUserName]	NVARCHAR(256) NULL,
	[NumeroCellulare]		NVARCHAR(MAX) NULL,
	[PasswordHash]			NVARCHAR(MAX) NULL,
	[PhoneNumber]			NVARCHAR(MAX) NULL,
	[PhoneNumberConfirmed]	BIT NOT NULL,
	[SecurityStamp]			NVARCHAR(MAX) NULL,
	[TwoFactorEnabled]		BIT NOT NULL,
	[UserName]				NVARCHAR(256) NULL,
	[FirstName]				NVARCHAR(MAX)	NULL,
	[LastName]				NVARCHAR(MAX) NULL,
	[CreationToken]			NVARCHAR(MAX) NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED (Id)
)