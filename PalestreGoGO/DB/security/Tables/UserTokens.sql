CREATE TABLE [security].[UserTokens]
(
	[UserId]			UNIQUEIDENTIFIER NOT NULL,
	[LoginProvider]		NVARCHAR(450) NOT NULL,
	[Name]				NVARCHAR(450) NOT NULL,
	[Value]				NVARCHAR(max) NULL,

	CONSTRAINT [PK_UserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
	CONSTRAINT [FK_UserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])REFERENCES [security].[Users] ([Id])
)