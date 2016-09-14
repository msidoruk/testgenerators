CREATE TABLE [Jp].[StateReg]
(
	[StateRegId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[RegistrationDate] DATETIME NULL,
	[RegistrationPlace] VARCHAR (100) NULL,
	[RegisteredByDept] VARCHAR (100) NULL,
	[OGRN] VARCHAR (50) NULL,
	[INN] VARCHAR (50) NULL,
	[KPP] VARCHAR (50) NULL,
	[OKATO] VARCHAR (50) NULL,
	[OKTMO] VARCHAR (50) NULL,
	[OKFS] VARCHAR (50) NULL,
	[OKPO] VARCHAR (50) NULL,
	[OKOPF] VARCHAR (50) NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	[Description] VARCHAR (200) NULL,
	-- Constraints
	CONSTRAINT PK_StateReg PRIMARY KEY(StateRegId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_StateReg_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
)

