CREATE TABLE [Jp].[Infos]
(
	[InfoId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[OkopfId] INT NULL,
	[Name] VARCHAR (200) NULL,
	[ShortName] VARCHAR (200) NULL,
	[ForeignName] VARCHAR (200) NULL,
	[IsRegisteredInRussia] BIT NULL,
	[MustBeLicensed] BIT NULL,
	[HaveOwner] BIT NULL,
	[ActsForOwnerInterest] BIT NULL,
	[JuridicalPersonFinancialStateId] INT NULL,
	[BusinessAreaId] INT NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	[Description] VARCHAR (200) NULL,
	-- Constraints
	CONSTRAINT PK_Infos PRIMARY KEY(InfoId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Infos_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_Okopfs_Jp_Infos_OkopfId FOREIGN KEY (OkopfId) REFERENCES Dicts.[Okopfs](OkopfId),
	CONSTRAINT FK_Dicts_JuridicalPersonFinancialStates_Jp_Infos_JuridicalPersonFinancialStateId FOREIGN KEY (JuridicalPersonFinancialStateId) REFERENCES Dicts.[JuridicalPersonFinancialStates](JuridicalPersonFinancialStateId),
	CONSTRAINT FK_Dicts_BusinessAreas_Jp_Infos_BusinessAreaId FOREIGN KEY (BusinessAreaId) REFERENCES Dicts.[BusinessAreas](BusinessAreaId),
)

