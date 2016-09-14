CREATE TABLE [Jp].[Info]
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
	CONSTRAINT PK_Info PRIMARY KEY(InfoId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Info_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_Okopfs_Jp_Info_OkopfId FOREIGN KEY (OkopfId) REFERENCES Dicts.[Okopfs](OkopfId),
	CONSTRAINT FK_Dicts_JuridicalPersonFinancialStates_Jp_Info_JuridicalPersonFinancialStateId FOREIGN KEY (JuridicalPersonFinancialStateId) REFERENCES Dicts.[JuridicalPersonFinancialStates](JuridicalPersonFinancialStateId),
	CONSTRAINT FK_Dicts_BusinessAreas_Jp_Info_BusinessAreaId FOREIGN KEY (BusinessAreaId) REFERENCES Dicts.[BusinessAreas](BusinessAreaId),
)

