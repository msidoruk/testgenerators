CREATE TABLE [Jp].[StateRegOkveds]
(
	[StateRegOkvedId] INT IDENTITY(1,1) NOT NULL,
	[StateRegistrationId] INT NULL,
	[Okved] VARCHAR (50) NULL,
	-- Constraints
	CONSTRAINT PK_StateRegOkveds PRIMARY KEY(StateRegOkvedId),
	CONSTRAINT FK_Jp_StateRegistrations_Jp_StateRegOkveds_StateRegistrationId FOREIGN KEY (StateRegistrationId) REFERENCES Jp.[StateRegistrations](StateRegistrationId),
)

